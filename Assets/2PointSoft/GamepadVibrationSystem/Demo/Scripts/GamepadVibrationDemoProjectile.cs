using UnityEngine;

namespace TwoPointSoft.GamepadVibration
{
    [AddComponentMenu("")]
    public sealed class GamepadVibrationDemoProjectile : MonoBehaviour
    {
        private Vector3 direction;
        private float speed;
        private float remainingLifetime;
        private GamepadVibration impactVibration;

        public void Initialize(Vector3 travelDirection, float travelSpeed, float lifetime, GamepadVibration impactFeedback)
        {
            direction = travelDirection.sqrMagnitude > 0f ? travelDirection.normalized : Vector3.forward;
            speed = Mathf.Max(0f, travelSpeed);
            remainingLifetime = Mathf.Max(0f, lifetime);
            impactVibration = impactFeedback;
        }

        private void Update()
        {
            transform.position += direction * (speed * Time.unscaledDeltaTime);
            remainingLifetime -= Time.unscaledDeltaTime;
            if (remainingLifetime <= 0f) Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other == null || other.isTrigger) return;
            if (impactVibration != null) impactVibration.Play();
            Destroy(gameObject);
        }
    }
}
