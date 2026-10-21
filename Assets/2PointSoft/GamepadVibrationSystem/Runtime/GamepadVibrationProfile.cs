using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>A reusable vibration configuration saved as a Unity project asset.</summary>
    [CreateAssetMenu(
        fileName = "New Gamepad Vibration Profile",
        menuName = "2 Point Soft/Gamepad Vibration Profile",
        order = 100)]
    public sealed class GamepadVibrationProfile : ScriptableObject
    {
        [SerializeField, TextArea(2, 4)] private string description;
        [SerializeField] private GamepadVibrationPlaybackMode playbackMode = GamepadVibrationPlaybackMode.OneShot;
        [SerializeField, Range(0f, 1f)] private float lowFrequency = 0.5f;
        [SerializeField, Range(0f, 1f)] private float highFrequency = 0.5f;
        [SerializeField, Min(0f)] private float duration = 0.25f;
        [SerializeField, Min(1)] private int pulses = 1;
        [SerializeField, Min(0f)] private float interval = 0.05f;
        [SerializeField] private bool fadeOut;
        [SerializeField, Range(0f, 1f)] private float globalIntensity = 1f;
        [SerializeField, Tooltip("Stable C# identifier used by the optional generated API.")]
        private string codeName;


        public string Description => description;
        public GamepadVibrationPlaybackMode PlaybackMode => playbackMode;
        public string CodeName => codeName;
        public GamepadVibrationSettings Settings => new GamepadVibrationSettings(
            lowFrequency, highFrequency, duration, pulses, interval, fadeOut, globalIntensity);

        /// <summary>Plays this profile on Gamepad.current.</summary>
        public GamepadVibrationHandle Play()
        {
            return PlayInternal(Gamepad.current, true);
        }

        /// <summary>Plays this profile on one specific gamepad.</summary>
        public GamepadVibrationHandle Play(Gamepad gamepad)
        {
            return PlayInternal(gamepad, false);
        }

        /// <summary>Plays this profile on the gamepad paired with a local player.</summary>
        public GamepadVibrationHandle Play(PlayerInput playerInput)
        {
            return Play(GamepadVibrationManager.ResolveGamepad(playerInput));
        }

        public void Configure(GamepadVibrationPlaybackMode mode, GamepadVibrationSettings settings, string newDescription = null, string newCodeName = null)
        {
            playbackMode = mode;
            lowFrequency = settings.LowFrequency;
            highFrequency = settings.HighFrequency;
            duration = settings.Duration;
            pulses = settings.Pulses;
            interval = settings.Interval;
            fadeOut = settings.FadeOut;
            globalIntensity = settings.GlobalIntensity;
            description = newDescription ?? description;
            codeName = newCodeName ?? codeName;
            ValidateValues();
        }

        private GamepadVibrationHandle PlayInternal(Gamepad gamepad, bool allowMissingCurrent)
        {
            if (playbackMode == GamepadVibrationPlaybackMode.Continuous)
            {
                if (gamepad == null) return new GamepadVibrationHandle();
                return GamepadVibrationManager.PlayContinuous(
                    Mathf.Clamp01(lowFrequency * globalIntensity),
                    Mathf.Clamp01(highFrequency * globalIntensity),
                    gamepad);
            }

            GamepadVibrationSettings effectiveSettings = playbackMode == GamepadVibrationPlaybackMode.OneShot
                ? Settings.With(pulses: 1)
                : Settings;
            return allowMissingCurrent
                ? GamepadVibrationManager.Play(effectiveSettings)
                : GamepadVibrationManager.Play(effectiveSettings, gamepad);
        }

        private void OnValidate()
        {
            ValidateValues();
        }

        private void ValidateValues()
        {
            lowFrequency = Mathf.Clamp01(lowFrequency);
            highFrequency = Mathf.Clamp01(highFrequency);
            duration = Mathf.Max(0f, duration);
            pulses = Mathf.Max(1, pulses);
            interval = Mathf.Max(0f, interval);
            globalIntensity = Mathf.Clamp01(globalIntensity);
        }
    }
}
