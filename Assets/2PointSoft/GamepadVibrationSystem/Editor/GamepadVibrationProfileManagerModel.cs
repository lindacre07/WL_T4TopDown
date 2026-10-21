using System;
using System.Collections.Generic;
using System.Linq;

namespace TwoPointSoft.GamepadVibration.Editor
{
    public enum GamepadVibrationProfileCreationMode
    {
        FromScratch,
        CopyBuiltIn,
        CopyExisting
    }

    public readonly struct GamepadVibrationProfileCreationData
    {
        public GamepadVibrationPlaybackMode PlaybackMode { get; }
        public GamepadVibrationSettings Settings { get; }
        public string Description { get; }

        public GamepadVibrationProfileCreationData(
            GamepadVibrationPlaybackMode playbackMode,
            GamepadVibrationSettings settings,
            string description)
        {
            PlaybackMode = playbackMode;
            Settings = settings;
            Description = description;
        }
    }

    public enum GamepadVibrationProfileApiStatus
    {
        NotGenerated,
        UpToDate,
        OutOfDate
    }

    /// <summary>Pure profile-library operations used by the Profile Manager window.</summary>
    public static class GamepadVibrationProfileManagerModel
    {
        private static readonly GamepadVibrationPreset[] BuiltIns =
        {
            GamepadVibrationPreset.LightHit,
            GamepadVibrationPreset.HeavyHit,
            GamepadVibrationPreset.Explosion,
            GamepadVibrationPreset.Heartbeat,
            GamepadVibrationPreset.Alert,
            GamepadVibrationPreset.Hazard,
            GamepadVibrationPreset.UISelect,
            GamepadVibrationPreset.UIConfirm
        };

        public static IReadOnlyList<GamepadVibrationPreset> BuiltInPresets => BuiltIns;
        public static bool CanDeleteBuiltIn => false;

        public static string GetApiActionLabel(GamepadVibrationProfileApiStatus status)
        {
            switch (status)
            {
                case GamepadVibrationProfileApiStatus.UpToDate: return "API UP TO DATE";
                case GamepadVibrationProfileApiStatus.OutOfDate: return "UPDATE API";
                default: return "GENERATE API";
            }
        }

        public static GamepadVibrationProfileCreationData ResolveCreationData(
            GamepadVibrationProfileCreationMode mode,
            GamepadVibrationPreset builtInPreset,
            GamepadVibrationProfile existingProfile)
        {
            if (mode == GamepadVibrationProfileCreationMode.CopyExisting && existingProfile != null)
            {
                return new GamepadVibrationProfileCreationData(
                    existingProfile.PlaybackMode,
                    existingProfile.Settings,
                    existingProfile.Description);
            }

            if (mode == GamepadVibrationProfileCreationMode.CopyBuiltIn &&
                builtInPreset != GamepadVibrationPreset.Custom)
            {
                GamepadVibrationSettings settings = GamepadVibrationPresets.Get(builtInPreset);
                return new GamepadVibrationProfileCreationData(
                    settings.Pulses > 1
                        ? GamepadVibrationPlaybackMode.Pulsed
                        : GamepadVibrationPlaybackMode.OneShot,
                    settings,
                    $"Editable custom copy of the built-in {UnityEditor.ObjectNames.NicifyVariableName(builtInPreset.ToString())} preset.");
            }

            return new GamepadVibrationProfileCreationData(
                GamepadVibrationPlaybackMode.OneShot,
                new GamepadVibrationSettings(0.5f, 0.5f, 0.25f),
                "Custom vibration profile.");
        }

        public static GamepadVibrationProfileApiStatus GetApiStatus(
            IEnumerable<GamepadVibrationProfile> profiles, string generatedPath)
        {
            if (!System.IO.File.Exists(generatedPath))
                return GamepadVibrationProfileApiStatus.NotGenerated;

            string expected = GamepadVibrationProfileCodeGenerator.GenerateSource(profiles);
            string current = System.IO.File.ReadAllText(generatedPath);
            return string.Equals(expected, current, StringComparison.Ordinal)
                ? GamepadVibrationProfileApiStatus.UpToDate
                : GamepadVibrationProfileApiStatus.OutOfDate;
        }

        public static List<GamepadVibrationProfile> FilterCustomProfiles(
            IEnumerable<GamepadVibrationProfile> profiles, string search)
        {
            string query = search?.Trim() ?? string.Empty;
            return profiles
                .Where(profile => profile != null)
                .Where(profile => query.Length == 0 ||
                    profile.name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (profile.CodeName ?? string.Empty).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (profile.Description ?? string.Empty).IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .OrderBy(profile => profile.name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
