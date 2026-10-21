using UnityEditor;
using UnityEngine;

namespace TwoPointSoft.GamepadVibration.Editor
{
    [CustomEditor(typeof(GamepadVibrationProfile))]
    public sealed class GamepadVibrationProfileEditor : UnityEditor.Editor
    {
        private SerializedProperty description;
        private SerializedProperty playbackMode;
        private SerializedProperty lowFrequency;
        private SerializedProperty highFrequency;
        private SerializedProperty duration;
        private SerializedProperty pulses;
        private SerializedProperty interval;
        private SerializedProperty fadeOut;
        private SerializedProperty globalIntensity;
        private SerializedProperty codeName;
        private GamepadVibrationHandle previewHandle;

        private void OnEnable()
        {
            description = serializedObject.FindProperty("description");
            playbackMode = serializedObject.FindProperty("playbackMode");
            lowFrequency = serializedObject.FindProperty("lowFrequency");
            highFrequency = serializedObject.FindProperty("highFrequency");
            duration = serializedObject.FindProperty("duration");
            pulses = serializedObject.FindProperty("pulses");
            interval = serializedObject.FindProperty("interval");
            fadeOut = serializedObject.FindProperty("fadeOut");
            globalIntensity = serializedObject.FindProperty("globalIntensity");
            codeName = serializedObject.FindProperty("codeName");
        }

        private void OnDisable()
        {
            previewHandle?.Stop();
            previewHandle = null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("GAMEPAD VIBRATION PROFILE", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Reusable custom vibration", EditorStyles.miniLabel);
            EditorGUILayout.Space(5f);
            EditorGUILayout.PropertyField(description);
            EditorGUILayout.PropertyField(codeName);
            if (string.IsNullOrWhiteSpace(codeName.stringValue))
                EditorGUILayout.HelpBox("The generated API will derive a code name from the asset name.", MessageType.Info);

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField("PLAYBACK", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playbackMode);
            EditorGUILayout.LabelField("VIBRATION SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(lowFrequency);
            EditorGUILayout.PropertyField(highFrequency);

            GamepadVibrationPlaybackMode mode = (GamepadVibrationPlaybackMode)playbackMode.intValue;
            if (mode != GamepadVibrationPlaybackMode.Continuous)
            {
                EditorGUILayout.PropertyField(duration);
                if (mode == GamepadVibrationPlaybackMode.Pulsed)
                {
                    EditorGUILayout.PropertyField(pulses);
                    EditorGUILayout.PropertyField(interval);
                }
                EditorGUILayout.PropertyField(fadeOut);
            }
            EditorGUILayout.PropertyField(globalIntensity);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(8f);
            EditorGUILayout.HelpBox(
                "Vibration preview is available in Play Mode only. Enter Play Mode to use TEST PROFILE and STOP TEST.",
                MessageType.Info);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUI.enabled = Application.isPlaying;
                if (GUILayout.Button("TEST PROFILE"))
                {
                    previewHandle?.Stop();
                    previewHandle = ((GamepadVibrationProfile)target).Play();
                }
                if (GUILayout.Button("STOP TEST"))
                {
                    previewHandle?.Stop();
                    previewHandle = null;
                }
                GUI.enabled = true;
            }
            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Enter Play Mode to test physical vibration safely.", MessageType.Info);

            if (GUILayout.Button("UPDATE GENERATED CODE API"))
            {
                string path = GamepadVibrationProfileCodeGenerator.GenerateAll();
                Debug.Log("Updated custom vibration API: " + path);
            }
            if (GUILayout.Button("OPEN PROFILE MANAGER")) GamepadVibrationProfileManager.Open();
        }
    }
}
