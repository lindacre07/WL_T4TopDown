using UnityEngine;

namespace TLD.Trail2D
{
    [CreateAssetMenu(fileName = "New TLD Trail Preset", menuName = "TLD/Ultimate 2D Trail/Trail Preset")]
    public class TLDTrailPreset : ScriptableObject
    {
        [Header("Preset Information")]
        public string presetName = "New Preset";
        [TextArea(3, 5)]
        public string description = "Custom trail preset";
        public Sprite previewIcon;

        [Header("Trail Configuration")]
        public TLDTrailRenderer.TrailType trailType;
        public Gradient colorGradient;
        public AnimationCurve widthCurve;
        public AnimationCurve opacityCurve;
        public Material trailMaterial;

        [Header("Visual Effects")]
        public bool useGlow;
        public bool useDashed;
        public float dashLength = 0.2f;
        public float dashGap = 0.1f;
        public bool useSparkles;

        [Header("Behavior")]
        public float trailDuration = 1f;
        public float minVertexDistance = 0.1f;
        public float minTimeBetweenPoints = 0.05f;
        public int smoothingIterations = 3;
        public bool distanceBased = true;

        [Header("Advanced Effects")]
        public bool enableTurbulence;
        public float turbulenceStrength = 0.1f;
        public float turbulenceFrequency = 1f;
        public bool speedBasedStretching;
        public float stretchMultiplier = 1.5f;
        public TLDTrailRenderer.TaperMode taperMode;
        public float taperSharpness = 2f;

        [Header("Unique Features")]
        public bool useRainbowMode;
        public float rainbowSpeed = 1f;
        public bool enableSpriteStamping;
        public Sprite stampSprite;
        public float stampInterval = 0.5f;
        public float stampScale = 1f;
        public bool applyGravity;
        public float gravityStrength = 9.81f;
        public bool applyWind;
        public Vector2 windDirection = Vector2.right;
        public float windStrength = 1f;

        [Header("Performance")]
        public int maxTrailPoints = 100;
        public bool useObjectPooling = true;

        public void ApplyToTrail(TLDTrailRenderer trail)
        {
            // Use reflection to apply all settings
            var trailType_field = typeof(TLDTrailRenderer).GetField("trailType",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (trailType_field != null) trailType_field.SetValue(trail, trailType);

            trail.SetColorGradient(colorGradient);
            trail.SetWidthCurve(widthCurve);

            // Apply other settings through reflection or public methods
            ApplyFieldValue(trail, "opacityCurve", opacityCurve);
            ApplyFieldValue(trail, "trailMaterial", trailMaterial);
            ApplyFieldValue(trail, "useGlow", useGlow);
            ApplyFieldValue(trail, "useDashed", useDashed);
            ApplyFieldValue(trail, "dashLength", dashLength);
            ApplyFieldValue(trail, "dashGap", dashGap);
            ApplyFieldValue(trail, "useSparkles", useSparkles);
            ApplyFieldValue(trail, "trailDuration", trailDuration);
            ApplyFieldValue(trail, "minVertexDistance", minVertexDistance);
            ApplyFieldValue(trail, "minTimeBetweenPoints", minTimeBetweenPoints);
            ApplyFieldValue(trail, "smoothingIterations", smoothingIterations);
            ApplyFieldValue(trail, "distanceBased", distanceBased);
            ApplyFieldValue(trail, "enableTurbulence", enableTurbulence);
            ApplyFieldValue(trail, "turbulenceStrength", turbulenceStrength);
            ApplyFieldValue(trail, "turbulenceFrequency", turbulenceFrequency);
            ApplyFieldValue(trail, "speedBasedStretching", speedBasedStretching);
            ApplyFieldValue(trail, "stretchMultiplier", stretchMultiplier);
            ApplyFieldValue(trail, "taperMode", taperMode);
            ApplyFieldValue(trail, "taperSharpness", taperSharpness);
            ApplyFieldValue(trail, "useRainbowMode", useRainbowMode);
            ApplyFieldValue(trail, "rainbowSpeed", rainbowSpeed);
            ApplyFieldValue(trail, "enableSpriteStamping", enableSpriteStamping);
            ApplyFieldValue(trail, "stampSprite", stampSprite);
            ApplyFieldValue(trail, "stampInterval", stampInterval);
            ApplyFieldValue(trail, "stampScale", stampScale);
            ApplyFieldValue(trail, "applyGravity", applyGravity);
            ApplyFieldValue(trail, "gravityStrength", gravityStrength);
            ApplyFieldValue(trail, "applyWind", applyWind);
            ApplyFieldValue(trail, "windDirection", windDirection);
            ApplyFieldValue(trail, "windStrength", windStrength);
            ApplyFieldValue(trail, "maxTrailPoints", maxTrailPoints);
            ApplyFieldValue(trail, "useObjectPooling", useObjectPooling);
        }

        private void ApplyFieldValue(TLDTrailRenderer trail, string fieldName, object value)
        {
            var field = typeof(TLDTrailRenderer).GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(trail, value);
        }

        #region Built-in Presets Factory
        public static TLDTrailPreset CreateSlashEffect()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Sword Slash";
            preset.description = "Sharp, quick trail for sword slashes and melee attacks";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 1f, 1f),
                new Color(0.7f, 0.9f, 1f, 0f)
            );

            preset.widthCurve = CreateCurve(new Keyframe(0, 0.3f), new Keyframe(0.2f, 1f), new Keyframe(1, 0.1f));
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0f));

            preset.trailDuration = 0.3f;
            preset.minVertexDistance = 0.05f;
            preset.smoothingIterations = 2;
            preset.taperMode = TLDTrailRenderer.TaperMode.BothEnds;
            preset.taperSharpness = 3f;
            preset.speedBasedStretching = true;
            preset.stretchMultiplier = 2f;

            return preset;
        }

        public static TLDTrailPreset CreateMagicStreak()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Magic Streak";
            preset.description = "Mystical glowing trail for magic spells and abilities";
            preset.trailType = TLDTrailRenderer.TrailType.ProjectileTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.5f, 0.2f, 1f, 1f),
                new Color(0.8f, 0.4f, 1f, 0f)
            );

            preset.widthCurve = AnimationCurve.Constant(0, 1, 0.8f);
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(0.7f, 0.8f), new Keyframe(1, 0f));

            preset.trailDuration = 0.8f;
            preset.useGlow = true;
            preset.useSparkles = true;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.05f;
            preset.turbulenceFrequency = 3f;
            preset.smoothingIterations = 4;

            return preset;
        }

        public static TLDTrailPreset CreateJetTrail()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Jet Trail";
            preset.description = "Smoke-like trail for rockets, jets, and vehicles";
            preset.trailType = TLDTrailRenderer.TrailType.ProjectileTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 0.8f, 0.3f, 0.9f),
                new Color(0.3f, 0.3f, 0.3f, 0f)
            );

            preset.widthCurve = CreateCurve(new Keyframe(0, 0.3f), new Keyframe(0.5f, 1f), new Keyframe(1, 1.2f));
            preset.opacityCurve = CreateCurve(new Keyframe(0, 0.9f), new Keyframe(1, 0f));

            preset.trailDuration = 1.5f;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.15f;
            preset.turbulenceFrequency = 1f;
            preset.smoothingIterations = 5;
            preset.taperMode = TLDTrailRenderer.TaperMode.EndOnly;

            return preset;
        }

        public static TLDTrailPreset CreateRainbowDash()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Rainbow Dash";
            preset.description = "Colorful rainbow trail with vibrant colors";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.useRainbowMode = true;
            preset.rainbowSpeed = 0.5f;
            preset.widthCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0f));

            preset.trailDuration = 1f;
            preset.smoothingIterations = 4;
            preset.taperMode = TLDTrailRenderer.TaperMode.EndOnly;
            preset.useSparkles = true;

            return preset;
        }

        public static TLDTrailPreset CreateGhostReplay()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Ghost Replay";
            preset.description = "Semi-transparent trail for racing game replays";
            preset.trailType = TLDTrailRenderer.TrailType.PathHistoryTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.3f, 0.8f, 1f, 0.5f),
                new Color(0.3f, 0.8f, 1f, 0.1f)
            );

            preset.widthCurve = AnimationCurve.Constant(0, 1, 0.9f);
            preset.opacityCurve = AnimationCurve.Constant(0, 1, 0.4f);

            preset.trailDuration = 3f;
            preset.minVertexDistance = 0.2f;
            preset.smoothingIterations = 6;
            preset.maxTrailPoints = 200;

            return preset;
        }

        public static TLDTrailPreset CreateRibbonFlow()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Flowing Ribbon";
            preset.description = "Smooth flowing trail for capes, scarves, and flags";
            preset.trailType = TLDTrailRenderer.TrailType.RibbonTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.8f, 0.1f, 0.1f, 1f),
                new Color(1f, 0.3f, 0.3f, 0.7f)
            );

            preset.widthCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0.8f));
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0.6f));

            preset.trailDuration = 0.8f;
            preset.smoothingIterations = 6;
            preset.applyGravity = true;
            preset.gravityStrength = 2f;
            preset.applyWind = true;
            preset.windStrength = 0.5f;

            return preset;
        }

        // ===== 10 NEW UNIQUE PRESETS =====

        public static TLDTrailPreset CreateLightningBolt()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Lightning Bolt";
            preset.description = "Erratic electric trail with sharp jagged edges";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 0.8f, 1f),  // Bright white-yellow
                new Color(0.3f, 0.5f, 1f, 0f)  // Electric blue fade
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.4f),
                new Keyframe(0.3f, 1.2f),
                new Keyframe(0.7f, 0.8f),
                new Keyframe(1, 0.2f)
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(0.6f, 0.9f), new Keyframe(1, 0f));

            preset.trailDuration = 0.2f;
            preset.minVertexDistance = 0.03f;
            preset.smoothingIterations = 1; // Sharp edges
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.25f;
            preset.turbulenceFrequency = 8f; // Very erratic
            preset.taperMode = TLDTrailRenderer.TaperMode.BothEnds;
            preset.taperSharpness = 4f;
            preset.useGlow = true;

            return preset;
        }

        public static TLDTrailPreset CreateFireComet()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Fire Comet";
            preset.description = "Burning trail that expands and flickers like flames";
            preset.trailType = TLDTrailRenderer.TrailType.ProjectileTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 0.9f, 1f),   // White hot
                new Color(1f, 0.3f, 0f, 0.8f),  // Orange
                new Color(0.8f, 0.1f, 0f, 0.4f), // Dark red
                new Color(0.2f, 0.2f, 0.2f, 0f)  // Smoke
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.5f),
                new Keyframe(0.2f, 1.2f),
                new Keyframe(1, 2f)  // Expands like smoke
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(0.5f, 0.8f), new Keyframe(1, 0f));

            preset.trailDuration = 1.2f;
            preset.smoothingIterations = 3;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.2f;
            preset.turbulenceFrequency = 3f;
            preset.applyGravity = true;
            preset.gravityStrength = -1f; // Rises upward (negative gravity)
            preset.taperMode = TLDTrailRenderer.TaperMode.StartOnly;
            preset.useGlow = true;

            return preset;
        }

        public static TLDTrailPreset CreatePoisonOoze()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Poison Ooze";
            preset.description = "Thick dripping toxic trail that pools and spreads";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.5f, 1f, 0.3f, 1f),  // Bright toxic green
                new Color(0.2f, 0.6f, 0.1f, 0.6f)  // Dark green
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.8f),
                new Keyframe(0.5f, 1.2f),
                new Keyframe(1, 1.5f)  // Spreads like liquid
            );
            preset.opacityCurve = AnimationCurve.Constant(0, 1, 0.8f);

            preset.trailDuration = 2f;
            preset.smoothingIterations = 8; // Very smooth/liquid
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.1f;
            preset.turbulenceFrequency = 0.5f; // Slow bubbling
            preset.applyGravity = true;
            preset.gravityStrength = 5f; // Drips down
            preset.taperMode = TLDTrailRenderer.TaperMode.StartOnly;

            return preset;
        }

        public static TLDTrailPreset CreateShadowFlicker()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Shadow Flicker";
            preset.description = "Unstable shadowy trail that phases in and out";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.2f, 0.1f, 0.3f, 0.9f),  // Dark purple
                new Color(0.05f, 0.05f, 0.05f, 0.4f)  // Nearly black
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 1.2f),
                new Keyframe(0.3f, 0.6f),
                new Keyframe(0.6f, 1.4f),
                new Keyframe(1, 0.3f)  // Flickering width
            );
            preset.opacityCurve = CreateCurve(
                new Keyframe(0, 0.8f),
                new Keyframe(0.3f, 0.4f),
                new Keyframe(0.6f, 0.7f),
                new Keyframe(1, 0f)  // Flickering opacity
            );

            preset.trailDuration = 0.6f;
            preset.minVertexDistance = 0.08f;
            preset.smoothingIterations = 2;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.15f;
            preset.turbulenceFrequency = 5f;
            preset.useDashed = true;
            preset.dashLength = 0.15f;
            preset.dashGap = 0.08f;

            return preset;
        }

        public static TLDTrailPreset CreateCrystalShatter()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Crystal Shatter";
            preset.description = "Fractured crystalline trail with sharp angular edges";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.8f, 0.95f, 1f, 1f),  // Ice blue
                new Color(0.5f, 0.8f, 1f, 0.7f),
                new Color(0.3f, 0.5f, 0.8f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.3f),
                new Keyframe(0.1f, 1.5f),
                new Keyframe(0.3f, 0.6f),
                new Keyframe(0.5f, 1.2f),
                new Keyframe(0.7f, 0.4f),
                new Keyframe(1, 0.1f)  // Jagged crystal shards
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0f));

            preset.trailDuration = 0.5f;
            preset.minVertexDistance = 0.05f;
            preset.smoothingIterations = 0; // No smoothing for sharp edges
            preset.taperMode = TLDTrailRenderer.TaperMode.BothEnds;
            preset.taperSharpness = 5f;
            preset.useSparkles = true;
            preset.useGlow = true;

            return preset;
        }

        public static TLDTrailPreset CreateBloodSplatter()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Blood Splatter";
            preset.description = "Viscous crimson trail with dripping effect";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.9f, 0.1f, 0.1f, 1f),  // Bright red
                new Color(0.5f, 0.05f, 0.05f, 0.8f)  // Dark blood
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.6f),
                new Keyframe(0.2f, 1.3f),
                new Keyframe(1, 0.9f)  // Splatters and drips
            );
            preset.opacityCurve = AnimationCurve.Constant(0, 1, 0.95f);

            preset.trailDuration = 1.5f;
            preset.smoothingIterations = 4;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.12f;
            preset.turbulenceFrequency = 2f;
            preset.applyGravity = true;
            preset.gravityStrength = 8f; // Heavy dripping
            preset.speedBasedStretching = true;
            preset.stretchMultiplier = 2.5f;

            return preset;
        }

        public static TLDTrailPreset CreateGlitchDistortion()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Glitch Distortion";
            preset.description = "Digital corruption with color artifacts and displacement";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0f, 1f, 1f, 1f),     // Cyan
                new Color(1f, 0f, 1f, 0.8f),   // Magenta
                new Color(1f, 1f, 0f, 0.6f),   // Yellow
                new Color(0.2f, 0.2f, 0.2f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.8f),
                new Keyframe(0.15f, 1.5f),
                new Keyframe(0.3f, 0.5f),
                new Keyframe(0.5f, 1.2f),
                new Keyframe(0.7f, 0.4f),
                new Keyframe(1, 0.8f)  // Erratic digital glitch
            );
            preset.opacityCurve = CreateCurve(
                new Keyframe(0, 1f),
                new Keyframe(0.3f, 0.7f),
                new Keyframe(0.6f, 0.9f),
                new Keyframe(1, 0f)
            );

            preset.trailDuration = 0.4f;
            preset.minVertexDistance = 0.04f;
            preset.smoothingIterations = 1;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.3f;
            preset.turbulenceFrequency = 10f; // Very chaotic
            preset.useDashed = true;
            preset.dashLength = 0.1f;
            preset.dashGap = 0.05f;

            return preset;
        }

        public static TLDTrailPreset CreateSpiralVortex()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Spiral Vortex";
            preset.description = "Swirling vortex trail that curves inward";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.8f, 0.3f, 1f, 1f),  // Purple
                new Color(0.3f, 0.1f, 0.6f, 0.7f),
                new Color(0.1f, 0.05f, 0.3f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 1.5f),
                new Keyframe(0.5f, 0.8f),
                new Keyframe(1, 0.3f)  // Narrows toward center
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0f));

            preset.trailDuration = 1f;
            preset.smoothingIterations = 7; // Very smooth spiral
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.18f;
            preset.turbulenceFrequency = 2f;
            preset.taperMode = TLDTrailRenderer.TaperMode.EndOnly;
            preset.taperSharpness = 3f;
            preset.useGlow = true;

            return preset;
        }

        public static TLDTrailPreset CreateEtherealWisp()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Ethereal Wisp";
            preset.description = "Soft floating spirit trail with gentle movement";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.9f, 1f, 1f, 0.7f),  // Soft cyan-white
                new Color(0.6f, 0.8f, 1f, 0.4f),
                new Color(0.4f, 0.6f, 0.9f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.7f),
                new Keyframe(0.3f, 1.1f),
                new Keyframe(0.7f, 0.9f),
                new Keyframe(1, 0.5f)  // Gentle pulsing
            );
            preset.opacityCurve = CreateCurve(
                new Keyframe(0, 0.6f),
                new Keyframe(0.5f, 0.7f),
                new Keyframe(1, 0f)
            );

            preset.trailDuration = 1.8f;
            preset.smoothingIterations = 8; // Very smooth
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.08f;
            preset.turbulenceFrequency = 1f; // Slow gentle waves
            preset.applyWind = true;
            preset.windStrength = 0.3f;
            preset.windDirection = new Vector2(0.5f, 0.5f);
            preset.useGlow = true;
            preset.useSparkles = true;

            return preset;
        }

        public static TLDTrailPreset CreateArcaneRune()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Arcane Rune";
            preset.description = "Mystical trail with pulsing magical energy";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 0.8f, 1f, 1f),   // Pink-white
                new Color(0.8f, 0.2f, 1f, 0.9f), // Purple
                new Color(0.4f, 0.1f, 0.6f, 0.5f),
                new Color(0.2f, 0.05f, 0.3f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.5f),
                new Keyframe(0.2f, 1.2f),
                new Keyframe(0.4f, 0.7f),
                new Keyframe(0.6f, 1.1f),
                new Keyframe(0.8f, 0.6f),
                new Keyframe(1, 0.3f)  // Pulsing energy
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0f));

            preset.trailDuration = 0.9f;
            preset.smoothingIterations = 4;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.12f;
            preset.turbulenceFrequency = 4f;
            preset.taperMode = TLDTrailRenderer.TaperMode.BothEnds;
            preset.taperSharpness = 2.5f;
            preset.useGlow = true;
            preset.useSparkles = true;

            return preset;
        }

        // ===== 10 CRAZY NEW PRESETS =====

        public static TLDTrailPreset CreateNeonPulse()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Neon Pulse";
            preset.description = "Intense neon trail with strobing pulses";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0f, 1f, 1f, 1f),      // Bright cyan
                new Color(1f, 0f, 1f, 1f),      // Bright magenta
                new Color(1f, 1f, 0f, 0.8f),    // Yellow
                new Color(0f, 0f, 0f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 1.5f),
                new Keyframe(0.1f, 0.3f),
                new Keyframe(0.2f, 1.8f),
                new Keyframe(0.3f, 0.4f),
                new Keyframe(0.5f, 1.6f),
                new Keyframe(0.7f, 0.5f),
                new Keyframe(1, 0.2f)  // Intense strobing
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0f));

            preset.trailDuration = 0.35f;
            preset.minVertexDistance = 0.02f;
            preset.smoothingIterations = 0; // Sharp edges
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.08f;
            preset.turbulenceFrequency = 12f; // Ultra fast
            preset.useGlow = true;
            preset.speedBasedStretching = true;
            preset.stretchMultiplier = 3f;

            return preset;
        }

        public static TLDTrailPreset CreateChaoticStorm()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Chaotic Storm";
            preset.description = "Wildly unpredictable storm of energy";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 1f, 1f),      // White lightning
                new Color(0.2f, 0.4f, 1f, 0.9f), // Blue
                new Color(0.6f, 0.2f, 0.8f, 0.6f), // Purple
                new Color(0.1f, 0.1f, 0.2f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.8f),
                new Keyframe(0.15f, 2.2f),
                new Keyframe(0.25f, 0.4f),
                new Keyframe(0.4f, 1.9f),
                new Keyframe(0.6f, 0.5f),
                new Keyframe(0.8f, 1.5f),
                new Keyframe(1, 0.3f)  // Complete chaos
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(0.7f, 0.8f), new Keyframe(1, 0f));

            preset.trailDuration = 0.6f;
            preset.minVertexDistance = 0.03f;
            preset.smoothingIterations = 1;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.4f; // EXTREME
            preset.turbulenceFrequency = 15f; // INSANE
            preset.taperMode = TLDTrailRenderer.TaperMode.None;
            preset.useGlow = true;
            preset.useSparkles = true;

            return preset;
        }

        public static TLDTrailPreset CreateMoltenLava()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Molten Lava";
            preset.description = "Thick glowing lava that pools and drips heavily";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 0.8f, 1f),    // White hot
                new Color(1f, 0.6f, 0f, 1f),    // Orange
                new Color(0.9f, 0.2f, 0f, 0.9f), // Red
                new Color(0.3f, 0.1f, 0f, 0.5f)  // Dark crust
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.6f),
                new Keyframe(0.3f, 1.5f),
                new Keyframe(0.6f, 1.8f),
                new Keyframe(1, 2.5f)  // Pools and spreads
            );
            preset.opacityCurve = AnimationCurve.Constant(0, 1, 1f);

            preset.trailDuration = 2.5f;
            preset.smoothingIterations = 7;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.15f;
            preset.turbulenceFrequency = 0.8f; // Slow bubbling
            preset.applyGravity = true;
            preset.gravityStrength = 12f; // VERY heavy
            preset.taperMode = TLDTrailRenderer.TaperMode.StartOnly;
            preset.useGlow = true;

            return preset;
        }

        public static TLDTrailPreset CreateVoidRift()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Void Rift";
            preset.description = "Reality-tearing dimensional rift with warping";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.8f, 0f, 1f, 1f),    // Bright purple edge
                new Color(0.3f, 0f, 0.5f, 0.9f), // Dark purple
                new Color(0.05f, 0f, 0.1f, 0.6f), // Nearly black
                new Color(0f, 0f, 0f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.3f),
                new Keyframe(0.2f, 2.5f),  // Tears open
                new Keyframe(0.5f, 2.8f),
                new Keyframe(0.8f, 2.2f),
                new Keyframe(1, 0.4f)  // Collapses
            );
            preset.opacityCurve = CreateCurve(
                new Keyframe(0, 1f),
                new Keyframe(0.3f, 0.7f),
                new Keyframe(0.7f, 0.8f),
                new Keyframe(1, 0f)
            );

            preset.trailDuration = 1.2f;
            preset.smoothingIterations = 2;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.35f;
            preset.turbulenceFrequency = 6f;
            preset.useDashed = true;
            preset.dashLength = 0.12f;
            preset.dashGap = 0.12f;
            preset.useGlow = true;

            return preset;
        }

        public static TLDTrailPreset CreatePixelExplosion()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Pixel Explosion";
            preset.description = "Retro pixelated trail with blocky fragments";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 1f, 1f),      // White
                new Color(1f, 0f, 0.5f, 1f),    // Hot pink
                new Color(0f, 1f, 1f, 0.8f),    // Cyan
                new Color(0.5f, 0f, 1f, 0f)     // Purple
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 1.2f),
                new Keyframe(0.15f, 0.6f),
                new Keyframe(0.3f, 1.4f),
                new Keyframe(0.45f, 0.7f),
                new Keyframe(0.6f, 1.3f),
                new Keyframe(0.75f, 0.8f),
                new Keyframe(1, 0.4f)  // Blocky pixels
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(1, 0f));

            preset.trailDuration = 0.5f;
            preset.minVertexDistance = 0.08f; // Larger spacing for pixel effect
            preset.smoothingIterations = 0; // NO smoothing - blocky!
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.25f;
            preset.turbulenceFrequency = 8f;
            preset.useSparkles = true;

            return preset;
        }

        public static TLDTrailPreset CreatePlasmaBeam()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Plasma Beam";
            preset.description = "High-energy plasma with core and corona";
            preset.trailType = TLDTrailRenderer.TrailType.ProjectileTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 1f, 1f),      // White core
                new Color(0f, 1f, 1f, 1f),      // Cyan inner
                new Color(0f, 0.5f, 1f, 0.8f),  // Blue outer
                new Color(0.2f, 0.2f, 0.8f, 0f) // Dim blue
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 1.8f),
                new Keyframe(0.2f, 2.2f),
                new Keyframe(0.5f, 2f),
                new Keyframe(0.8f, 1.5f),
                new Keyframe(1, 0.8f)  // Dissipates
            );
            preset.opacityCurve = CreateCurve(new Keyframe(0, 1f), new Keyframe(0.8f, 0.9f), new Keyframe(1, 0f));

            preset.trailDuration = 0.7f;
            preset.smoothingIterations = 5;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.06f;
            preset.turbulenceFrequency = 7f;
            preset.taperMode = TLDTrailRenderer.TaperMode.EndOnly;
            preset.useGlow = true;
            preset.speedBasedStretching = true;
            preset.stretchMultiplier = 2f;

            return preset;
        }

        public static TLDTrailPreset CreateInkSplash()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Ink Splash";
            preset.description = "Thick ink that splatters and bleeds";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.1f, 0.1f, 0.3f, 1f), // Dark blue-black
                new Color(0.05f, 0.05f, 0.15f, 0.9f),
                new Color(0.02f, 0.02f, 0.08f, 0.7f),
                new Color(0f, 0f, 0f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.5f),
                new Keyframe(0.1f, 1.8f),  // Splatters
                new Keyframe(0.3f, 2.2f),
                new Keyframe(0.6f, 2.5f),
                new Keyframe(1, 2f)  // Spreads and bleeds
            );
            preset.opacityCurve = AnimationCurve.Constant(0, 1, 0.95f);

            preset.trailDuration = 2f;
            preset.smoothingIterations = 6;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.18f;
            preset.turbulenceFrequency = 1.5f;
            preset.applyGravity = true;
            preset.gravityStrength = 6f;
            preset.speedBasedStretching = true;
            preset.stretchMultiplier = 2.8f;

            return preset;
        }

        public static TLDTrailPreset CreateHolyLight()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Holy Light";
            preset.description = "Divine radiant light with heavenly glow";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 1f, 1f, 1f),      // Pure white
                new Color(1f, 1f, 0.8f, 1f),    // Warm white
                new Color(1f, 0.95f, 0.7f, 0.7f), // Golden
                new Color(1f, 0.9f, 0.5f, 0f)   // Fade to gold
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 1.5f),
                new Keyframe(0.3f, 2f),
                new Keyframe(0.6f, 2.3f),
                new Keyframe(1, 3f)  // Radiates outward
            );
            preset.opacityCurve = CreateCurve(
                new Keyframe(0, 1f),
                new Keyframe(0.5f, 0.8f),
                new Keyframe(1, 0f)
            );

            preset.trailDuration = 1.5f;
            preset.smoothingIterations = 8; // Extremely smooth
            preset.enableTurbulence = false; // No turbulence - divine perfection
            preset.taperMode = TLDTrailRenderer.TaperMode.StartOnly;
            preset.taperSharpness = 1.5f;
            preset.useGlow = true;
            preset.useSparkles = true;
            preset.applyGravity = true;
            preset.gravityStrength = -2f; // Rises upward

            return preset;
        }

        public static TLDTrailPreset CreateBioHazard()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Bio Hazard";
            preset.description = "Mutating biological trail with pulsing growth";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(0.8f, 1f, 0.2f, 1f),  // Sickly yellow-green
                new Color(0.4f, 0.8f, 0.1f, 1f), // Toxic green
                new Color(0.2f, 0.5f, 0.1f, 0.8f), // Dark green
                new Color(0.1f, 0.2f, 0.05f, 0f)
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 0.6f),
                new Keyframe(0.2f, 1.4f),
                new Keyframe(0.35f, 0.8f),
                new Keyframe(0.5f, 1.8f),  // Pulsing growth
                new Keyframe(0.7f, 1.2f),
                new Keyframe(1, 2f)  // Mutates larger
            );
            preset.opacityCurve = AnimationCurve.Constant(0, 1, 0.9f);

            preset.trailDuration = 1.8f;
            preset.smoothingIterations = 3;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.22f;
            preset.turbulenceFrequency = 3f;
            preset.applyGravity = true;
            preset.gravityStrength = 3f;
            preset.useDashed = true;
            preset.dashLength = 0.18f;
            preset.dashGap = 0.06f; // Irregular mutating pattern

            return preset;
        }

        public static TLDTrailPreset CreateQuantumFlux()
        {
            var preset = CreateInstance<TLDTrailPreset>();
            preset.presetName = "Quantum Flux";
            preset.description = "Reality-bending quantum probability trail";
            preset.trailType = TLDTrailRenderer.TrailType.MotionTrail;

            preset.colorGradient = CreateGradient(
                new Color(1f, 0.5f, 1f, 1f),    // Pink
                new Color(0.5f, 1f, 1f, 0.9f),  // Cyan
                new Color(1f, 1f, 0.5f, 0.7f),  // Yellow
                new Color(0.5f, 0.5f, 1f, 0f)   // Blue
            );

            preset.widthCurve = CreateCurve(
                new Keyframe(0, 1.2f),
                new Keyframe(0.1f, 0.3f),  // Collapses
                new Keyframe(0.2f, 2f),    // Expands
                new Keyframe(0.35f, 0.5f),
                new Keyframe(0.5f, 1.8f),
                new Keyframe(0.65f, 0.4f),
                new Keyframe(0.8f, 1.5f),
                new Keyframe(1, 0.6f)  // Quantum uncertainty
            );
            preset.opacityCurve = CreateCurve(
                new Keyframe(0, 1f),
                new Keyframe(0.3f, 0.5f),
                new Keyframe(0.6f, 0.9f),
                new Keyframe(1, 0f)  // Phases in and out
            );

            preset.trailDuration = 0.8f;
            preset.minVertexDistance = 0.04f;
            preset.smoothingIterations = 1;
            preset.enableTurbulence = true;
            preset.turbulenceStrength = 0.3f;
            preset.turbulenceFrequency = 11f; // Very unstable
            preset.useDashed = true;
            preset.dashLength = 0.08f;
            preset.dashGap = 0.08f;
            preset.useGlow = true;
            preset.useSparkles = true;

            return preset;
        }

        private static Gradient CreateGradient(Color start, Color end)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(start, 0f);
            colorKeys[1] = new GradientColorKey(end, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(start.a, 0f);
            alphaKeys[1] = new GradientAlphaKey(end.a, 1f);

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private static Gradient CreateGradient(Color c1, Color c2, Color c3)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(c1, 0f);
            colorKeys[1] = new GradientColorKey(c2, 0.5f);
            colorKeys[2] = new GradientColorKey(c3, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[3];
            alphaKeys[0] = new GradientAlphaKey(c1.a, 0f);
            alphaKeys[1] = new GradientAlphaKey(c2.a, 0.5f);
            alphaKeys[2] = new GradientAlphaKey(c3.a, 1f);

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private static Gradient CreateGradient(Color c1, Color c2, Color c3, Color c4)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[4];
            colorKeys[0] = new GradientColorKey(c1, 0f);
            colorKeys[1] = new GradientColorKey(c2, 0.33f);
            colorKeys[2] = new GradientColorKey(c3, 0.66f);
            colorKeys[3] = new GradientColorKey(c4, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];
            alphaKeys[0] = new GradientAlphaKey(c1.a, 0f);
            alphaKeys[1] = new GradientAlphaKey(c2.a, 0.33f);
            alphaKeys[2] = new GradientAlphaKey(c3.a, 0.66f);
            alphaKeys[3] = new GradientAlphaKey(c4.a, 1f);

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private static AnimationCurve CreateCurve(params Keyframe[] keys)
        {
            return new AnimationCurve(keys);
        }
        #endregion
    }
}