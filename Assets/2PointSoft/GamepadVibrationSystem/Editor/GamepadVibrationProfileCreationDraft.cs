using System;
using UnityEngine;

namespace TwoPointSoft.GamepadVibration.Editor
{
    /// <summary>Editable non-persistent profile used to configure all values before creating an asset.</summary>
    public sealed class GamepadVibrationProfileCreationDraft : IDisposable
    {
        public GamepadVibrationProfile Profile { get; private set; }

        public GamepadVibrationProfileCreationDraft()
        {
            Profile = ScriptableObject.CreateInstance<GamepadVibrationProfile>();
            Profile.hideFlags = HideFlags.DontSave;
            Profile.name = "New Vibration";
        }

        public void Initialize(GamepadVibrationProfileCreationData data)
        {
            if (Profile == null) return;
            Profile.Configure(data.PlaybackMode, data.Settings, data.Description);
        }

        public void Dispose()
        {
            if (Profile == null) return;
            UnityEngine.Object.DestroyImmediate(Profile);
            Profile = null;
        }
    }
}
