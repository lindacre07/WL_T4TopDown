using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Combines active vibration requests independently for each target gamepad.</summary>
    [AddComponentMenu("")]
    public sealed class GamepadVibrationManager : MonoBehaviour
    {
        private const float StopConfirmationDuration = 0.12f;
        private const float StopRetryInterval = 0.02f;

        private struct StopConfirmationState
        {
            public float TimeRemaining;
            public float RetryDelay;

            public StopConfirmationState(float timeRemaining, float retryDelay)
            {
                TimeRemaining = timeRemaining;
                RetryDelay = retryDelay;
            }
        }

        private sealed class VibrationRequest
        {
            public readonly int Id;
            public readonly float Low;
            public readonly float High;
            public readonly float Duration;
            public readonly bool FadeOut;
            public readonly bool IsContinuous;
            public readonly Gamepad Target;
            public float Elapsed;

            public VibrationRequest(int id, float low, float high, float duration, bool fadeOut, bool isContinuous, Gamepad target)
            {
                Id = id;
                Low = Mathf.Clamp01(low);
                High = Mathf.Clamp01(high);
                Duration = Mathf.Max(0f, duration);
                FadeOut = fadeOut;
                IsContinuous = isContinuous;
                Target = target;
            }
        }

        private struct MotorOutput
        {
            public float Low;
            public float High;
        }

        private static GamepadVibrationManager instance;
        private static int nextRequestId = 1;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            instance = null;
            nextRequestId = 1;
        }

        private readonly List<VibrationRequest> activeRequests = new List<VibrationRequest>();
        private readonly List<GamepadVibrationHandle> activeHandles = new List<GamepadVibrationHandle>();
        private readonly HashSet<Gamepad> managedGamepads = new HashSet<Gamepad>();
        private readonly Dictionary<Gamepad, MotorOutput> outputs = new Dictionary<Gamepad, MotorOutput>();
        private readonly Dictionary<Gamepad, StopConfirmationState> stopConfirmations = new Dictionary<Gamepad, StopConfirmationState>();
        private bool isPrimaryInstance;
        private bool isQuitting;

        internal static int ActiveRequestCount => instance != null ? instance.activeRequests.Count : 0;

        internal static int GetActiveRequestCount(Gamepad target)
        {
            if (instance == null) return 0;
            int count = 0;
            foreach (VibrationRequest request in instance.activeRequests)
            {
                if (request.Target == target) count++;
            }
            return count;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            isPrimaryInstance = true;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            RemoveDisconnectedTargets();
            outputs.Clear();
            for (int i = activeRequests.Count - 1; i >= 0; i--)
            {
                VibrationRequest request = activeRequests[i];
                if (!request.IsContinuous)
                {
                    request.Elapsed += Time.unscaledDeltaTime;
                    if (request.Elapsed >= request.Duration)
                    {
                        activeRequests.RemoveAt(i);
                        continue;
                    }
                }

                if (request.Target == null) continue;
                float multiplier = !request.IsContinuous && request.FadeOut && request.Duration > 0f
                    ? Mathf.Clamp01(1f - request.Elapsed / request.Duration)
                    : 1f;
                outputs.TryGetValue(request.Target, out MotorOutput output);
                output.Low += request.Low * multiplier;
                output.High += request.High * multiplier;
                outputs[request.Target] = output;
            }

            ApplyOutputs();
        }

        /// <summary>Plays on Gamepad.current. A manager is created automatically.</summary>
        public static void Play(float low, float high, float duration, bool fadeOut = false)
        {
            Play(low, high, duration, Gamepad.current, fadeOut);
        }

        /// <summary>Plays a finite request on one specific gamepad.</summary>
        public static void Play(float low, float high, float duration, Gamepad target, bool fadeOut = false)
        {
            PlayOwned(low, high, duration, fadeOut, target);
        }

        /// <summary>Plays a built-in preset on Gamepad.current.</summary>
        public static GamepadVibrationHandle PlayPreset(GamepadVibrationPreset preset)
        {
            return Play(GamepadVibrationPresets.Get(preset));
        }

        /// <summary>Plays a built-in preset on one specific gamepad.</summary>
        public static GamepadVibrationHandle PlayPreset(GamepadVibrationPreset preset, Gamepad target)
        {
            return Play(GamepadVibrationPresets.Get(preset), target);
        }

        /// <summary>Plays reusable settings on Gamepad.current.</summary>
        public static GamepadVibrationHandle Play(GamepadVibrationSettings settings)
        {
            return PlayInternal(settings, Gamepad.current, true);
        }

        /// <summary>Plays reusable settings on one specific gamepad.</summary>
        public static GamepadVibrationHandle Play(GamepadVibrationSettings settings, Gamepad target)
        {
            return PlayInternal(settings, target, false);
        }

        private static GamepadVibrationHandle PlayInternal(GamepadVibrationSettings settings, Gamepad target, bool allowMissingTarget)
        {
            if (target == null && !allowMissingTarget) return new GamepadVibrationHandle { IsStopped = true };
            EnsureInstance();
            GamepadVibrationHandle handle = new GamepadVibrationHandle { Target = target };
            instance.activeHandles.Add(handle);
            instance.RegisterGamepad(target);
            handle.Sequence = instance.StartCoroutine(instance.PlaySettingsSequence(settings, handle));
            return handle;
        }

        /// <summary>Plays reusable settings on the gamepad paired with a local player.</summary>
        public static GamepadVibrationHandle Play(GamepadVibrationSettings settings, PlayerInput playerInput)
        {
            return Play(settings, ResolveGamepad(playerInput));
        }

        /// <summary>Plays a preset on the gamepad paired with a local player.</summary>
        public static GamepadVibrationHandle PlayPreset(GamepadVibrationPreset preset, PlayerInput playerInput)
        {
            return Play(GamepadVibrationPresets.Get(preset), ResolveGamepad(playerInput));
        }

        /// <summary>Starts an indefinite vibration on one gamepad and returns an individually stoppable handle.</summary>
        public static GamepadVibrationHandle PlayContinuous(float low, float high, Gamepad target)
        {
            if (target == null) return new GamepadVibrationHandle { IsStopped = true };
            EnsureInstance();
            GamepadVibrationHandle handle = new GamepadVibrationHandle { Target = target };
            handle.RequestIds.Add(PlayContinuousOwned(low, high, target));
            instance.activeHandles.Add(handle);
            return handle;
        }

        internal static void Stop(GamepadVibrationHandle handle)
        {
            if (handle == null || handle.IsStopped) return;
            handle.IsStopped = true;
            if (instance != null && handle.Sequence != null) instance.StopCoroutine(handle.Sequence);
            handle.Sequence = null;
            if (instance != null) instance.activeHandles.Remove(handle);
            for (int i = handle.RequestIds.Count - 1; i >= 0; i--) Stop(handle.RequestIds[i]);
            handle.RequestIds.Clear();
        }

        private IEnumerator PlaySettingsSequence(GamepadVibrationSettings settings, GamepadVibrationHandle handle)
        {
            float low = Mathf.Clamp01(settings.LowFrequency * settings.GlobalIntensity);
            float high = Mathf.Clamp01(settings.HighFrequency * settings.GlobalIntensity);
            for (int i = 0; i < settings.Pulses && !handle.IsStopped; i++)
            {
                int requestId = PlayOwned(low, high, settings.Duration, settings.FadeOut, handle.Target);
                handle.RequestIds.Add(requestId);
                if (settings.Duration > 0f) yield return new WaitForSecondsRealtime(settings.Duration);
                Stop(requestId);
                handle.RequestIds.Remove(requestId);
                if (i < settings.Pulses - 1 && settings.Interval > 0f)
                    yield return new WaitForSecondsRealtime(settings.Interval);
            }

            handle.Sequence = null;
            activeHandles.Remove(handle);
        }

        internal static int PlayOwned(float low, float high, float duration, bool fadeOut)
        {
            return PlayOwned(low, high, duration, fadeOut, Gamepad.current);
        }

        internal static int PlayOwned(float low, float high, float duration, bool fadeOut, Gamepad target)
        {
            EnsureInstance();
            int id = nextRequestId++;
            instance.RegisterGamepad(target);
            instance.activeRequests.Add(new VibrationRequest(id, low, high, duration, fadeOut, false, target));
            return id;
        }

        internal static int PlayContinuousOwned(float low, float high)
        {
            return PlayContinuousOwned(low, high, Gamepad.current);
        }

        internal static int PlayContinuousOwned(float low, float high, Gamepad target)
        {
            EnsureInstance();
            int id = nextRequestId++;
            instance.RegisterGamepad(target);
            instance.activeRequests.Add(new VibrationRequest(id, low, high, 0f, false, true, target));
            return id;
        }

        internal static void Stop(int requestId)
        {
            if (instance == null || requestId <= 0) return;
            for (int i = instance.activeRequests.Count - 1; i >= 0; i--)
            {
                if (instance.activeRequests[i].Id != requestId) continue;
                Gamepad target = instance.activeRequests[i].Target;
                instance.activeRequests.RemoveAt(i);
                instance.ApplyCurrentMotorOutput(target);
                break;
            }
        }

        /// <summary>Stops all managed requests and all managed gamepads.</summary>
        public static void StopAll()
        {
            if (instance == null)
            {
                StopGamepad(Gamepad.current);
                return;
            }

            for (int i = instance.activeHandles.Count - 1; i >= 0; i--) Stop(instance.activeHandles[i]);
            instance.activeHandles.Clear();
            instance.activeRequests.Clear();
            instance.BeginStopConfirmationForManagedGamepads();
        }

        /// <summary>Stops every request targeting one gamepad without affecting other gamepads.</summary>
        public static void StopAll(Gamepad target)
        {
            if (target == null || instance == null)
            {
                StopGamepad(target);
                return;
            }

            for (int i = instance.activeHandles.Count - 1; i >= 0; i--)
            {
                if (instance.activeHandles[i].Target == target) Stop(instance.activeHandles[i]);
            }
            for (int i = instance.activeRequests.Count - 1; i >= 0; i--)
            {
                if (instance.activeRequests[i].Target == target) instance.activeRequests.RemoveAt(i);
            }
            instance.BeginStopConfirmation(target);
        }

        /// <summary>Stops every request belonging to the gamepad paired with a local player.</summary>
        public static void StopAll(PlayerInput playerInput)
        {
            StopAll(ResolveGamepad(playerInput));
        }

        /// <summary>Returns the first gamepad paired with this PlayerInput, or null.</summary>
        public static Gamepad ResolveGamepad(PlayerInput playerInput)
        {
            if (playerInput == null) return null;
            foreach (InputDevice device in playerInput.devices)
            {
                if (device is Gamepad gamepad) return gamepad;
            }
            return null;
        }

        private static void EnsureInstance()
        {
            if (instance != null) return;
            GameObject managerObject = new GameObject("GamepadVibrationManager");
            instance = managerObject.AddComponent<GamepadVibrationManager>();
        }

        private void RemoveDisconnectedTargets()
        {
            if (managedGamepads.Count == 0) return;
            List<Gamepad> disconnected = null;
            foreach (Gamepad gamepad in managedGamepads)
            {
                if (gamepad != null && gamepad.added) continue;
                if (disconnected == null) disconnected = new List<Gamepad>();
                disconnected.Add(gamepad);
            }
            if (disconnected == null) return;

            foreach (Gamepad gamepad in disconnected)
            {
                for (int i = activeHandles.Count - 1; i >= 0; i--)
                {
                    if (activeHandles[i].Target == gamepad) Stop(activeHandles[i]);
                }
                for (int i = activeRequests.Count - 1; i >= 0; i--)
                {
                    if (activeRequests[i].Target == gamepad) activeRequests.RemoveAt(i);
                }
                managedGamepads.Remove(gamepad);
                stopConfirmations.Remove(gamepad);
            }
        }

        private void RegisterGamepad(Gamepad target)
        {
            if (target == null) return;
            managedGamepads.Add(target);
            stopConfirmations.Remove(target);
        }

        private void ApplyOutputs()
        {
            List<Gamepad> idleGamepads = null;
            foreach (Gamepad gamepad in managedGamepads)
            {
                if (outputs.TryGetValue(gamepad, out MotorOutput output))
                {
                    stopConfirmations.Remove(gamepad);
                    SetMotorSpeeds(gamepad, Mathf.Clamp01(output.Low), Mathf.Clamp01(output.High));
                    continue;
                }

                if (!stopConfirmations.TryGetValue(gamepad, out StopConfirmationState confirmation))
                {
                    BeginStopConfirmation(gamepad);
                    continue;
                }

                confirmation.TimeRemaining -= Time.unscaledDeltaTime;
                confirmation.RetryDelay -= Time.unscaledDeltaTime;
                if (confirmation.RetryDelay <= 0f || confirmation.TimeRemaining <= 0f)
                {
                    StopGamepad(gamepad);
                    confirmation.RetryDelay = StopRetryInterval;
                }

                if (confirmation.TimeRemaining > 0f)
                {
                    stopConfirmations[gamepad] = confirmation;
                    continue;
                }

                stopConfirmations.Remove(gamepad);
                if (idleGamepads == null) idleGamepads = new List<Gamepad>();
                idleGamepads.Add(gamepad);
            }

            if (idleGamepads == null) return;
            foreach (Gamepad gamepad in idleGamepads) managedGamepads.Remove(gamepad);
        }

        private void ApplyCurrentMotorOutput(Gamepad target)
        {
            if (target == null) return;
            float low = 0f;
            float high = 0f;
            bool hasRequest = false;
            foreach (VibrationRequest request in activeRequests)
            {
                if (request.Target != target) continue;
                hasRequest = true;
                float multiplier = !request.IsContinuous && request.FadeOut && request.Duration > 0f
                    ? Mathf.Clamp01(1f - request.Elapsed / request.Duration)
                    : 1f;
                low += request.Low * multiplier;
                high += request.High * multiplier;
            }
            if (hasRequest)
            {
                stopConfirmations.Remove(target);
                SetMotorSpeeds(target, Mathf.Clamp01(low), Mathf.Clamp01(high));
                return;
            }

            BeginStopConfirmation(target);
        }

        private void BeginStopConfirmation(Gamepad target)
        {
            if (target == null) return;
            managedGamepads.Add(target);
            StopGamepad(target);
            stopConfirmations[target] = new StopConfirmationState(
                StopConfirmationDuration,
                StopRetryInterval);
        }

        private void BeginStopConfirmationForManagedGamepads()
        {
            if (managedGamepads.Count == 0) return;
            foreach (Gamepad gamepad in managedGamepads) BeginStopConfirmation(gamepad);
        }

        private void StopManagedGamepads()
        {
            foreach (Gamepad gamepad in managedGamepads) StopGamepad(gamepad);
            managedGamepads.Clear();
            stopConfirmations.Clear();
        }

        private static void SetMotorSpeeds(Gamepad gamepad, float low, float high)
        {
            if (gamepad != null && gamepad.added) gamepad.SetMotorSpeeds(low, high);
        }

        private static void StopGamepad(Gamepad gamepad)
        {
            SetMotorSpeeds(gamepad, 0f, 0f);
        }

        private void OnApplicationQuit()
        {
            if (!isPrimaryInstance) return;
            isQuitting = true;
            activeRequests.Clear();
            StopManagedGamepads();
        }

        private void OnDestroy()
        {
            if (!isPrimaryInstance || instance != this) return;
            for (int i = activeHandles.Count - 1; i >= 0; i--)
            {
                GamepadVibrationHandle handle = activeHandles[i];
                if (handle.Sequence != null) StopCoroutine(handle.Sequence);
                handle.IsStopped = true;
                handle.Sequence = null;
                handle.RequestIds.Clear();
            }
            activeHandles.Clear();
            activeRequests.Clear();
            StopManagedGamepads();
            instance = null;
            if (!isQuitting) isPrimaryInstance = false;
        }
    }
}
