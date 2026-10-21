using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration.Demo
{
    /// <summary>Shows three explicit custom-profile use cases: attack, recoil, and continuous feedback.</summary>
    public sealed class GamepadVibrationProfilesDemo : MonoBehaviour
    {
        [Header("Example 1 - Sword Attack (One Shot)")]
        [SerializeField, Tooltip("Reusable one-shot profile used by the sword attack example.")]
        private GamepadVibrationProfile swordAttackProfile;

        [Header("Example 2 - Shotgun Recoil (Pulsed)")]
        [SerializeField, Tooltip("Reusable impact profile used by the shotgun recoil example.")]
        private GamepadVibrationProfile shotgunRecoilProfile;

        [Header("Example 3 - Engine Idle (Continuous)")]
        [SerializeField, Tooltip("Reusable continuous profile. Keep its returned handle to stop only this effect.")]
        private GamepadVibrationProfile engineIdleProfile;

        private GamepadVibrationHandle activeHandle;
        private GUIStyle titleStyle;
        private GUIStyle headingStyle;
        private GUIStyle textStyle;
        private GUIStyle codeStyle;
        private GUIStyle cardStyle;

        /// <summary>Compatibility view of the three explicit demo profile references.</summary>
        public GamepadVibrationProfile[] Profiles
        {
            get => new[] { swordAttackProfile, shotgunRecoilProfile, engineIdleProfile };
            set
            {
                swordAttackProfile = value != null && value.Length > 0 ? value[0] : null;
                shotgunRecoilProfile = value != null && value.Length > 1 ? value[1] : null;
                engineIdleProfile = value != null && value.Length > 2 ? value[2] : null;
            }
        }

        // One-shot and impact examples replace any previous demo-owned request.
        public void PlaySwordAttack() => PlayOneShotExample(swordAttackProfile);
        public void PlayShotgunRecoil() => PlayOneShotExample(shotgunRecoilProfile);

        // Continuous feedback keeps its handle so only Engine Idle is stopped.
        public void StartEngineIdle()
        {
            StopActiveProfile();
            activeHandle = engineIdleProfile != null ? engineIdleProfile.Play() : null;
        }

        public void StopActiveProfile()
        {
            activeHandle?.Stop();
            activeHandle = null;
        }

        private void PlayOneShotExample(GamepadVibrationProfile profile)
        {
            StopActiveProfile();
            activeHandle = profile != null ? profile.Play() : null;
        }

        // Runtime UI: three examples stay visible together instead of hiding behind a selection array.
        private void OnGUI()
        {
            BuildStyles();
            float width = Mathf.Min(Screen.width - 40f, 1100f);
            GUILayout.BeginArea(new Rect((Screen.width - width) * 0.5f, 18f, width, Screen.height - 36f));
            GUILayout.Label("CUSTOM VIBRATION PROFILES", titleStyle);
            GUILayout.Label("Three reusable profile assets • One explicit example for each common workflow", textStyle);
            GUILayout.Space(12f);

            using (new GUILayout.HorizontalScope())
            {
                DrawProfileCard(
                    "1 • SWORD ATTACK",
                    "ONE SHOT",
                    "A short reusable attack response. The gameplay script only calls profile.Play().",
                    swordAttackProfile,
                    "PLAY SWORD ATTACK",
                    PlaySwordAttack);
                GUILayout.Space(8f);
                DrawProfileCard(
                    "2 • SHOTGUN RECOIL",
                    "PULSED",
                    "Three short recoil pulses demonstrate pulse count and interval settings.",
                    shotgunRecoilProfile,
                    "PLAY SHOTGUN RECOIL",
                    PlayShotgunRecoil);
                GUILayout.Space(8f);
                DrawProfileCard(
                    "3 • ENGINE IDLE",
                    "CONTINUOUS",
                    "Continuous feedback returns a handle. Start it, then stop only this request.",
                    engineIdleProfile,
                    "START ENGINE IDLE",
                    StartEngineIdle,
                    true);
            }

            GUILayout.Space(12f);
            GUILayout.Label("HOW THESE EXAMPLES ARE ASSIGNED", headingStyle);
            GUILayout.Label(
                "Select ‘Custom Vibration Profiles Demo’ in the Hierarchy. The Inspector shows three named fields: Sword Attack, Shotgun Recoil, and Engine Idle.",
                textStyle);
            GUILayout.Space(8f);
            GUILayout.Label("DIRECT PROFILE ASSET API", headingStyle);
            GUILayout.Label(
                "swordAttackProfile.Play();\nshotgunRecoilProfile.Play(playerInput);\nGamepadVibrationHandle engineHandle = engineIdleProfile.Play();\nengineHandle.Stop();",
                codeStyle);
            GUILayout.Space(8f);
            GUILayout.Label("OPTIONAL GENERATED API", headingStyle);
            GUILayout.Label(
                "GamepadVibrationCustom.SwordAttack.Play();\nGamepadVibrationCustom.ShotgunRecoil.Play(playerInput);\nGamepadVibrationCustom.EngineIdle.Play();",
                codeStyle);
            GUILayout.Space(8f);
            GUILayout.Label(
                "Create or edit profiles: Tools > 2 Point Soft > Vibration Profile Manager",
                textStyle);
            GUILayout.Label(
                "Connected gamepad: " + (Gamepad.current != null ? Gamepad.current.displayName : "None"),
                textStyle);
            GUILayout.EndArea();
        }

        private void DrawProfileCard(
            string title,
            string category,
            string purpose,
            GamepadVibrationProfile profile,
            string playLabel,
            System.Action playAction,
            bool showStop = false)
        {
            using (new GUILayout.VerticalScope(cardStyle, GUILayout.ExpandWidth(true), GUILayout.MinHeight(255f)))
            {
                GUILayout.Label(title, headingStyle);
                GUILayout.Label(category, textStyle);
                GUILayout.Space(5f);
                GUILayout.Label(profile != null ? profile.name : "Missing profile reference", headingStyle);
                GUILayout.Label(purpose, textStyle, GUILayout.MinHeight(48f));
                if (profile != null)
                {
                    GamepadVibrationSettings settings = profile.Settings;
                    GUILayout.Label("Playback: " + profile.PlaybackMode, textStyle);
                    GUILayout.Label(
                        $"Low {settings.LowFrequency:0.00} • High {settings.HighFrequency:0.00}\nDuration {settings.Duration:0.00}s • Pulses {settings.Pulses}",
                        textStyle);
                }
                else
                {
                    GUILayout.Label("Assign this field on the demo component in the Inspector.", textStyle);
                }

                GUILayout.FlexibleSpace();
                GUI.enabled = profile != null;
                if (GUILayout.Button(playLabel, GUILayout.Height(42f))) playAction();
                GUI.enabled = true;
                if (showStop && GUILayout.Button("STOP ENGINE IDLE", GUILayout.Height(34f))) StopActiveProfile();
            }
        }

        // Presentation styles are built once and reused by the immediate-mode demo UI.
        private void BuildStyles()
        {
            if (titleStyle != null) return;
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            headingStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            textStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, alignment = TextAnchor.MiddleCenter, wordWrap = true };
            codeStyle = new GUIStyle(GUI.skin.box) { fontSize = 13, alignment = TextAnchor.MiddleLeft, wordWrap = true, padding = new RectOffset(14, 14, 8, 8) };
            cardStyle = new GUIStyle(GUI.skin.box) { padding = new RectOffset(12, 12, 12, 12) };
        }

        // Lifecycle ownership: leaving the scene never leaves a continuous motor request active.
        private void OnDisable()
        {
            StopActiveProfile();
        }
    }
}
