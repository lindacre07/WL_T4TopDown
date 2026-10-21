using UnityEngine;

namespace TLD.Trail2D
{
    /// <summary>
    /// TLD Trail Demo - Example script showing how to use the Ultimate 2D Trail System
    /// Attach this to any GameObject to see the trail in action
    /// </summary>
    [RequireComponent(typeof(TLDTrailRenderer))]
    public class TLDTrailDemo : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private MovementMode movementMode = MovementMode.CircularMotion;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float circleRadius = 3f;
        [SerializeField] private bool followMouse = false;

        [Header("Trail Control")]
        [SerializeField] private bool trailEnabled = true;
        [SerializeField] private KeyCode toggleTrailKey = KeyCode.Space;
        [SerializeField] private KeyCode clearTrailKey = KeyCode.C;

        [Header("Preset Switching")]
        [SerializeField] private bool enablePresetCycling = true;
        [SerializeField] private KeyCode nextPresetKey = KeyCode.RightArrow;
        [SerializeField] private KeyCode prevPresetKey = KeyCode.LeftArrow;

        private TLDTrailRenderer trailRenderer;
        private float timeElapsed = 0f;
        private Vector3 startPosition;
        private int currentPresetIndex = 0;
        private TLDTrailPreset[] presets;

        public enum MovementMode
        {
            CircularMotion,
            FigureEight,
            Zigzag,
            Random,
            MouseFollow,
            UserControlled
        }

        private void Start()
        {
            trailRenderer = GetComponent<TLDTrailRenderer>();
            startPosition = transform.position;

            // Load all available presets
            LoadPresets();

            Debug.Log("=== TLD Trail Demo Controls ===");
            Debug.Log($"[{toggleTrailKey}] - Toggle Trail On/Off");
            Debug.Log($"[{clearTrailKey}] - Clear Trail");
            if (enablePresetCycling)
            {
                Debug.Log($"[{nextPresetKey}/{prevPresetKey}] - Cycle Through Presets");
            }
        }

        private void LoadPresets()
        {
            presets = new TLDTrailPreset[]
            {
                // Original 5 (removed Ghost Replay)
                TLDTrailPreset.CreateSlashEffect(),
                TLDTrailPreset.CreateMagicStreak(),
                TLDTrailPreset.CreateJetTrail(),
                TLDTrailPreset.CreateRainbowDash(),
                TLDTrailPreset.CreateRibbonFlow(),
                
                // First 10 unique presets
                TLDTrailPreset.CreateLightningBolt(),
                TLDTrailPreset.CreateFireComet(),
                TLDTrailPreset.CreatePoisonOoze(),
                TLDTrailPreset.CreateShadowFlicker(),
                TLDTrailPreset.CreateCrystalShatter(),
                TLDTrailPreset.CreateBloodSplatter(),
                TLDTrailPreset.CreateGlitchDistortion(),
                TLDTrailPreset.CreateSpiralVortex(),
                TLDTrailPreset.CreateEtherealWisp(),
                TLDTrailPreset.CreateArcaneRune(),
                
                // 10 CRAZY new presets
                TLDTrailPreset.CreateNeonPulse(),
                TLDTrailPreset.CreateChaoticStorm(),
                TLDTrailPreset.CreateMoltenLava(),
                TLDTrailPreset.CreateVoidRift(),
                TLDTrailPreset.CreatePixelExplosion(),
                TLDTrailPreset.CreatePlasmaBeam(),
                TLDTrailPreset.CreateInkSplash(),
                TLDTrailPreset.CreateHolyLight(),
                TLDTrailPreset.CreateBioHazard(),
                TLDTrailPreset.CreateQuantumFlux()
            };

            Debug.Log($"Loaded {presets.Length} trail presets - Use Arrow Keys to cycle!");
        }

        private void Update()
        {
            HandleInput();

            if (trailEnabled)
            {
                UpdateMovement();
            }
        }

        private void HandleInput()
        {
            // Toggle trail
            if (Input.GetKeyDown(toggleTrailKey))
            {
                trailEnabled = !trailEnabled;
                Debug.Log($"Trail {(trailEnabled ? "Enabled" : "Disabled")}");
            }

            // Clear trail
            if (Input.GetKeyDown(clearTrailKey))
            {
                trailRenderer.ClearTrail();
                Debug.Log("Trail Cleared");
            }

            // Cycle presets
            if (enablePresetCycling)
            {
                if (Input.GetKeyDown(nextPresetKey))
                {
                    CyclePreset(1);
                }
                else if (Input.GetKeyDown(prevPresetKey))
                {
                    CyclePreset(-1);
                }
            }
        }

        private void CyclePreset(int direction)
        {
            currentPresetIndex = (currentPresetIndex + direction + presets.Length) % presets.Length;
            trailRenderer.SetPreset(presets[currentPresetIndex]);
            Debug.Log($"Switched to Preset: {presets[currentPresetIndex].presetName}");
        }

        private void UpdateMovement()
        {
            timeElapsed += Time.deltaTime * moveSpeed;

            switch (movementMode)
            {
                case MovementMode.CircularMotion:
                    MoveInCircle();
                    break;

                case MovementMode.FigureEight:
                    MoveInFigureEight();
                    break;

                case MovementMode.Zigzag:
                    MoveInZigzag();
                    break;

                case MovementMode.Random:
                    MoveRandomly();
                    break;

                case MovementMode.MouseFollow:
                    FollowMousePosition();
                    break;

                case MovementMode.UserControlled:
                    HandleUserControl();
                    break;
            }
        }

        private void MoveInCircle()
        {
            float x = Mathf.Cos(timeElapsed) * circleRadius;
            float y = Mathf.Sin(timeElapsed) * circleRadius;
            transform.position = startPosition + new Vector3(x, y, 0);
        }

        private void MoveInFigureEight()
        {
            float x = Mathf.Sin(timeElapsed) * circleRadius;
            float y = Mathf.Sin(timeElapsed * 2f) * circleRadius * 0.5f;
            transform.position = startPosition + new Vector3(x, y, 0);
        }

        private void MoveInZigzag()
        {
            float x = timeElapsed % 4f - 2f;
            float y = Mathf.PingPong(timeElapsed, 2f) - 1f;
            transform.position = startPosition + new Vector3(x, y, 0) * circleRadius;
        }

        private void MoveRandomly()
        {
            if (Time.frameCount % 30 == 0) // Change direction every 30 frames
            {
                Vector3 randomOffset = Random.insideUnitCircle * circleRadius;
                transform.position = Vector3.Lerp(
                    transform.position,
                    startPosition + randomOffset,
                    Time.deltaTime * moveSpeed
                );
            }
        }

        private void FollowMousePosition()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            transform.position = Vector3.Lerp(
                transform.position,
                mousePos,
                Time.deltaTime * moveSpeed
            );
        }

        private void HandleUserControl()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, vertical, 0) * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // Draw movement path preview
            Gizmos.color = new Color(0.3f, 0.7f, 1f, 0.3f);

            switch (movementMode)
            {
                case MovementMode.CircularMotion:
                case MovementMode.FigureEight:
                case MovementMode.Zigzag:
                    Gizmos.DrawWireSphere(startPosition, circleRadius);
                    break;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Test All Presets")]
        private void TestAllPresets()
        {
            if (presets == null || presets.Length == 0)
            {
                LoadPresets();
            }

            StartCoroutine(CyclePresetCoroutine());
        }

        private System.Collections.IEnumerator CyclePresetCoroutine()
        {
            for (int i = 0; i < presets.Length; i++)
            {
                trailRenderer.SetPreset(presets[i]);
                Debug.Log($"Testing Preset {i + 1}/{presets.Length}: {presets[i].presetName}");
                yield return new UnityEngine.WaitForSeconds(3f);
                trailRenderer.ClearTrail();
                yield return new UnityEngine.WaitForSeconds(0.5f);
            }

            Debug.Log("Preset testing complete!");
        }
#endif
    }
}