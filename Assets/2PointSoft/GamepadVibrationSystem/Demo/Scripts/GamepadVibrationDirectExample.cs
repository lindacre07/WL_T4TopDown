using UnityEngine;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Demonstrates component-free preset, override, custom, and legacy C# APIs.</summary>
    [AddComponentMenu("2 Point Soft/Demo/Gamepad Vibration Direct API Example")]
    public sealed class GamepadVibrationDirectExample : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float lowFrequency = 0.45f;
        [SerializeField, Range(0f, 1f)] private float highFrequency = 0.75f;
        [SerializeField, Min(0f)] private float duration = 0.2f;
        [SerializeField] private bool fadeOut = true;

        // Simplest usage: built-in defaults, no parameters and no configured component.
        public void PlayAlertDefault()
        {
            GamepadVibrationPresets.Alert.Play();
        }

        // Starts from a default and changes only values needed for this call.
        public void PlayModifiedExplosion()
        {
            GamepadVibrationPresets.Explosion
                .With(highFrequency: 1f, duration: 0.35f)
                .Play();
        }

        // Creates a reusable configuration entirely in code.
        public void PlayCustomSwordHit()
        {
            GamepadVibrationSettings swordHit = new GamepadVibrationSettings(
                lowFrequency: 0.55f,
                highFrequency: 0.90f,
                duration: 0.16f,
                pulses: 1,
                fadeOut: true
            );
            swordHit.Play();
        }

        // Conventional enum-based alternative.
        public void PlayAlertByEnum()
        {
            GamepadVibrationManager.PlayPreset(GamepadVibrationPreset.Alert);
        }

        // Original low-level API remains available for complete control.
        public void PlayDirect()
        {
            GamepadVibrationManager.Play(
                Mathf.Clamp01(lowFrequency),
                Mathf.Clamp01(highFrequency),
                Mathf.Max(0f, duration),
                fadeOut
            );
        }

        public void StopAll()
        {
            GamepadVibrationManager.StopAll();
        }
    }
}
