using UnityEditor;
using UnityEngine;

namespace TwoPointSoft.GamepadVibration.Editor
{
    [CustomEditor(typeof(GamepadVibration))]
    [CanEditMultipleObjects]
    public sealed class GamepadVibrationEditor : UnityEditor.Editor
    {
        private SerializedProperty preset;
        private SerializedProperty source;
        private SerializedProperty profile;
        private SerializedProperty playbackMode;
        private SerializedProperty targetMode;
        private SerializedProperty playerInput;
        private SerializedProperty lowFrequency;
        private SerializedProperty highFrequency;
        private SerializedProperty duration;
        private SerializedProperty pulses;
        private SerializedProperty interval;
        private SerializedProperty fadeOut;
        private SerializedProperty globalIntensity;

        private void OnEnable()
        {
            preset = serializedObject.FindProperty("preset");
            source = serializedObject.FindProperty("source");
            profile = serializedObject.FindProperty("profile");
            playbackMode = serializedObject.FindProperty("playbackMode");
            targetMode = serializedObject.FindProperty("targetMode");
            playerInput = serializedObject.FindProperty("playerInput");
            lowFrequency = serializedObject.FindProperty("lowFrequency");
            highFrequency = serializedObject.FindProperty("highFrequency");
            duration = serializedObject.FindProperty("duration");
            pulses = serializedObject.FindProperty("pulses");
            interval = serializedObject.FindProperty("interval");
            fadeOut = serializedObject.FindProperty("fadeOut");
            globalIntensity = serializedObject.FindProperty("globalIntensity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("GAMEPAD VIBRATION SYSTEM", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("by 2 Point Soft", EditorStyles.miniLabel);
            EditorGUILayout.Space(6f);
            EditorGUILayout.PropertyField(source);
            GamepadVibrationSource selectedSource = (GamepadVibrationSource)source.intValue;
            if (selectedSource == GamepadVibrationSource.VibrationProfile)
            {
                EditorGUILayout.PropertyField(profile);
                if (profile.objectReferenceValue == null)
                    EditorGUILayout.HelpBox("Assign a reusable vibration profile.", MessageType.Warning);
                if (GUILayout.Button("OPEN PROFILE MANAGER")) GamepadVibrationProfileManager.Open();
                DrawTargetSection();
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(preset);
            bool presetChanged = EditorGUI.EndChangeCheck();

            if (presetChanged)
            {
                serializedObject.ApplyModifiedProperties();
                ApplySelectedPreset();
                serializedObject.Update();
            }

            EditorGUILayout.Space(6f);
            DrawTargetSection();
            DrawSection("PLAYBACK", playbackMode);

            GamepadVibrationPlaybackMode mode = (GamepadVibrationPlaybackMode)playbackMode.intValue;
            if (mode == GamepadVibrationPlaybackMode.Continuous)
            {
                DrawSection("VIBRATION SETTINGS", lowFrequency, highFrequency);
            }
            else
            {
                DrawSection("VIBRATION SETTINGS", lowFrequency, highFrequency, duration);
                if (mode == GamepadVibrationPlaybackMode.Pulsed)
                {
                    DrawSection("PULSES", pulses, interval);
                }
                DrawSection("BEHAVIOR", fadeOut);
            }

            DrawSection("GLOBAL INTENSITY", globalIntensity);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTargetSection()
        {
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField("GAMEPAD TARGET", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(targetMode);

            GamepadVibrationTargetMode mode = (GamepadVibrationTargetMode)targetMode.intValue;
            if (mode == GamepadVibrationTargetMode.PlayerInput)
            {
                EditorGUILayout.PropertyField(playerInput);
                if (playerInput.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox(
                        "No reference assigned. The component will use the first PlayerInput found on this object or a parent.",
                        MessageType.Info);
                }
            }
        }

        private void DrawSection(string title, params SerializedProperty[] properties)
        {
            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            foreach (SerializedProperty property in properties)
            {
                EditorGUILayout.PropertyField(property);
            }

            if (EditorGUI.EndChangeCheck() && preset.enumValueIndex != (int)GamepadVibrationPreset.Custom)
            {
                preset.enumValueIndex = (int)GamepadVibrationPreset.Custom;
            }
        }

        private void ApplySelectedPreset()
        {
            GamepadVibrationPreset selectedPreset = (GamepadVibrationPreset)preset.enumValueIndex;
            foreach (Object selectedTarget in targets)
            {
                GamepadVibration vibration = selectedTarget as GamepadVibration;
                if (vibration == null) continue;

                Undo.RecordObject(vibration, "Change Gamepad Vibration Preset");
                vibration.ApplyPreset(selectedPreset);
                EditorUtility.SetDirty(vibration);
            }
        }
    }
}
