using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Runtime representation used by the optional generated custom-profile API.</summary>
    public readonly struct GamepadVibrationGeneratedProfile
    {
        public GamepadVibrationPlaybackMode PlaybackMode { get; }
        public GamepadVibrationSettings Settings { get; }

        public GamepadVibrationGeneratedProfile(GamepadVibrationPlaybackMode playbackMode, GamepadVibrationSettings settings)
        {
            PlaybackMode = playbackMode;
            Settings = settings;
        }

        public GamepadVibrationHandle Play()
        {
            if (PlaybackMode != GamepadVibrationPlaybackMode.Continuous) return GetFiniteSettings().Play();
            Gamepad gamepad = Gamepad.current;
            return gamepad == null
                ? new GamepadVibrationHandle()
                : GamepadVibrationManager.PlayContinuous(
                    Mathf.Clamp01(Settings.LowFrequency * Settings.GlobalIntensity),
                    Mathf.Clamp01(Settings.HighFrequency * Settings.GlobalIntensity),
                    gamepad);
        }

        public GamepadVibrationHandle Play(Gamepad gamepad)
        {
            if (PlaybackMode != GamepadVibrationPlaybackMode.Continuous) return GetFiniteSettings().Play(gamepad);
            return gamepad == null
                ? new GamepadVibrationHandle()
                : GamepadVibrationManager.PlayContinuous(
                    Mathf.Clamp01(Settings.LowFrequency * Settings.GlobalIntensity),
                    Mathf.Clamp01(Settings.HighFrequency * Settings.GlobalIntensity),
                    gamepad);
        }

        public GamepadVibrationHandle Play(PlayerInput playerInput)
        {
            return Play(GamepadVibrationManager.ResolveGamepad(playerInput));
        }

        private GamepadVibrationSettings GetFiniteSettings()
        {
            return PlaybackMode == GamepadVibrationPlaybackMode.OneShot
                ? Settings.With(pulses: 1)
                : Settings;
        }
    }
}
