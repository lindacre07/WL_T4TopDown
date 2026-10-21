using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TLD.Trail2D.Editor
{
    [CustomEditor(typeof(TLDTrailRenderer))]
    public class TLDTrailRendererEditor : UnityEditor.Editor
    {
        private TLDTrailRenderer trail;
        private bool showVisualCustomization = true;
        private bool showBehaviorSettings = true;
        private bool showAdvancedEffects = false;
        private bool showUniqueFeatures = false;
        private bool showPerformance = false;

        private Texture2D previewTexture;
        private const int PREVIEW_WIDTH = 350;
        private const int PREVIEW_HEIGHT = 150;

        // Preset Library
        private List<TLDTrailPreset> presetLibrary = new List<TLDTrailPreset>();

        // Performance colors
        private Color accentColor = new Color(0.2f, 0.6f, 1f);
        private Color lowImpactColor = new Color(0.3f, 0.8f, 0.3f);
        private Color mediumImpactColor = new Color(0.9f, 0.7f, 0.2f);
        private Color highImpactColor = new Color(0.9f, 0.3f, 0.3f);

        // Style cache
        private GUIStyle headerStyle;
        private GUIStyle sectionHeaderStyle;
        private GUIStyle presetButtonStyle;
        private bool stylesInitialized = false;

        private void OnEnable()
        {
            trail = (TLDTrailRenderer)target;
            LoadPresetLibrary();
            GeneratePreviewTexture();
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = accentColor }
            };

            sectionHeaderStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = accentColor }
            };

            presetButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 9,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 40,
                wordWrap = true
            };

            stylesInitialized = true;
        }

        public override void OnInspectorGUI()
        {
            InitializeStyles();
            serializedObject.Update();

            DrawCleanHeader();
            DrawPresetLibrary();

            EditorGUILayout.Space(5);
            DrawLine();
            EditorGUILayout.Space(5);

            DrawTrailTypeSelector();
            DrawVisualCustomizationSection();
            DrawBehaviorSettingsSection();
            DrawAdvancedEffectsSection();
            DrawUniqueFeaturesSection();
            DrawPerformanceSection();

            EditorGUILayout.Space(5);
            DrawLine();
            EditorGUILayout.Space(5);

            DrawPreviewSection();
            DrawActionButtons();

            if (serializedObject.ApplyModifiedProperties())
            {
                GeneratePreviewTexture();
            }
        }

        #region Header
        private void DrawCleanHeader()
        {
            EditorGUILayout.LabelField("TLD ULTIMATE 2D TRAIL SYSTEM", headerStyle);
            EditorGUILayout.Space(3);
        }

        private void DrawLine()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }
        #endregion

        #region Preset Library
        private void LoadPresetLibrary()
        {
            presetLibrary.Clear();

            // Original 5 presets (removed Ghost Replay)
            presetLibrary.Add(TLDTrailPreset.CreateSlashEffect());
            presetLibrary.Add(TLDTrailPreset.CreateMagicStreak());
            presetLibrary.Add(TLDTrailPreset.CreateJetTrail());
            presetLibrary.Add(TLDTrailPreset.CreateRainbowDash());
            presetLibrary.Add(TLDTrailPreset.CreateRibbonFlow());

            // First 10 unique presets
            presetLibrary.Add(TLDTrailPreset.CreateLightningBolt());
            presetLibrary.Add(TLDTrailPreset.CreateFireComet());
            presetLibrary.Add(TLDTrailPreset.CreatePoisonOoze());
            presetLibrary.Add(TLDTrailPreset.CreateShadowFlicker());
            presetLibrary.Add(TLDTrailPreset.CreateCrystalShatter());
            presetLibrary.Add(TLDTrailPreset.CreateBloodSplatter());
            presetLibrary.Add(TLDTrailPreset.CreateGlitchDistortion());
            presetLibrary.Add(TLDTrailPreset.CreateSpiralVortex());
            presetLibrary.Add(TLDTrailPreset.CreateEtherealWisp());
            presetLibrary.Add(TLDTrailPreset.CreateArcaneRune());

            // 10 NEW CRAZY presets
            presetLibrary.Add(TLDTrailPreset.CreateNeonPulse());
            presetLibrary.Add(TLDTrailPreset.CreateChaoticStorm());
            presetLibrary.Add(TLDTrailPreset.CreateMoltenLava());
            presetLibrary.Add(TLDTrailPreset.CreateVoidRift());
            presetLibrary.Add(TLDTrailPreset.CreatePixelExplosion());
            presetLibrary.Add(TLDTrailPreset.CreatePlasmaBeam());
            presetLibrary.Add(TLDTrailPreset.CreateInkSplash());
            presetLibrary.Add(TLDTrailPreset.CreateHolyLight());
            presetLibrary.Add(TLDTrailPreset.CreateBioHazard());
            presetLibrary.Add(TLDTrailPreset.CreateQuantumFlux());
        }

        private void DrawPresetLibrary()
        {
            EditorGUILayout.LabelField("Quick Presets", EditorStyles.boldLabel);
            EditorGUILayout.Space(3);

            // Calculate rows needed (5 columns per row for 25 presets)
            int columns = 5;
            int rows = Mathf.CeilToInt(presetLibrary.Count / (float)columns);

            for (int row = 0; row < rows; row++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int col = 0; col < columns; col++)
                {
                    int index = row * columns + col;
                    if (index < presetLibrary.Count)
                    {
                        DrawCompactPresetButton(presetLibrary[index]);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawCompactPresetButton(TLDTrailPreset preset)
        {
            string label = preset.presetName.Replace(" ", "\n");

            Color buttonColor = GetPresetColor(preset.presetName);
            GUI.backgroundColor = buttonColor;

            if (GUILayout.Button(label, presetButtonStyle, GUILayout.Height(40)))
            {
                trail.SetPreset(preset);
                EditorUtility.SetDirty(trail);
                GeneratePreviewTexture();
            }

            GUI.backgroundColor = Color.white;
        }

        private Color GetPresetColor(string presetName)
        {
            // Color coding by category
            if (presetName.Contains("Slash") || presetName.Contains("Strike") || presetName.Contains("Cut"))
                return new Color(0.85f, 0.9f, 1f); // Light blue
            else if (presetName.Contains("Magic") || presetName.Contains("Mystic") || presetName.Contains("Arcane"))
                return new Color(0.85f, 0.75f, 1f); // Light purple
            else if (presetName.Contains("Fire") || presetName.Contains("Flame") || presetName.Contains("Burn"))
                return new Color(1f, 0.7f, 0.5f); // Orange
            else if (presetName.Contains("Electric") || presetName.Contains("Lightning") || presetName.Contains("Spark"))
                return new Color(0.9f, 1f, 0.6f); // Yellow
            else if (presetName.Contains("Rainbow") || presetName.Contains("Prismatic"))
                return new Color(1f, 0.9f, 0.95f); // Pink
            else if (presetName.Contains("Ghost") || presetName.Contains("Spirit") || presetName.Contains("Ethereal"))
                return new Color(0.75f, 0.9f, 1f); // Cyan
            else if (presetName.Contains("Poison") || presetName.Contains("Toxic") || presetName.Contains("Acid"))
                return new Color(0.7f, 1f, 0.7f); // Green
            else if (presetName.Contains("Shadow") || presetName.Contains("Dark") || presetName.Contains("Void"))
                return new Color(0.7f, 0.7f, 0.8f); // Gray
            else if (presetName.Contains("Blood") || presetName.Contains("Crimson"))
                return new Color(1f, 0.75f, 0.75f); // Red
            else if (presetName.Contains("Ice") || presetName.Contains("Frost") || presetName.Contains("Crystal"))
                return new Color(0.8f, 0.95f, 1f); // Ice blue
            else if (presetName.Contains("Jet") || presetName.Contains("Rocket") || presetName.Contains("Smoke"))
                return new Color(1f, 0.8f, 0.6f); // Tan
            else if (presetName.Contains("Ribbon") || presetName.Contains("Cloth") || presetName.Contains("Fabric"))
                return new Color(1f, 0.75f, 0.75f); // Rose
            else if (presetName.Contains("Glitch") || presetName.Contains("Digital") || presetName.Contains("Cyber"))
                return new Color(0.6f, 1f, 1f); // Bright cyan
            else if (presetName.Contains("Spiral") || presetName.Contains("Helix") || presetName.Contains("Twist"))
                return new Color(1f, 0.85f, 1f); // Light magenta
            else
                return Color.white;
        }

        private string GetPresetEmoji(string presetName)
        {
            // Removed - no emojis
            return "";
        }
        #endregion

        #region Trail Type Selector
        private void DrawTrailTypeSelector()
        {
            EditorGUILayout.Space(3);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trailType"), new GUIContent("Trail Type"));

            // Simple help text
            var trailTypeProp = serializedObject.FindProperty("trailType");
            string helpText = GetTrailTypeDescription(trailTypeProp.enumValueIndex);
            EditorGUILayout.HelpBox(helpText, MessageType.None);
        }

        private string GetTrailTypeDescription(int index)
        {
            switch (index)
            {
                case 0: return "Motion Trail - Character dashes, dodges, quick movements";
                case 1: return "Projectile Trail - Bullets, arrows, magic missiles";
                case 2: return "Path History - Shows recent movement path";
                case 3: return "Ribbon Trail - Flowing fabric-like trails";
                default: return "";
            }
        }
        #endregion

        #region Visual Customization
        private void DrawVisualCustomizationSection()
        {
            EditorGUILayout.Space(5);
            showVisualCustomization = EditorGUILayout.Foldout(showVisualCustomization, "Visual Customization", true, sectionHeaderStyle);

            if (showVisualCustomization)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("colorGradient"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("widthCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("opacityCurve"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("trailMaterial"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("trailTexture"));

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Visual Effects", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useGlow"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useDashed"));
                if (serializedObject.FindProperty("useDashed").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dashLength"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("dashGap"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useSparkles"));
                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Behavior Settings
        private void DrawBehaviorSettingsSection()
        {
            EditorGUILayout.Space(5);
            showBehaviorSettings = EditorGUILayout.Foldout(showBehaviorSettings, "Behavior Settings", true, sectionHeaderStyle);

            if (showBehaviorSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("trailDuration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minVertexDistance"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minTimeBetweenPoints"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("smoothingIterations"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("distanceBased"));

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Velocity Thresholds", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableVelocityThreshold"));
                if (serializedObject.FindProperty("enableVelocityThreshold").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("minVelocity"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxVelocity"));
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Advanced Effects
        private void DrawAdvancedEffectsSection()
        {
            EditorGUILayout.Space(5);
            showAdvancedEffects = EditorGUILayout.Foldout(showAdvancedEffects, "Advanced Effects", true, sectionHeaderStyle);

            if (showAdvancedEffects)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Turbulence", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableTurbulence"));
                if (serializedObject.FindProperty("enableTurbulence").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("turbulenceStrength"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("turbulenceFrequency"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Speed Stretching", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("speedBasedStretching"));
                if (serializedObject.FindProperty("speedBasedStretching").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stretchMultiplier"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Tapering", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("taperMode"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("taperSharpness"));

                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Unique Features
        private void DrawUniqueFeaturesSection()
        {
            EditorGUILayout.Space(5);
            showUniqueFeatures = EditorGUILayout.Foldout(showUniqueFeatures, "Unique Features", true, sectionHeaderStyle);

            if (showUniqueFeatures)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Rainbow Mode", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useRainbowMode"));
                if (serializedObject.FindProperty("useRainbowMode").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rainbowSpeed"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Sprite Stamping", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableSpriteStamping"));
                if (serializedObject.FindProperty("enableSpriteStamping").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stampSprite"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stampInterval"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stampScale"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Physics Effects", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("applyGravity"));
                if (serializedObject.FindProperty("applyGravity").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("gravityStrength"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("applyWind"));
                if (serializedObject.FindProperty("applyWind").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("windDirection"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("windStrength"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Special Modes", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("enableCollision"));
                if (serializedObject.FindProperty("enableCollision").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("collisionMask"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ghostReplayMode"));

                EditorGUI.indentLevel--;
            }
        }
        #endregion

        #region Performance
        private void DrawPerformanceSection()
        {
            EditorGUILayout.Space(5);
            showPerformance = EditorGUILayout.Foldout(showPerformance, "Performance", true, sectionHeaderStyle);

            if (showPerformance)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxTrailPoints"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useObjectPooling"));
                if (serializedObject.FindProperty("useObjectPooling").boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("poolSize"));
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space(5);
                float impact = trail.GetEstimatedPerformanceImpact();
                DrawPerformanceImpactBar(impact);

                if (Application.isPlaying)
                {
                    EditorGUILayout.Space(3);
                    EditorGUILayout.LabelField($"Active Points: {trail.GetPointCount()}", EditorStyles.miniLabel);
                }

                EditorGUI.indentLevel--;
            }
        }

        private void DrawPerformanceImpactBar(float impact)
        {
            EditorGUILayout.LabelField("Performance Impact", EditorStyles.miniLabel);

            Rect barRect = GUILayoutUtility.GetRect(18, 18);
            barRect = EditorGUI.IndentedRect(barRect);

            EditorGUI.DrawRect(barRect, new Color(0.2f, 0.2f, 0.2f));

            float fillAmount = Mathf.Clamp01(impact / 10f);
            Color fillColor = Color.Lerp(
                Color.Lerp(lowImpactColor, mediumImpactColor, fillAmount * 2f),
                highImpactColor,
                Mathf.Max(0, (fillAmount - 0.5f) * 2f)
            );

            Rect fillRect = new Rect(barRect.x, barRect.y, barRect.width * fillAmount, barRect.height);
            EditorGUI.DrawRect(fillRect, fillColor);

            Handles.DrawSolidRectangleWithOutline(barRect, Color.clear, Color.gray);

            string impactLabel = impact < 3f ? "Low" : impact < 7f ? "Medium" : "High";
            GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            EditorGUI.LabelField(barRect, impactLabel, labelStyle);
        }
        #endregion

        #region Preview
        private void DrawPreviewSection()
        {
            EditorGUILayout.LabelField("Live Preview", EditorStyles.boldLabel);

            if (previewTexture != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(previewTexture, GUILayout.Width(PREVIEW_WIDTH), GUILayout.Height(PREVIEW_HEIGHT));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                EditorGUILayout.Space(3);
                if (GUILayout.Button("Refresh Preview", GUILayout.Height(25)))
                {
                    GeneratePreviewTexture();
                }
            }
        }

        private void GeneratePreviewTexture()
        {
            if (previewTexture == null)
            {
                previewTexture = new Texture2D(PREVIEW_WIDTH, PREVIEW_HEIGHT, TextureFormat.RGBA32, false);
            }

            // Make sure texture is the right size
            if (previewTexture.width != PREVIEW_WIDTH || previewTexture.height != PREVIEW_HEIGHT)
            {
                previewTexture.Reinitialize(PREVIEW_WIDTH, PREVIEW_HEIGHT);
            }

            Color[] clearColors = new Color[PREVIEW_WIDTH * PREVIEW_HEIGHT];
            for (int i = 0; i < clearColors.Length; i++)
            {
                clearColors[i] = new Color(0.1f, 0.1f, 0.15f, 1f);
            }
            previewTexture.SetPixels(clearColors);

            DrawTrailPreview();
            previewTexture.Apply();
        }

        private void DrawTrailPreview()
        {
            int segments = 50;
            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;

                int x = (int)(30 + t * (PREVIEW_WIDTH - 60));
                int y = (int)(PREVIEW_HEIGHT / 2 + Mathf.Sin(t * Mathf.PI * 2f) * 35f);

                Color color = GetGradientColor(t);
                float width = GetCurveValue(t) * 15f;

                DrawCircle(x, y, (int)width, color);
            }
        }

        private Color GetGradientColor(float t)
        {
            var gradientProp = serializedObject.FindProperty("colorGradient");
            if (gradientProp != null && gradientProp.propertyType == SerializedPropertyType.Gradient)
            {
                return gradientProp.gradientValue.Evaluate(t);
            }
            return Color.cyan;
        }

        private float GetCurveValue(float t)
        {
            var curveProp = serializedObject.FindProperty("widthCurve");
            if (curveProp != null && curveProp.propertyType == SerializedPropertyType.AnimationCurve)
            {
                return curveProp.animationCurveValue.Evaluate(t);
            }
            return 1f;
        }

        private void DrawCircle(int centerX, int centerY, int radius, Color color)
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        int px = centerX + x;
                        int py = centerY + y;

                        if (px >= 0 && px < PREVIEW_WIDTH && py >= 0 && py < PREVIEW_HEIGHT)
                        {
                            Color existing = previewTexture.GetPixel(px, py);
                            Color blended = Color.Lerp(existing, color, color.a);
                            previewTexture.SetPixel(px, py, blended);
                        }
                    }
                }
            }
        }
        #endregion

        #region Action Buttons
        private void DrawActionButtons()
        {
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset Trail", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Reset Trail",
                    "Clear the current trail?", "Yes", "No"))
                {
                    trail.ClearTrail();
                }
            }

            if (GUILayout.Button("Save as Preset", GUILayout.Height(30)))
            {
                CreatePresetFromCurrent();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void CreatePresetFromCurrent()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Trail Preset",
                "MyTrailPreset",
                "asset",
                "Save your custom trail preset");

            if (!string.IsNullOrEmpty(path))
            {
                TLDTrailPreset preset = CreateInstance<TLDTrailPreset>();
                preset.presetName = "Custom Preset";
                preset.description = "User-created custom trail preset";

                AssetDatabase.CreateAsset(preset, path);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = preset;

                EditorUtility.DisplayDialog("Preset Saved",
                    $"Trail preset saved successfully at:\n{path}", "OK");
            }
        }
        #endregion

        #region Helper Methods
        private bool DrawFoldoutSection(string title, bool foldout)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.3f, 0.7f, 1f) }
            };

            bool newFoldout = EditorGUILayout.Foldout(foldout, title, true, foldoutStyle);

            if (!newFoldout)
            {
                EditorGUILayout.EndVertical();
            }

            return newFoldout;
        }
        #endregion

        private void OnDestroy()
        {
            if (previewTexture != null)
            {
                DestroyImmediate(previewTexture);
            }
        }
    }
}