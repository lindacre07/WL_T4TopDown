using System.Collections.Generic;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Represents one script-started vibration sequence that can be stopped independently.</summary>
    public sealed class GamepadVibrationHandle
    {
        internal readonly List<int> RequestIds = new List<int>();
        internal UnityEngine.Coroutine Sequence;
        internal UnityEngine.InputSystem.Gamepad Target;
        internal bool IsStopped;

        public bool IsPlaying => !IsStopped && (Sequence != null || RequestIds.Count > 0);

        public void Stop()
        {
            GamepadVibrationManager.Stop(this);
        }
    }
}
