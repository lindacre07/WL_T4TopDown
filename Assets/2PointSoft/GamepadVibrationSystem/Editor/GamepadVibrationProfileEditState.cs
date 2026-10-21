using System;
using UnityEditor;
using UnityEngine;

namespace TwoPointSoft.GamepadVibration.Editor
{
    /// <summary>Keeps profile form edits separate from the persisted asset until Save is pressed.</summary>
    public sealed class GamepadVibrationProfileEditState : IDisposable
    {
        private string savedSnapshot;

        public GamepadVibrationProfile WorkingProfile { get; private set; }
        public bool CanReset => WorkingProfile != null &&
                                !string.Equals(savedSnapshot, Capture(WorkingProfile), StringComparison.Ordinal);

        public GamepadVibrationProfileEditState(GamepadVibrationProfile source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            WorkingProfile = UnityEngine.Object.Instantiate(source);
            WorkingProfile.hideFlags = HideFlags.DontSave;
            WorkingProfile.name = source.name;
            savedSnapshot = Capture(WorkingProfile);
        }

        public void Reset()
        {
            if (WorkingProfile == null) return;
            EditorJsonUtility.FromJsonOverwrite(savedSnapshot, WorkingProfile);
        }

        public void SaveTo(GamepadVibrationProfile target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            EditorUtility.CopySerialized(WorkingProfile, target);
            savedSnapshot = Capture(WorkingProfile);
        }

        public void Dispose()
        {
            if (WorkingProfile == null) return;
            UnityEngine.Object.DestroyImmediate(WorkingProfile);
            WorkingProfile = null;
        }

        private static string Capture(GamepadVibrationProfile profile)
        {
            return EditorJsonUtility.ToJson(profile, false);
        }
    }
}
