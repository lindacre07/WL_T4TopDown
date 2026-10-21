using UnityEngine;

namespace TwoPointSoft.GamepadVibration
{
    [AddComponentMenu("2 Point Soft/Gamepad Vibration Trigger")]
    public sealed class GamepadVibrationTrigger : MonoBehaviour
    {
        [SerializeField, Tooltip("Vibration component that will be played.")]
        private GamepadVibration vibration;
        [SerializeField, Tooltip("Choose whether entering or leaving the trigger plays vibration.")]
        private GamepadVibrationTriggerEvent triggerEvent = GamepadVibrationTriggerEvent.OnEnter;
        [SerializeField, Tooltip("Only objects with this tag can activate the trigger. Leave empty to accept any tag.")]
        private string requiredTag = "Player";
        [SerializeField, Tooltip("Prevents the trigger from activating more than once.")]
        private bool triggerOnce;
        [SerializeField, Tooltip("Stops the assigned vibration when the opposite trigger event occurs. Useful for Continuous playback.")]
        private bool stopOnOppositeEvent;

        private bool hasTriggered;

        public GamepadVibration Vibration
        {
            get => vibration;
            set => vibration = value;
        }

        public GamepadVibrationTriggerEvent TriggerEvent
        {
            get => triggerEvent;
            set => triggerEvent = value;
        }

        public string RequiredTag
        {
            get => requiredTag;
            set => requiredTag = value ?? string.Empty;
        }

        public bool TriggerOnce
        {
            get => triggerOnce;
            set => triggerOnce = value;
        }

        public bool StopOnOppositeEvent
        {
            get => stopOnOppositeEvent;
            set => stopOnOppositeEvent = value;
        }

        /// <summary>Allows this trigger to activate again when Trigger Once is enabled.</summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }

        private void Reset()
        {
            vibration = GetComponent<GamepadVibration>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!MatchesRequiredTag(other)) return;
            if (triggerEvent == GamepadVibrationTriggerEvent.OnEnter) TryPlay(other);
            else if (stopOnOppositeEvent && vibration != null) vibration.Stop();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!MatchesRequiredTag(other)) return;
            if (triggerEvent == GamepadVibrationTriggerEvent.OnExit) TryPlay(other);
            else if (stopOnOppositeEvent && vibration != null) vibration.Stop();
        }

        private void TryPlay(Collider other)
        {
            if (vibration == null || other == null || triggerOnce && hasTriggered) return;

            vibration.Play();
            hasTriggered = true;
        }

        private bool MatchesRequiredTag(Collider other)
        {
            if (other == null) return false;
            return string.IsNullOrWhiteSpace(requiredTag) || other.CompareTag(requiredTag);
        }
    }
}
