using UnityEngine;
using System.Collections;

namespace TLD.Trail2D
{
    /// <summary>
    /// TLD Trail Effects Utility - Helper methods and advanced trail manipulation
    /// </summary>
    public class TLDTrailEffects : MonoBehaviour
    {
        private TLDTrailRenderer trailRenderer;

        private void Awake()
        {
            trailRenderer = GetComponent<TLDTrailRenderer>();
            if (trailRenderer == null)
            {
                Debug.LogError("TLDTrailEffects requires a TLDTrailRenderer component!");
            }
        }

        #region Quick Effect Methods

        /// <summary>
        /// Create a quick slash effect with automatic cleanup
        /// </summary>
        public void SlashEffect(Vector3 startPos, Vector3 endPos, float duration = 0.3f)
        {
            StartCoroutine(SlashEffectCoroutine(startPos, endPos, duration));
        }

        private IEnumerator SlashEffectCoroutine(Vector3 start, Vector3 end, float duration)
        {
            trailRenderer.ClearTrail();

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            trailRenderer.ClearTrail();
        }

        /// <summary>
        /// Create a pulse effect that briefly shows the trail
        /// </summary>
        public void PulseEffect(float intensity = 2f, float duration = 0.2f)
        {
            StartCoroutine(PulseEffectCoroutine(intensity, duration));
        }

        private IEnumerator PulseEffectCoroutine(float intensity, float duration)
        {
            // Store original gradient
            Gradient originalGradient = new Gradient();
            // Enhance colors temporarily

            yield return new WaitForSeconds(duration);

            // Restore original gradient
        }

        /// <summary>
        /// Create a dash trail effect
        /// </summary>
        public void DashEffect(Vector3 direction, float distance, float speed = 10f)
        {
            StartCoroutine(DashEffectCoroutine(direction, distance, speed));
        }

        private IEnumerator DashEffectCoroutine(Vector3 direction, float distance, float speed)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + direction.normalized * distance;

            float elapsed = 0f;
            float duration = distance / speed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
        }

        #endregion

        #region Trail Manipulation

        /// <summary>
        /// Fade out the trail gradually
        /// </summary>
        public void FadeOutTrail(float duration = 1f)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }

        private IEnumerator FadeOutCoroutine(float duration)
        {
            float elapsed = 0f;
            AnimationCurve originalOpacity = new AnimationCurve(); // Store original

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Gradually reduce opacity
                AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1 - t, 1, 0);
                trailRenderer.SetWidthCurve(fadeCurve);

                yield return null;
            }

            trailRenderer.ClearTrail();
        }

        /// <summary>
        /// Create a spiral trail effect
        /// </summary>
        public void SpiralEffect(float radius = 2f, float rotations = 3f, float duration = 2f)
        {
            StartCoroutine(SpiralEffectCoroutine(radius, rotations, duration));
        }

        private IEnumerator SpiralEffectCoroutine(float radius, float rotations, float duration)
        {
            Vector3 center = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float angle = t * rotations * Mathf.PI * 2f;
                float currentRadius = radius * (1f - t); // Spiral inward

                float x = Mathf.Cos(angle) * currentRadius;
                float y = Mathf.Sin(angle) * currentRadius;

                transform.position = center + new Vector3(x, y, 0);

                yield return null;
            }
        }

        /// <summary>
        /// Create an explosion-style radial trail
        /// </summary>
        public void ExplosionTrail(int segments = 8, float distance = 2f, float duration = 0.5f)
        {
            StartCoroutine(ExplosionTrailCoroutine(segments, distance, duration));
        }

        private IEnumerator ExplosionTrailCoroutine(int segments, float distance, float duration)
        {
            Vector3 center = transform.position;

            for (int i = 0; i < segments; i++)
            {
                float angle = (i / (float)segments) * Mathf.PI * 2f;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

                StartCoroutine(ExplosionSegmentCoroutine(center, direction, distance, duration));
            }

            yield return null;
        }

        private IEnumerator ExplosionSegmentCoroutine(Vector3 start, Vector3 direction, float distance, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.position = start + direction * distance * t;

                yield return null;
            }
        }

        #endregion

        #region Color Effects

        /// <summary>
        /// Flash the trail with a specific color
        /// </summary>
        public void FlashColor(Color flashColor, float duration = 0.2f)
        {
            StartCoroutine(FlashColorCoroutine(flashColor, duration));
        }

        private IEnumerator FlashColorCoroutine(Color flashColor, float duration)
        {
            // Store original gradient
            Gradient originalGradient = new Gradient();

            // Create flash gradient
            Gradient flashGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(flashColor, 0f);
            colorKeys[1] = new GradientColorKey(flashColor, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(0f, 1f);

            flashGradient.SetKeys(colorKeys, alphaKeys);
            trailRenderer.SetColorGradient(flashGradient);

            yield return new WaitForSeconds(duration);

            // Restore original (if stored)
            // trailRenderer.SetColorGradient(originalGradient);
        }

        /// <summary>
        /// Cycle through rainbow colors
        /// </summary>
        public void RainbowCycle(float cycleSpeed = 1f, float duration = 3f)
        {
            StartCoroutine(RainbowCycleCoroutine(cycleSpeed, duration));
        }

        private IEnumerator RainbowCycleCoroutine(float cycleSpeed, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float hue = Mathf.Repeat(elapsed * cycleSpeed, 1f);

                Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);

                Gradient gradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(rainbowColor, 0f);
                colorKeys[1] = new GradientColorKey(rainbowColor * 0.5f, 1f);

                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1f, 0f);
                alphaKeys[1] = new GradientAlphaKey(0f, 1f);

                gradient.SetKeys(colorKeys, alphaKeys);
                trailRenderer.SetColorGradient(gradient);

                yield return null;
            }
        }

        #endregion

        #region Motion Effects

        /// <summary>
        /// Create a figure-8 trail pattern
        /// </summary>
        public void FigureEightPattern(float size = 2f, float duration = 2f)
        {
            StartCoroutine(FigureEightCoroutine(size, duration));
        }

        private IEnumerator FigureEightCoroutine(float size, float duration)
        {
            Vector3 center = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = (elapsed / duration) * Mathf.PI * 2f;

                float x = Mathf.Sin(t) * size;
                float y = Mathf.Sin(t * 2f) * size * 0.5f;

                transform.position = center + new Vector3(x, y, 0);

                yield return null;
            }
        }

        /// <summary>
        /// Create a zigzag trail pattern
        /// </summary>
        public void ZigzagPattern(float amplitude = 2f, float frequency = 3f, float duration = 2f)
        {
            StartCoroutine(ZigzagCoroutine(amplitude, frequency, duration));
        }

        private IEnumerator ZigzagCoroutine(float amplitude, float frequency, float duration)
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float x = t * 5f; // Move forward
                float y = Mathf.Sin(t * frequency * Mathf.PI * 2f) * amplitude;

                transform.position = startPos + new Vector3(x, y, 0);

                yield return null;
            }
        }

        /// <summary>
        /// Create a circular orbit pattern
        /// </summary>
        public void OrbitPattern(Vector3 center, float radius = 2f, float speed = 1f, float duration = 3f)
        {
            StartCoroutine(OrbitCoroutine(center, radius, speed, duration));
        }

        private IEnumerator OrbitCoroutine(Vector3 center, float radius, float speed, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float angle = elapsed * speed;

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                transform.position = center + new Vector3(x, y, 0);

                yield return null;
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Enable/disable trail rendering
        /// </summary>
        public void SetTrailEnabled(bool enabled)
        {
            if (trailRenderer != null)
            {
                trailRenderer.enabled = enabled;
            }
        }

        /// <summary>
        /// Get the trail renderer component
        /// </summary>
        public TLDTrailRenderer GetTrailRenderer()
        {
            return trailRenderer;
        }

        /// <summary>
        /// Apply a preset with optional fade-in
        /// </summary>
        public void ApplyPresetWithFade(TLDTrailPreset preset, float fadeTime = 0.3f)
        {
            if (preset != null && trailRenderer != null)
            {
                trailRenderer.SetPreset(preset);
                StartCoroutine(FadeInCoroutine(fadeTime));
            }
        }

        private IEnumerator FadeInCoroutine(float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Gradually increase opacity
                AnimationCurve fadeCurve = AnimationCurve.Linear(0, t, 1, 0);
                trailRenderer.SetWidthCurve(fadeCurve);

                yield return null;
            }
        }

        #endregion

#if UNITY_EDITOR
        [ContextMenu("Test Slash Effect")]
        private void TestSlash()
        {
            SlashEffect(transform.position, transform.position + Vector3.right * 3f);
        }

        [ContextMenu("Test Spiral Effect")]
        private void TestSpiral()
        {
            SpiralEffect(2f, 3f, 2f);
        }

        [ContextMenu("Test Explosion Trail")]
        private void TestExplosion()
        {
            ExplosionTrail(8, 2f, 0.5f);
        }
#endif
    }
}