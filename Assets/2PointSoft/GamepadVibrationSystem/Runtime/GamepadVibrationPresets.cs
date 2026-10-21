namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Built-in vibration defaults ready for use from any script.</summary>
    public static class GamepadVibrationPresets
    {
        public static GamepadVibrationSettings LightHit => new GamepadVibrationSettings(0.20f, 0.50f, 0.10f);
        public static GamepadVibrationSettings HeavyHit => new GamepadVibrationSettings(0.80f, 1.00f, 0.25f);
        public static GamepadVibrationSettings Explosion => new GamepadVibrationSettings(1.00f, 0.80f, 0.60f, 2, 0.10f, true);
        public static GamepadVibrationSettings Heartbeat => new GamepadVibrationSettings(0.70f, 0.30f, 0.15f, 2, 0.20f);
        public static GamepadVibrationSettings Alert => new GamepadVibrationSettings(0.25f, 0.75f, 0.12f, 3, 0.10f);
        public static GamepadVibrationSettings Hazard => new GamepadVibrationSettings(0.45f, 0.55f, 0.20f, 3, 0.15f, true);
        public static GamepadVibrationSettings UISelect => new GamepadVibrationSettings(0.08f, 0.18f, 0.05f);
        public static GamepadVibrationSettings UIConfirm => new GamepadVibrationSettings(0.15f, 0.40f, 0.08f, 2, 0.05f);

        public static GamepadVibrationSettings Get(GamepadVibrationPreset preset)
        {
            switch (preset)
            {
                case GamepadVibrationPreset.LightHit: return LightHit;
                case GamepadVibrationPreset.HeavyHit: return HeavyHit;
                case GamepadVibrationPreset.Explosion: return Explosion;
                case GamepadVibrationPreset.Heartbeat: return Heartbeat;
                case GamepadVibrationPreset.Alert: return Alert;
                case GamepadVibrationPreset.Hazard: return Hazard;
                case GamepadVibrationPreset.UISelect: return UISelect;
                case GamepadVibrationPreset.UIConfirm: return UIConfirm;
                default: return default;
            }
        }
    }
}
