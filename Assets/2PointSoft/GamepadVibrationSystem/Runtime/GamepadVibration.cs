using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration
{
    [AddComponentMenu("2 Point Soft/Gamepad Vibration")]
    public sealed class GamepadVibration : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private GamepadVibrationSource source = GamepadVibrationSource.BuiltInOrCustomValues;
        [SerializeField] private GamepadVibrationProfile profile;

        [Header("Preset")]
        [SerializeField, Tooltip("Choose a starting vibration pattern or Custom values.")]
        private GamepadVibrationPreset preset = GamepadVibrationPreset.Custom;

        [Header("Playback")]
        [SerializeField, Tooltip("One Shot plays once, Pulsed repeats, and Continuous plays until Stop is called.")]
        private GamepadVibrationPlaybackMode playbackMode = GamepadVibrationPlaybackMode.Pulsed;

        [Header("Gamepad Target")]
        [SerializeField, Tooltip("Current Gamepad keeps single-player behavior. Player Input targets the gamepad paired with a local player.")]
        private GamepadVibrationTargetMode targetMode = GamepadVibrationTargetMode.CurrentGamepad;
        [SerializeField, Tooltip("Optional PlayerInput. If empty, the component searches its parents.")]
        private PlayerInput playerInput;

        [Header("Vibration Settings")]
        [SerializeField, Range(0f, 1f), Tooltip("Intensity of the low-frequency motor.")]
        private float lowFrequency = 0.5f;
        [SerializeField, Range(0f, 1f), Tooltip("Intensity of the high-frequency motor.")]
        private float highFrequency = 0.5f;
        [SerializeField, Min(0f), Tooltip("Length of each pulse in seconds.")]
        private float duration = 0.25f;

        [Header("Pulses")]
        [SerializeField, Min(1), Tooltip("Number of vibration pulses.")]
        private int pulses = 1;
        [SerializeField, Min(0f), Tooltip("Pause in seconds between pulses.")]
        private float interval = 0.05f;

        [Header("Behavior")]
        [SerializeField, Tooltip("Gradually reduces motor intensity during each pulse.")]
        private bool fadeOut;

        [Header("Global Intensity")]
        [SerializeField, Range(0f, 1f), Tooltip("Multiplier applied to both motor intensities.")]
        private float globalIntensity = 1f;

        private readonly List<int> ownedRequestIds = new List<int>();
        private Coroutine pulseSequence;
        private GamepadVibrationHandle profileHandle;

        public GamepadVibrationSource Source { get => source; set => source = value; }
        public GamepadVibrationProfile Profile { get => profile; set => profile = value; }
        public GamepadVibrationPreset Preset => preset;
        public GamepadVibrationPlaybackMode PlaybackMode { get => playbackMode; set => playbackMode = value; }
        public GamepadVibrationTargetMode TargetMode { get => targetMode; set => targetMode = value; }
        public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }
        public float LowFrequency { get => lowFrequency; set => lowFrequency = Mathf.Clamp01(value); }
        public float HighFrequency { get => highFrequency; set => highFrequency = Mathf.Clamp01(value); }
        public float Duration { get => duration; set => duration = Mathf.Max(0f, value); }
        public int Pulses { get => pulses; set => pulses = Mathf.Max(1, value); }
        public float Interval { get => interval; set => interval = Mathf.Max(0f, value); }
        public bool FadeOut { get => fadeOut; set => fadeOut = value; }
        public float GlobalIntensity { get => globalIntensity; set => globalIntensity = Mathf.Clamp01(value); }

        /// <summary>Plays this component's configured vibration sequence.</summary>
        public void Play()
        {
            Stop();
            ValidateValues();

            Gamepad target = ResolveTargetGamepad();
            if (targetMode == GamepadVibrationTargetMode.PlayerInput && target == null) return;

            if (source == GamepadVibrationSource.VibrationProfile)
            {
                if (profile == null) return;
                profileHandle = targetMode == GamepadVibrationTargetMode.CurrentGamepad && target == null
                    ? profile.Play()
                    : profile.Play(target);
                return;
            }

            if (playbackMode == GamepadVibrationPlaybackMode.Continuous)
            {
                float low = Mathf.Clamp01(lowFrequency * globalIntensity);
                float high = Mathf.Clamp01(highFrequency * globalIntensity);
                ownedRequestIds.Add(GamepadVibrationManager.PlayContinuousOwned(low, high, target));
                return;
            }

            pulseSequence = StartCoroutine(PlayPulseSequence(target));
        }

        /// <summary>Stops only vibration requests started by this component.</summary>
        public void Stop()
        {
            profileHandle?.Stop();
            profileHandle = null;

            if (pulseSequence != null)
            {
                StopCoroutine(pulseSequence);
                pulseSequence = null;
            }

            for (int i = ownedRequestIds.Count - 1; i >= 0; i--)
            {
                GamepadVibrationManager.Stop(ownedRequestIds[i]);
            }

            ownedRequestIds.Clear();
        }

        public void ApplyPreset(GamepadVibrationPreset newPreset)
        {
            preset = newPreset;
            if (newPreset == GamepadVibrationPreset.Custom)
            {
                ValidateValues();
                return;
            }

            GamepadVibrationSettings settings = GamepadVibrationPresets.Get(newPreset);
            playbackMode = settings.Pulses > 1
                ? GamepadVibrationPlaybackMode.Pulsed
                : GamepadVibrationPlaybackMode.OneShot;
            lowFrequency = settings.LowFrequency;
            highFrequency = settings.HighFrequency;
            duration = settings.Duration;
            pulses = settings.Pulses;
            interval = settings.Interval;
            fadeOut = settings.FadeOut;
            globalIntensity = settings.GlobalIntensity;
            ValidateValues();
        }

        internal void SetPresetToCustom()
        {
            preset = GamepadVibrationPreset.Custom;
        }

        private IEnumerator PlayPulseSequence(Gamepad target)
        {
            float low = Mathf.Clamp01(lowFrequency * globalIntensity);
            float high = Mathf.Clamp01(highFrequency * globalIntensity);

            int pulseCount = playbackMode == GamepadVibrationPlaybackMode.OneShot ? 1 : pulses;
            for (int i = 0; i < pulseCount; i++)
            {
                int requestId = GamepadVibrationManager.PlayOwned(low, high, duration, fadeOut, target);
                ownedRequestIds.Add(requestId);

                if (duration > 0f) yield return new WaitForSecondsRealtime(duration);

                // Remove this exact request even if manager/coroutine update ordering differs.
                GamepadVibrationManager.Stop(requestId);
                ownedRequestIds.Remove(requestId);

                if (i < pulseCount - 1 && interval > 0f)
                {
                    yield return new WaitForSecondsRealtime(interval);
                }
            }

            pulseSequence = null;
        }

        private Gamepad ResolveTargetGamepad()
        {
            if (targetMode == GamepadVibrationTargetMode.CurrentGamepad) return Gamepad.current;
            PlayerInput source = playerInput != null ? playerInput : GetComponentInParent<PlayerInput>();
            return GamepadVibrationManager.ResolveGamepad(source);
        }

        private void OnValidate()
        {
            ValidateValues();
        }

        private void OnDisable()
        {
            Stop();
        }

        private void OnDestroy()
        {
            Stop();
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
