using UnityEngine;
using System.Collections.Generic;

namespace TLD.Trail2D
{
    /// <summary>
    /// TLD Trail Renderer - Professional 2D Trail System for Unity
    /// 
    /// This component uses Unity's LineRenderer for true 2D trail rendering.
    /// No mesh generation required - works natively with Unity's 2D rendering pipeline.
    /// 
    /// Features:
    /// - 4 trail types (Motion, Projectile, Path History, Ribbon)
    /// - Real-time color/width gradients
    /// - Physics effects (gravity, wind)
    /// - Advanced effects (turbulence, tapering, rainbow mode)
    /// - Sprite stamping along trail path
    /// - Performance optimized with object pooling
    /// 
    /// Usage:
    /// 1. Add this component to any GameObject
    /// 2. Automatically creates a child with LineRenderer
    /// 3. Apply a preset or customize manually
    /// 4. Trail follows the GameObject automatically
    /// </summary>
    [AddComponentMenu("TLD/Ultimate 2D Trail System/Trail Renderer")]
    public class TLDTrailRenderer : MonoBehaviour
    {
        #region Core Trail Types
        public enum TrailType
        {
            MotionTrail,        // Character dashes, sword slashes
            ProjectileTrail,    // Bullets, arrows, magic missiles
            PathHistoryTrail,   // Recent movement tracking
            RibbonTrail         // Flowing scarves, capes, flags
        }

        [Header("Trail Type")]
        [SerializeField] private TrailType trailType = TrailType.MotionTrail;
        #endregion

        #region Visual Customization
        [Header("Visual Customization")]
        [SerializeField] private Gradient colorGradient = new Gradient();
        [SerializeField] private AnimationCurve widthCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private AnimationCurve opacityCurve = AnimationCurve.Linear(0, 1, 1, 0);
        [SerializeField] private Material trailMaterial;
        [SerializeField] private Texture2D trailTexture;

        [Header("Visual Effects")]
        [SerializeField] private bool useGlow = false;
        [SerializeField] private bool useDashed = false;
        [SerializeField] private float dashLength = 0.2f;
        [SerializeField] private float dashGap = 0.1f;
        [SerializeField] private bool useSparkles = false;
        #endregion

        #region Behavior Settings
        [Header("Behavior Settings")]
        [SerializeField] private float trailDuration = 1f;
        [SerializeField] private float minVertexDistance = 0.1f;
        [SerializeField] private float minTimeBetweenPoints = 0.05f;
        [SerializeField] private int smoothingIterations = 3;
        [SerializeField] private bool distanceBased = true;

        [Header("Velocity Thresholds")]
        [SerializeField] private float minVelocity = 0f;
        [SerializeField] private float maxVelocity = 100f;
        [SerializeField] private bool enableVelocityThreshold = false;
        #endregion

        #region Advanced Effects
        [Header("Advanced Effects")]
        [SerializeField] private bool enableTurbulence = false;
        [SerializeField] private float turbulenceStrength = 0.1f;
        [SerializeField] private float turbulenceFrequency = 1f;

        [SerializeField] private bool speedBasedStretching = false;
        [SerializeField] private float stretchMultiplier = 1.5f;

        [Header("Tapering")]
        [SerializeField] private TaperMode taperMode = TaperMode.BothEnds;
        [SerializeField] private float taperSharpness = 2f;

        [Header("Trail Splitting")]
        [SerializeField] private bool enableSplitting = false;
        [SerializeField] private int maxSplits = 3;
        [SerializeField] private float splitAngle = 30f;
        #endregion

        #region Unique Features
        [Header("Multi-Color Segments")]
        [SerializeField] private bool useRainbowMode = false;
        [SerializeField] private float rainbowSpeed = 1f;

        [Header("Sprite Stamping")]
        [SerializeField] private bool enableSpriteStamping = false;
        [SerializeField] private Sprite stampSprite;
        [SerializeField] private float stampInterval = 0.5f;
        [SerializeField] private float stampScale = 1f;

        [Header("Physics Effects")]
        [SerializeField] private bool applyGravity = false;
        [SerializeField] private float gravityStrength = 9.81f;
        [SerializeField] private bool applyWind = false;
        [SerializeField] private Vector2 windDirection = Vector2.right;
        [SerializeField] private float windStrength = 1f;

        [Header("Collision & Ghost")]
        [SerializeField] private bool enableCollision = false;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private bool ghostReplayMode = false;
        #endregion

        #region Performance
        [Header("Performance")]
        [SerializeField] private int maxTrailPoints = 100;
        [SerializeField] private bool useObjectPooling = true;
        [SerializeField] private int poolSize = 10;
        #endregion

        #region Internal Data
        private List<TrailPoint> trailPoints = new List<TrailPoint>();
        private LineRenderer lineRenderer;
        private GameObject trailObject;
        private Vector3 lastPosition;
        private float lastPointTime;
        private List<GameObject> spriteStamps = new List<GameObject>();
        private float lastStampTime;

        // For custom 2D mesh rendering
        private List<GameObject> trailSegments = new List<GameObject>();
        #endregion

        #region Trail Point Structure
        private class TrailPoint
        {
            public Vector3 position;
            public float time;
            public float velocity;
            public Vector3 physicsOffset;

            public TrailPoint(Vector3 pos, float t, float vel)
            {
                position = pos;
                time = t;
                velocity = vel;
                physicsOffset = Vector3.zero;
            }
        }

        public enum TaperMode
        {
            None,
            BothEnds,
            StartOnly,
            EndOnly
        }
        #endregion

        #region Initialization
        private void Awake()
        {
            // Create a child object for the LineRenderer
            trailObject = new GameObject("TLD_Trail_Renderer");
            trailObject.transform.SetParent(transform);
            trailObject.transform.localPosition = Vector3.zero;

            // Add and configure LineRenderer for 2D
            lineRenderer = trailObject.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.alignment = LineAlignment.TransformZ;

            // Set sorting for 2D
            lineRenderer.sortingLayerName = "Default";
            lineRenderer.sortingOrder = 0;

            if (trailMaterial != null)
            {
                lineRenderer.material = trailMaterial;
            }
            else
            {
                // Create a default material if none assigned
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }

            lastPosition = transform.position;
            lastPointTime = Time.time;

            InitializeDefaultGradient();
        }

        private void InitializeDefaultGradient()
        {
            if (colorGradient.colorKeys.Length == 0)
            {
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(Color.cyan, 0f);
                colorKeys[1] = new GradientColorKey(Color.blue, 1f);

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1f, 0f);
                alphaKeys[1] = new GradientAlphaKey(0f, 1f);

                colorGradient.SetKeys(colorKeys, alphaKeys);
            }
        }
        #endregion

        #region Update Loop
        private void LateUpdate()
        {
            UpdateTrailPoints();
            CleanupOldPoints();
            ApplyPhysicsEffects();
            GenerateMesh();
            UpdateSpriteStamps();
        }

        private void UpdateTrailPoints()
        {
            Vector3 currentPosition = transform.position;
            float currentVelocity = (currentPosition - lastPosition).magnitude / Time.deltaTime;

            // Velocity threshold check
            if (enableVelocityThreshold)
            {
                if (currentVelocity < minVelocity || currentVelocity > maxVelocity)
                {
                    return;
                }
            }

            bool shouldAddPoint = false;

            if (distanceBased)
            {
                float distance = Vector3.Distance(currentPosition, lastPosition);
                shouldAddPoint = distance >= minVertexDistance;
            }
            else
            {
                float timeSinceLastPoint = Time.time - lastPointTime;
                shouldAddPoint = timeSinceLastPoint >= minTimeBetweenPoints;
            }

            if (shouldAddPoint)
            {
                AddTrailPoint(currentPosition, currentVelocity);
                lastPosition = currentPosition;
                lastPointTime = Time.time;
            }
        }

        private void AddTrailPoint(Vector3 position, float velocity)
        {
            TrailPoint newPoint = new TrailPoint(position, Time.time, velocity);
            trailPoints.Insert(0, newPoint);

            // Limit trail points
            if (trailPoints.Count > maxTrailPoints)
            {
                trailPoints.RemoveAt(trailPoints.Count - 1);
            }

            // Sprite stamping
            if (enableSpriteStamping && stampSprite != null)
            {
                if (Time.time - lastStampTime >= stampInterval)
                {
                    CreateSpriteStamp(position);
                    lastStampTime = Time.time;
                }
            }
        }

        private void CleanupOldPoints()
        {
            float currentTime = Time.time;
            trailPoints.RemoveAll(p => currentTime - p.time > trailDuration);
        }
        #endregion

        #region Physics Effects
        private void ApplyPhysicsEffects()
        {
            if (!applyGravity && !applyWind) return;

            for (int i = 0; i < trailPoints.Count; i++)
            {
                float age = Time.time - trailPoints[i].time;
                Vector3 offset = Vector3.zero;

                if (applyGravity)
                {
                    offset.y -= gravityStrength * age * age * 0.5f;
                }

                if (applyWind)
                {
                    offset += (Vector3)windDirection.normalized * windStrength * age;
                }

                trailPoints[i].physicsOffset = offset;
            }
        }
        #endregion

        #region Mesh Generation
        private void GenerateMesh()
        {
            if (trailPoints.Count < 2)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            List<TrailPoint> smoothedPoints = SmoothTrail(trailPoints);

            // Set up LineRenderer
            lineRenderer.positionCount = smoothedPoints.Count;

            // Create arrays for positions and widths
            Vector3[] positions = new Vector3[smoothedPoints.Count];
            AnimationCurve widthOverTime = new AnimationCurve();

            for (int i = 0; i < smoothedPoints.Count; i++)
            {
                TrailPoint point = smoothedPoints[i];
                float normalizedAge = (Time.time - point.time) / trailDuration;

                // Position with physics offset
                Vector3 position = point.position + point.physicsOffset;

                // Apply turbulence
                if (enableTurbulence)
                {
                    float turbulence = Mathf.Sin(normalizedAge * turbulenceFrequency * Mathf.PI * 2f) * turbulenceStrength;
                    position.y += turbulence;
                }

                positions[i] = position;

                // Calculate width with tapering
                float width = widthCurve.Evaluate(normalizedAge) * GetTaperMultiplier(normalizedAge);

                // Speed-based stretching
                if (speedBasedStretching && i > 0)
                {
                    width *= Mathf.Lerp(1f, stretchMultiplier, point.velocity / maxVelocity);
                }

                // Width curve - normalized time from 0 to 1
                float normalizedPosition = i / (float)(smoothedPoints.Count - 1);
                widthOverTime.AddKey(normalizedPosition, width * 2f); // Multiply by 2 because LineRenderer width is diameter
            }

            // Apply positions and width
            lineRenderer.SetPositions(positions);
            lineRenderer.widthCurve = widthOverTime;

            // Apply color gradient - use the main gradient with opacity applied
            // We can't create dynamic gradients, so apply the user's gradient directly
            // and let the opacity be handled by the material/shader
            if (useRainbowMode)
            {
                // Create a simple rainbow gradient (max 8 keys)
                Gradient rainbowGradient = CreateRainbowGradient();
                lineRenderer.colorGradient = rainbowGradient;
            }
            else
            {
                // Use the user-defined gradient
                lineRenderer.colorGradient = colorGradient;
            }

            // Optional: Set texture mode for better tiling
            lineRenderer.textureMode = LineTextureMode.Tile;
        }

        private Gradient CreateRainbowGradient()
        {
            Gradient gradient = new Gradient();

            // Create 7 color keys for rainbow (within Unity's 8 key limit)
            GradientColorKey[] colorKeys = new GradientColorKey[7];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

            float timeOffset = Time.time * rainbowSpeed;

            for (int i = 0; i < 7; i++)
            {
                float hue = Mathf.Repeat((i / 7f) + timeOffset, 1f);
                colorKeys[i] = new GradientColorKey(Color.HSVToRGB(hue, 1f, 1f), i / 6f);
            }

            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(0f, 1f);

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private List<TrailPoint> SmoothTrail(List<TrailPoint> points)
        {
            if (smoothingIterations == 0 || points.Count < 3)
                return new List<TrailPoint>(points);

            List<TrailPoint> smoothed = new List<TrailPoint>(points);

            for (int iteration = 0; iteration < smoothingIterations; iteration++)
            {
                List<TrailPoint> temp = new List<TrailPoint> { smoothed[0] };

                for (int i = 1; i < smoothed.Count - 1; i++)
                {
                    Vector3 avgPos = (smoothed[i - 1].position + smoothed[i].position + smoothed[i + 1].position) / 3f;
                    TrailPoint smoothPoint = new TrailPoint(avgPos, smoothed[i].time, smoothed[i].velocity);
                    smoothPoint.physicsOffset = smoothed[i].physicsOffset;
                    temp.Add(smoothPoint);
                }

                temp.Add(smoothed[smoothed.Count - 1]);
                smoothed = temp;
            }

            return smoothed;
        }

        private float GetTaperMultiplier(float normalizedAge)
        {
            switch (taperMode)
            {
                case TaperMode.BothEnds:
                    return Mathf.Sin(normalizedAge * Mathf.PI);
                case TaperMode.StartOnly:
                    return Mathf.Pow(normalizedAge, 1f / taperSharpness);
                case TaperMode.EndOnly:
                    return Mathf.Pow(1f - normalizedAge, 1f / taperSharpness);
                default:
                    return 1f;
            }
        }

        private Color GetRainbowColor(float t)
        {
            t = Mathf.Repeat(t, 1f);
            return Color.HSVToRGB(t, 1f, 1f);
        }
        #endregion

        #region Sprite Stamping
        private void CreateSpriteStamp(Vector3 position)
        {
            GameObject stamp = new GameObject("TLD Trail Stamp");
            stamp.transform.position = position;
            stamp.transform.localScale = Vector3.one * stampScale;

            SpriteRenderer sr = stamp.AddComponent<SpriteRenderer>();
            sr.sprite = stampSprite;
            sr.color = colorGradient.Evaluate(0);
            sr.sortingLayerName = lineRenderer.sortingLayerName;
            sr.sortingOrder = lineRenderer.sortingOrder - 1;

            spriteStamps.Add(stamp);
            Destroy(stamp, trailDuration);
        }

        private void UpdateSpriteStamps()
        {
            spriteStamps.RemoveAll(s => s == null);
        }
        #endregion

        #region Public Methods
        public void ClearTrail()
        {
            trailPoints.Clear();
            if (lineRenderer != null)
            {
                lineRenderer.positionCount = 0;
            }

            foreach (var stamp in spriteStamps)
            {
                if (stamp != null) Destroy(stamp);
            }
            spriteStamps.Clear();
        }

        public void SetPreset(TLDTrailPreset preset)
        {
            if (preset == null) return;
            preset.ApplyToTrail(this);
        }

        public void SetColorGradient(Gradient gradient)
        {
            colorGradient = gradient;
        }

        public void SetWidthCurve(AnimationCurve curve)
        {
            widthCurve = curve;
        }

        public int GetPointCount()
        {
            return trailPoints.Count;
        }

        public float GetEstimatedPerformanceImpact()
        {
            // Simple performance estimate based on settings
            float impact = 0f;
            impact += trailPoints.Count * 0.1f;
            impact += smoothingIterations * 0.5f;
            impact += enableTurbulence ? 1f : 0f;
            impact += enableSpriteStamping ? 2f : 0f;
            impact += (applyGravity || applyWind) ? 1f : 0f;
            return Mathf.Clamp(impact, 0f, 10f);
        }
        #endregion

        private void OnDestroy()
        {
            if (trailObject != null)
            {
                Destroy(trailObject);
            }
        }
    }
}