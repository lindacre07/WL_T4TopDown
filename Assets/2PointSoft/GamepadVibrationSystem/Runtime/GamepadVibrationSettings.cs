using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Reusable validated values for a one-shot or pulsed vibration sequence.</summary>
    public readonly struct GamepadVibrationSettings
    {
        public float LowFrequency { get; }
        public float HighFrequency { get; }
        public float Duration { get; }
        public int Pulses { get; }
        public float Interval { get; }
        public bool FadeOut { get; }
        public float GlobalIntensity { get; }

        public GamepadVibrationSettings(
            float lowFrequency,
            float highFrequency,
            float duration,
            int pulses = 1,
            float interval = 0f,
            bool fadeOut = false,
            float globalIntensity = 1f)
        {
            LowFrequency = Mathf.Clamp01(lowFrequency);
            HighFrequency = Mathf.Clamp01(highFrequency);
            Duration = Mathf.Max(0f, duration);
            Pulses = Mathf.Max(1, pulses);
            Interval = Mathf.Max(0f, interval);
            FadeOut = fadeOut;
            GlobalIntensity = Mathf.Clamp01(globalIntensity);
        }

        public GamepadVibrationSettings With(
            float? lowFrequency = null,
            float? highFrequency = null,
            float? duration = null,
            int? pulses = null,
            float? interval = null,
            bool? fadeOut = null,
            float? globalIntensity = null)
        {
            return new GamepadVibrationSettings(
                lowFrequency ?? LowFrequency,
                highFrequency ?? HighFrequency,
                duration ?? Duration,
                pulses ?? Pulses,
                interval ?? Interval,
                fadeOut ?? FadeOut,
                globalIntensity ?? GlobalIntensity
            );
        }

        /// <summary>Plays these settings without requiring a scene component.</summary>
        public GamepadVibrationHandle Play()
        {
            return GamepadVibrationManager.Play(this);
        }

        /// <summary>Plays these settings on one specific gamepad.</summary>
        public GamepadVibrationHandle Play(Gamepad gamepad)
        {
            return GamepadVibrationManager.Play(this, gamepad);
        }

        /// <summary>Plays these settings on the gamepad paired with a local player.</summary>
        public GamepadVibrationHandle Play(PlayerInput playerInput)
        {
            return GamepadVibrationManager.Play(this, playerInput);
        }
    }
}
