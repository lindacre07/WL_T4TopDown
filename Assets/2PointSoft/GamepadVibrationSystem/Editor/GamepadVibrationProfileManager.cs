using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TwoPointSoft.GamepadVibration.Editor
{
    /// <summary>Central searchable library for built-in presets, custom profiles, preview, and generated API.</summary>
    public sealed class GamepadVibrationProfileManager : EditorWindow
    {
        private const float SidebarWidth = 245f;
        private const float DetailPadding = 10f;
        private const float SectionSpacing = 10f;
        private const float ActionSpacing = 6f;
        private const float ActionButtonHeight = 30f;

        private GamepadVibrationProfile[] allProfiles = Array.Empty<GamepadVibrationProfile>();
        private GamepadVibrationProfile selectedProfile;
        private GamepadVibrationPreset selectedBuiltIn = GamepadVibrationPreset.Custom;
        private Vector2 libraryScroll;
        private Vector2 detailScroll;
        private string search = string.Empty;
        private bool showBuiltIns;
        private bool showCreatePanel;
        private string newProfileName = "New Vibration";
        private GamepadVibrationProfileCreationMode creationMode = GamepadVibrationProfileCreationMode.FromScratch;
        private GamepadVibrationPreset sourceBuiltIn = GamepadVibrationPreset.LightHit;
        private GamepadVibrationProfile sourceExisting;
        private GamepadVibrationProfileCreationDraft creationDraft;
        private GamepadVibrationProfileEditState editState;
        private GamepadVibrationHandle previewHandle;
        private GUIStyle titleStyle;
        private GUIStyle selectedRowStyle;
        private GUIStyle badgeStyle;
        private GUIStyle statusStyle;

        [MenuItem("Tools/2 Point Soft/Vibration Profile Manager", priority = 100)]
        public static void Open()
        {
            GamepadVibrationProfileManager window = GetWindow<GamepadVibrationProfileManager>();
            window.titleContent = new GUIContent("Vibration Profiles");
            window.minSize = new Vector2(820f, 640f);
            window.RefreshProfiles();
            window.Show();
        }

        private void OnEnable()
        {
            RefreshProfiles();
            EditorApplication.projectChanged += RefreshProfiles;
        }

        private void OnDisable()
        {
            StopPreview();
            editState?.Dispose();
            editState = null;
            creationDraft?.Dispose();
            creationDraft = null;
            EditorApplication.projectChanged -= RefreshProfiles;
        }

        private void OnGUI()
        {
            BuildStyles();
            DrawHeader();
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawLibraryColumn();
                DrawDetailColumn();
            }
            DrawBottomBar();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space(8f);
            GUILayout.Label("GAMEPAD VIBRATION SYSTEM", titleStyle);
            GUILayout.Label("Vibration Profile Manager", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Space(7f);
        }

        private void DrawLibraryColumn()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(SidebarWidth), GUILayout.ExpandHeight(true)))
            {
                GUILayout.Label("PROFILE LIBRARY", EditorStyles.boldLabel);
                using (new EditorGUILayout.HorizontalScope())
                {
                    search = EditorGUILayout.TextField(search, GUI.skin.FindStyle("ToolbarSearchTextField") ?? EditorStyles.textField);
                    if (GUILayout.Button("×", EditorStyles.miniButton, GUILayout.Width(24f)))
                    {
                        search = string.Empty;
                        GUI.FocusControl(null);
                    }
                }

                libraryScroll = EditorGUILayout.BeginScrollView(libraryScroll);
                DrawBuiltInLibrary();
                EditorGUILayout.Space(4f);
                DrawCustomLibrary();
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space(5f);
                if (GUILayout.Button("+ CREATE PROFILE", GUILayout.Height(30f)))
                    BeginCreateProfile();
                DrawApiActionButton(GUILayout.Height(30f));
            }
        }

        private void DrawBuiltInLibrary()
        {
            showBuiltIns = EditorGUILayout.Foldout(showBuiltIns, "BUILT-IN PRESETS (8)", true);
            if (!showBuiltIns) return;

            foreach (GamepadVibrationPreset preset in GamepadVibrationProfileManagerModel.BuiltInPresets)
            {
                string display = ObjectNames.NicifyVariableName(preset.ToString());
                if (!Matches(display)) continue;
                bool selected = selectedProfile == null && selectedBuiltIn == preset;
                if (DrawLibraryRow(display, "BUILT-IN", selected))
                {
                    StopPreview();
                    showCreatePanel = false;
                    creationDraft?.Dispose();
                    creationDraft = null;
                    editState?.Dispose();
                    editState = null;
                    selectedProfile = null;
                    selectedBuiltIn = preset;
                }
            }
        }

        private void DrawCustomLibrary()
        {
            var filtered = GamepadVibrationProfileManagerModel.FilterCustomProfiles(allProfiles, search);
            GUILayout.Label($"CUSTOM PROFILES ({allProfiles.Length})", EditorStyles.boldLabel);
            if (filtered.Count == 0)
            {
                GUILayout.Label(allProfiles.Length == 0 ? "No custom profiles yet." : "No matching profiles.", EditorStyles.wordWrappedMiniLabel);
                return;
            }

            foreach (GamepadVibrationProfile profile in filtered)
            {
                if (DrawLibraryRow(profile.name, "CUSTOM", selectedProfile == profile))
                {
                    SelectProfile(profile);
                }
            }
            if (filtered.Count != allProfiles.Length)
                GUILayout.Label($"Showing {filtered.Count} of {allProfiles.Length}", EditorStyles.centeredGreyMiniLabel);
        }

        private bool DrawLibraryRow(string label, string badge, bool selected)
        {
            GUIStyle style = selected ? selectedRowStyle : EditorStyles.miniButton;
            using (new EditorGUILayout.HorizontalScope())
            {
                bool clicked = GUILayout.Button(label, style, GUILayout.Height(24f));
                GUILayout.Label(badge, badgeStyle, GUILayout.Width(48f));
                return clicked;
            }
        }

        private void DrawDetailColumn()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                detailScroll = EditorGUILayout.BeginScrollView(detailScroll);

                EditorGUILayout.Space(DetailPadding);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Space(DetailPadding);
                    using (new EditorGUILayout.VerticalScope())
                    {
                        if (showCreatePanel) DrawCreateDetail();
                        else if (selectedProfile != null) DrawCustomDetail(selectedProfile);
                        else if (selectedBuiltIn != GamepadVibrationPreset.Custom) DrawBuiltInDetail(selectedBuiltIn);
                        else DrawEmptyDetail();
                    }
                    GUILayout.Space(DetailPadding);
                }
                EditorGUILayout.Space(DetailPadding);

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawEmptyDetail()
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("SELECT A VIBRATION PROFILE", titleStyle);
            GUILayout.Label("Choose a built-in preset or custom profile from the searchable library.\nOnly one profile is edited at a time, so the window remains compact as the library grows.",
                new GUIStyle(EditorStyles.wordWrappedLabel) { alignment = TextAnchor.MiddleCenter });
            GUILayout.FlexibleSpace();
        }

        private void DrawBuiltInDetail(GamepadVibrationPreset preset)
        {
            GUILayout.Label(ObjectNames.NicifyVariableName(preset.ToString()), titleStyle);
            GUILayout.Label("BUILT-IN • PROTECTED", statusStyle);
            EditorGUILayout.HelpBox("This preset is always available and cannot be edited or deleted. Duplicate it to create an independent custom profile.", MessageType.Info);
            EditorGUILayout.Space(6f);
            GUILayout.Label("VIBRATION SETTINGS", EditorStyles.boldLabel);
            GamepadVibrationSettings settings = GamepadVibrationPresets.Get(preset);
            DrawReadOnlySettings(settings);
            EditorGUILayout.Space(8f);
            EditorGUILayout.HelpBox(
                "Vibration preview is available in Play Mode only. Enter Play Mode to use TEST and STOP.",
                MessageType.Info);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = Application.isPlaying;
                if (GUILayout.Button("TEST", GUILayout.Height(30f))) Preview(settings);
                if (GUILayout.Button("STOP", GUILayout.Height(30f))) StopPreview();
                GUI.enabled = true;
                if (GUILayout.Button("COPY CODE", GUILayout.Height(30f)))
                    Copy($"GamepadVibrationPresets.{preset}.Play();");
            }
            if (GUILayout.Button("DUPLICATE AS CUSTOM", GUILayout.Height(32f))) DuplicateBuiltIn(preset);
        }

        private void DrawCustomDetail(GamepadVibrationProfile profile)
        {
            if (profile == null) return;
            if (editState == null) editState = new GamepadVibrationProfileEditState(profile);
            GamepadVibrationProfile workingProfile = editState.WorkingProfile;

            GUILayout.Label(profile.name, titleStyle);
            GUILayout.Label("CUSTOM PROFILE", statusStyle);
            EditorGUILayout.Space(SectionSpacing);

            SerializedObject serializedProfile = new SerializedObject(workingProfile);
            serializedProfile.Update();

            GUILayout.Label("IDENTITY", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);
            EditorGUILayout.LabelField("Asset Name", profile.name);
            EditorGUILayout.PropertyField(serializedProfile.FindProperty("description"));

            SerializedProperty codeName = serializedProfile.FindProperty("codeName");
            EditorGUILayout.PropertyField(codeName);

            string effectiveCodeName = GamepadVibrationProfileCodeGenerator.ToIdentifier(
                string.IsNullOrWhiteSpace(codeName.stringValue) ? profile.name : codeName.stringValue);

            EditorGUILayout.LabelField("Generated Member", "GamepadVibrationCustom." + effectiveCodeName);

            EditorGUILayout.Space(SectionSpacing);
            GUILayout.Label("PLAYBACK", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);

            SerializedProperty mode = serializedProfile.FindProperty("playbackMode");
            EditorGUILayout.PropertyField(mode);

            EditorGUILayout.Space(SectionSpacing);
            GUILayout.Label("VIBRATION SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);

            EditorGUILayout.PropertyField(serializedProfile.FindProperty("lowFrequency"));
            EditorGUILayout.PropertyField(serializedProfile.FindProperty("highFrequency"));

            GamepadVibrationPlaybackMode selectedMode = (GamepadVibrationPlaybackMode)mode.intValue;
            if (selectedMode != GamepadVibrationPlaybackMode.Continuous)
            {
                EditorGUILayout.PropertyField(serializedProfile.FindProperty("duration"));

                if (selectedMode == GamepadVibrationPlaybackMode.Pulsed)
                {
                    EditorGUILayout.Space(4f);
                    GUILayout.Label("PULSE SETTINGS", EditorStyles.miniBoldLabel);
                    EditorGUILayout.PropertyField(serializedProfile.FindProperty("pulses"));
                    EditorGUILayout.PropertyField(serializedProfile.FindProperty("interval"));
                }

                EditorGUILayout.PropertyField(serializedProfile.FindProperty("fadeOut"));
            }

            EditorGUILayout.Space(SectionSpacing);
            GUILayout.Label("GLOBAL INTENSITY", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);
            EditorGUILayout.PropertyField(serializedProfile.FindProperty("globalIntensity"));

            if (serializedProfile.ApplyModifiedProperties())
                Repaint();

            EditorGUILayout.Space(SectionSpacing + 2f);
            EditorGUILayout.HelpBox(
                "Vibration preview is available in Play Mode only. Enter Play Mode to use TEST and STOP.",
                MessageType.Info);

            EditorGUILayout.Space(ActionSpacing);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = Application.isPlaying;
                if (GUILayout.Button("TEST", GUILayout.Height(ActionButtonHeight))) Preview(workingProfile);
                if (GUILayout.Button("STOP", GUILayout.Height(ActionButtonHeight))) StopPreview();

                GUI.enabled = true;
                if (GUILayout.Button("PING ASSET", GUILayout.Height(ActionButtonHeight))) Ping(profile);
                if (GUILayout.Button("COPY C# CALL", GUILayout.Height(ActionButtonHeight))) ShowCopyMenu(effectiveCodeName);
            }

            EditorGUILayout.Space(ActionSpacing);
            EditorGUILayout.SelectableLabel(
                $"GamepadVibrationCustom.{effectiveCodeName}.Play();",
                EditorStyles.textField,
                GUILayout.Height(EditorGUIUtility.singleLineHeight + 4f));

            bool hasUnsavedChanges = editState.CanReset;
            if (hasUnsavedChanges)
            {
                EditorGUILayout.Space(ActionSpacing);
                EditorGUILayout.HelpBox(
                    "This form contains unsaved changes. Save them or reset to the last saved asset state.",
                    MessageType.Warning);
            }

            EditorGUILayout.Space(ActionSpacing);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = hasUnsavedChanges;
                if (GUILayout.Button("SAVE PROFILE", GUILayout.Height(28f))) SaveProfile(profile);
                if (GUILayout.Button("RESET UNSAVED CHANGES", GUILayout.Height(28f))) ResetUnsavedChanges();
                GUI.enabled = true;
            }

            EditorGUILayout.Space(ActionSpacing);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("DUPLICATE", GUILayout.Height(28f))) DuplicateCustom(profile);
                if (GUILayout.Button("DELETE PROFILE", GUILayout.Height(28f))) DeleteCustom(profile);
            }

            EditorGUILayout.Space(4f);
        }

        private void DrawCreateDetail()
        {
            if (creationDraft == null) BeginCreateProfile();
            GamepadVibrationProfile draft = creationDraft.Profile;

            GUILayout.Label("CREATE VIBRATION PROFILE", titleStyle);
            GUILayout.Label("CONFIGURE BEFORE CREATING", statusStyle);
            EditorGUILayout.Space(SectionSpacing);

            EditorGUILayout.HelpBox(
                "Configure the complete profile here. The asset will be created only after you press CREATE PROFILE.",
                MessageType.Info);

            EditorGUILayout.Space(SectionSpacing);

            // IDENTITY
            GUILayout.Label("IDENTITY", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);

            string previousDefaultCodeName = GamepadVibrationProfileCodeGenerator.ToIdentifier(newProfileName);
            newProfileName = EditorGUILayout.TextField("Profile Name", newProfileName);

            EditorGUILayout.Space(4f);

            EditorGUI.BeginChangeCheck();
            GamepadVibrationProfileCreationMode newMode =
                (GamepadVibrationProfileCreationMode)EditorGUILayout.EnumPopup("Creation Mode", creationMode);

            GamepadVibrationPreset newBuiltIn = sourceBuiltIn;
            GamepadVibrationProfile newExisting = sourceExisting;

            if (newMode == GamepadVibrationProfileCreationMode.CopyBuiltIn)
            {
                EditorGUILayout.Space(2f);
                newBuiltIn = DrawBuiltInPopup(sourceBuiltIn);
            }
            else if (newMode == GamepadVibrationProfileCreationMode.CopyExisting)
            {
                EditorGUILayout.Space(2f);
                newExisting = (GamepadVibrationProfile)EditorGUILayout.ObjectField(
                    "Source Profile", sourceExisting, typeof(GamepadVibrationProfile), false);
            }

            bool sourceChanged = EditorGUI.EndChangeCheck();
            creationMode = newMode;
            sourceBuiltIn = newBuiltIn;
            sourceExisting = newExisting;

            if (sourceChanged) InitializeCreationDraft();

            SerializedObject serializedDraft = new SerializedObject(draft);
            serializedDraft.Update();

            SerializedProperty codeName = serializedDraft.FindProperty("codeName");
            if (string.IsNullOrWhiteSpace(codeName.stringValue) || codeName.stringValue == previousDefaultCodeName)
                codeName.stringValue = GamepadVibrationProfileCodeGenerator.ToIdentifier(newProfileName);

            EditorGUILayout.Space(4f);
            EditorGUILayout.PropertyField(serializedDraft.FindProperty("description"));
            EditorGUILayout.Space(4f);
            EditorGUILayout.PropertyField(codeName);

            EditorGUILayout.LabelField(
                "Generated Member",
                "GamepadVibrationCustom." +
                GamepadVibrationProfileCodeGenerator.ToIdentifier(
                    string.IsNullOrWhiteSpace(codeName.stringValue) ? newProfileName : codeName.stringValue));

            // PLAYBACK
            EditorGUILayout.Space(SectionSpacing);
            GUILayout.Label("PLAYBACK", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);

            SerializedProperty mode = serializedDraft.FindProperty("playbackMode");
            EditorGUILayout.PropertyField(mode);

            // VIBRATION SETTINGS
            EditorGUILayout.Space(SectionSpacing);
            GUILayout.Label("VIBRATION SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);

            EditorGUILayout.PropertyField(serializedDraft.FindProperty("lowFrequency"));
            EditorGUILayout.PropertyField(serializedDraft.FindProperty("highFrequency"));

            GamepadVibrationPlaybackMode playbackMode = (GamepadVibrationPlaybackMode)mode.intValue;
            if (playbackMode != GamepadVibrationPlaybackMode.Continuous)
            {
                EditorGUILayout.PropertyField(serializedDraft.FindProperty("duration"));

                if (playbackMode == GamepadVibrationPlaybackMode.Pulsed)
                {
                    EditorGUILayout.Space(4f);
                    GUILayout.Label("PULSE SETTINGS", EditorStyles.miniBoldLabel);
                    EditorGUILayout.PropertyField(serializedDraft.FindProperty("pulses"));
                    EditorGUILayout.PropertyField(serializedDraft.FindProperty("interval"));
                }

                EditorGUILayout.PropertyField(serializedDraft.FindProperty("fadeOut"));
            }

            // GLOBAL INTENSITY
            EditorGUILayout.Space(SectionSpacing);
            GUILayout.Label("GLOBAL INTENSITY", EditorStyles.boldLabel);
            EditorGUILayout.Space(2f);
            EditorGUILayout.PropertyField(serializedDraft.FindProperty("globalIntensity"));

            serializedDraft.ApplyModifiedProperties();

            // CREATION HELP
            EditorGUILayout.Space(SectionSpacing + 2f);
            EditorGUILayout.HelpBox(CreationHelpText(), MessageType.Info);

            bool canCreate = !string.IsNullOrWhiteSpace(newProfileName) &&
                (creationMode != GamepadVibrationProfileCreationMode.CopyExisting || sourceExisting != null);

            // ACTIONS
            EditorGUILayout.Space(ActionSpacing);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = canCreate;
                if (GUILayout.Button("CREATE PROFILE", GUILayout.Height(34f))) CreateCustomProfile();

                GUI.enabled = true;
                if (GUILayout.Button("CANCEL", GUILayout.Height(34f))) CancelCreateProfile();
            }

            EditorGUILayout.Space(4f);
        }

        private void BeginCreateProfile()
        {
            StopPreview();
            editState?.Dispose();
            editState = null;
            creationDraft?.Dispose();
            creationDraft = new GamepadVibrationProfileCreationDraft();
            showCreatePanel = true;
            selectedProfile = null;
            selectedBuiltIn = GamepadVibrationPreset.Custom;
            newProfileName = "New Vibration";
            creationMode = GamepadVibrationProfileCreationMode.FromScratch;
            sourceBuiltIn = GamepadVibrationPreset.LightHit;
            sourceExisting = null;
            InitializeCreationDraft();
            detailScroll = Vector2.zero;
            Repaint();
        }

        private void InitializeCreationDraft()
        {
            if (creationDraft == null) return;
            GamepadVibrationProfileCreationData data = GamepadVibrationProfileManagerModel.ResolveCreationData(
                creationMode, sourceBuiltIn, sourceExisting);
            creationDraft.Initialize(data);
            creationDraft.Profile.Configure(
                creationDraft.Profile.PlaybackMode,
                creationDraft.Profile.Settings,
                creationDraft.Profile.Description,
                GamepadVibrationProfileCodeGenerator.ToIdentifier(newProfileName));
        }

        private void CancelCreateProfile()
        {
            creationDraft?.Dispose();
            creationDraft = null;
            showCreatePanel = false;
            Repaint();
        }

        private string CreationHelpText()
        {
            switch (creationMode)
            {
                case GamepadVibrationProfileCreationMode.CopyBuiltIn:
                    return "The editor started with the selected built-in values. All settings above can be changed before creation.";
                case GamepadVibrationProfileCreationMode.CopyExisting:
                    return sourceExisting == null
                        ? "Select or search for an existing profile to use as the editable starting point."
                        : "The editor copied the existing profile values. Changes above do not modify the source profile.";
                default:
                    return "The editor started with neutral values. Configure all settings above before creating the asset.";
            }
        }

        private GamepadVibrationPreset DrawBuiltInPopup(GamepadVibrationPreset current)
        {
            string[] names = GamepadVibrationProfileManagerModel.BuiltInPresets
                .Select(value => ObjectNames.NicifyVariableName(value.ToString())).ToArray();
            int index = Array.IndexOf(GamepadVibrationProfileManagerModel.BuiltInPresets.ToArray(), current);
            index = EditorGUILayout.Popup("Built-in Preset", Mathf.Max(0, index), names);
            return GamepadVibrationProfileManagerModel.BuiltInPresets[index];
        }

        private void DrawCreationHelp()
        {
            string message;
            switch (creationMode)
            {
                case GamepadVibrationProfileCreationMode.CopyBuiltIn:
                    message = "Creates an editable independent copy of the selected protected preset.";
                    break;
                case GamepadVibrationProfileCreationMode.CopyExisting:
                    message = "Select or search for an existing profile asset. The new profile is an independent copy.";
                    break;
                default:
                    message = "Creates a new profile with neutral starting values.";
                    break;
            }
            EditorGUILayout.HelpBox(message, MessageType.Info);
        }

        private void DrawApiActionButton(params GUILayoutOption[] options)
        {
            GamepadVibrationProfileApiStatus status = GetCurrentApiStatus();
            Color previousColor = GUI.backgroundColor;
            GUI.backgroundColor = ApiButtonColor(status);
            if (GUILayout.Button(GamepadVibrationProfileManagerModel.GetApiActionLabel(status), options))
                GenerateOrUpdateApi();
            GUI.backgroundColor = previousColor;
        }

        private GamepadVibrationProfileApiStatus GetCurrentApiStatus()
        {
            return GamepadVibrationProfileManagerModel.GetApiStatus(
                allProfiles, GamepadVibrationProfileCodeGenerator.DefaultOutputPath);
        }

        private static Color ApiButtonColor(GamepadVibrationProfileApiStatus status)
        {
            switch (status)
            {
                case GamepadVibrationProfileApiStatus.UpToDate:
                    return EditorGUIUtility.isProSkin
                        ? new Color(0.35f, 0.72f, 0.42f)
                        : new Color(0.45f, 0.82f, 0.50f);
                case GamepadVibrationProfileApiStatus.OutOfDate:
                    return EditorGUIUtility.isProSkin
                        ? new Color(0.95f, 0.63f, 0.20f)
                        : new Color(1f, 0.72f, 0.28f);
                default:
                    return EditorGUIUtility.isProSkin
                        ? new Color(0.82f, 0.34f, 0.30f)
                        : new Color(0.95f, 0.46f, 0.40f);
            }
        }

        private void GenerateOrUpdateApi()
        {
            GamepadVibrationProfileCodeGenerator.GenerateAll();
            RefreshProfiles();
            ShowNotification(new GUIContent("Generated API updated"));
        }

        private void DrawBottomBar()
        {
            GamepadVibrationProfileApiStatus apiStatus = GetCurrentApiStatus();
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label(ApiStatusText(apiStatus), statusStyle, GUILayout.Width(220f));
                GUILayout.FlexibleSpace();
                GUILayout.Label($"{allProfiles.Length} custom profiles", EditorStyles.miniLabel);
                Color previousColor = GUI.backgroundColor;
                GUI.backgroundColor = ApiButtonColor(apiStatus);
                if (GUILayout.Button(
                        GamepadVibrationProfileManagerModel.GetApiActionLabel(apiStatus),
                        EditorStyles.toolbarButton,
                        GUILayout.Width(145f)))
                    GenerateOrUpdateApi();
                GUI.backgroundColor = previousColor;
                GUI.enabled = System.IO.File.Exists(GamepadVibrationProfileCodeGenerator.DefaultOutputPath);
                if (GUILayout.Button("Ping Generated File", EditorStyles.toolbarButton, GUILayout.Width(130f)))
                    Ping(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GamepadVibrationProfileCodeGenerator.DefaultOutputPath));
                GUI.enabled = true;
                if (GUILayout.Button("Copy Using", EditorStyles.toolbarButton, GUILayout.Width(80f)))
                    Copy("using TwoPointSoft.GamepadVibration.Generated;");
            }
        }

        private void CreateCustomProfile()
        {
            if (creationDraft == null || creationDraft.Profile == null) return;
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Vibration Profile", newProfileName.Trim(), "asset",
                "Choose where to save the reusable custom vibration profile.", "Assets/GamepadVibrationProfiles");
            if (string.IsNullOrEmpty(path)) return;

            GamepadVibrationProfile profile = CreateInstance<GamepadVibrationProfile>();
            EditorUtility.CopySerialized(creationDraft.Profile, profile);
            profile.name = newProfileName.Trim();
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            CancelCreateProfile();
            RefreshProfiles();
            SelectAndPing(profile);
        }

        private void DuplicateBuiltIn(GamepadVibrationPreset preset)
        {
            GamepadVibrationProfileCreationData data = GamepadVibrationProfileManagerModel.ResolveCreationData(
                GamepadVibrationProfileCreationMode.CopyBuiltIn, preset, null);
            CreateFromData(ObjectNames.NicifyVariableName(preset.ToString()) + " Custom", data);
        }

        private void DuplicateCustom(GamepadVibrationProfile source)
        {
            GamepadVibrationProfileCreationData data = GamepadVibrationProfileManagerModel.ResolveCreationData(
                GamepadVibrationProfileCreationMode.CopyExisting, GamepadVibrationPreset.Custom, source);
            CreateFromData(source.name + " Copy", data, source.CodeName + "Copy");
        }

        private void CreateFromData(string suggestedName, GamepadVibrationProfileCreationData data, string codeName = null)
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Custom Vibration Profile", suggestedName, "asset",
                "Choose where to save the custom profile.", "Assets/GamepadVibrationProfiles");
            if (string.IsNullOrEmpty(path)) return;
            GamepadVibrationProfile profile = CreateInstance<GamepadVibrationProfile>();
            profile.Configure(data.PlaybackMode, data.Settings, data.Description,
                GamepadVibrationProfileCodeGenerator.ToIdentifier(codeName ?? suggestedName));
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            RefreshProfiles();
            SelectAndPing(profile);
        }

        private void SaveProfile(GamepadVibrationProfile profile)
        {
            if (profile == null || editState == null || !editState.CanReset) return;
            Undo.RecordObject(profile, "Save Vibration Profile");
            editState.SaveTo(profile);
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            Repaint();
        }

        private void ResetUnsavedChanges()
        {
            if (editState == null || !editState.CanReset) return;
            editState.Reset();
            StopPreview();
            GUI.FocusControl(null);
            Repaint();
        }

        private void DeleteCustom(GamepadVibrationProfile profile)
        {
            string path = AssetDatabase.GetAssetPath(profile);
            if (!EditorUtility.DisplayDialog("Delete Vibration Profile",
                    $"Delete ‘{profile.name}’?\n\nExisting component references will become missing. Generated code may need to be updated.",
                    "Delete Profile", "Cancel")) return;
            StopPreview();
            editState?.Dispose();
            editState = null;
            selectedProfile = null;
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
            RefreshProfiles();
        }

        private void RefreshProfiles()
        {
            string selectedPath = selectedProfile != null ? AssetDatabase.GetAssetPath(selectedProfile) : null;
            allProfiles = AssetDatabase.FindAssets("t:GamepadVibrationProfile")
                .Select(guid => AssetDatabase.LoadAssetAtPath<GamepadVibrationProfile>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(profile => profile != null)
                .OrderBy(profile => profile.name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (!string.IsNullOrEmpty(selectedPath))
                selectedProfile = AssetDatabase.LoadAssetAtPath<GamepadVibrationProfile>(selectedPath);
            Repaint();
        }

        private bool Matches(string text)
        {
            return string.IsNullOrWhiteSpace(search) ||
                   text.IndexOf(search.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void SelectProfile(GamepadVibrationProfile profile)
        {
            if (profile == selectedProfile && editState != null && !showCreatePanel) return;
            StopPreview();
            showCreatePanel = false;
            creationDraft?.Dispose();
            creationDraft = null;
            editState?.Dispose();
            editState = null;
            selectedBuiltIn = GamepadVibrationPreset.Custom;
            selectedProfile = profile;
            if (profile != null) editState = new GamepadVibrationProfileEditState(profile);
            Repaint();
        }

        private void SelectAndPing(GamepadVibrationProfile profile)
        {
            SelectProfile(profile);
            search = string.Empty;
            Ping(profile);
            Repaint();
        }

        private void Preview(GamepadVibrationProfile profile)
        {
            StopPreview();
            previewHandle = profile != null ? profile.Play() : null;
        }

        private void Preview(GamepadVibrationSettings settings)
        {
            StopPreview();
            previewHandle = settings.Play();
        }

        private void StopPreview()
        {
            previewHandle?.Stop();
            previewHandle = null;
        }

        private static void DrawReadOnlySettings(GamepadVibrationSettings settings)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Slider("Low Frequency", settings.LowFrequency, 0f, 1f);
                EditorGUILayout.Slider("High Frequency", settings.HighFrequency, 0f, 1f);
                EditorGUILayout.FloatField("Duration", settings.Duration);
                EditorGUILayout.IntField("Pulses", settings.Pulses);
                if (settings.Pulses > 1) EditorGUILayout.FloatField("Interval", settings.Interval);
                EditorGUILayout.Toggle("Fade Out", settings.FadeOut);
                EditorGUILayout.Slider("Global Intensity", settings.GlobalIntensity, 0f, 1f);
            }
        }

        private static void Ping(UnityEngine.Object target)
        {
            if (target == null) return;
            Selection.activeObject = target;
            EditorGUIUtility.PingObject(target);
        }

        private static void Copy(string value)
        {
            EditorGUIUtility.systemCopyBuffer = value;
            Debug.Log("Copied: " + value);
        }

        private static string ApiStatusText(GamepadVibrationProfileApiStatus status)
        {
            switch (status)
            {
                case GamepadVibrationProfileApiStatus.UpToDate: return "✓ API is up to date";
                case GamepadVibrationProfileApiStatus.OutOfDate: return "⚠ API is out of date";
                default: return "⚠ API has not been generated";
            }
        }

        private static void ShowCopyMenu(string identifier)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Single Player"), false, () => Copy($"GamepadVibrationCustom.{identifier}.Play();"));
            menu.AddItem(new GUIContent("PlayerInput"), false, () => Copy($"GamepadVibrationCustom.{identifier}.Play(playerInput);"));
            menu.AddItem(new GUIContent("Gamepad"), false, () => Copy($"GamepadVibrationCustom.{identifier}.Play(gamepad);"));
            menu.ShowAsContext();
        }

        private void BuildStyles()
        {
            if (titleStyle != null) return;
            titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            selectedRowStyle = new GUIStyle(EditorStyles.miniButton) { fontStyle = FontStyle.Bold };
            badgeStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleRight, fontSize = 8 };
            statusStyle = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
        }
    }
}
