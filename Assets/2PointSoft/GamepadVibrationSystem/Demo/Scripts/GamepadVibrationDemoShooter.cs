using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Action demo: fires a projectile and plays recoil vibration.</summary>
    [AddComponentMenu("2 Point Soft/Demo/Gamepad Vibration Demo Shooter")]
    public sealed class GamepadVibrationDemoShooter : MonoBehaviour
    {
        [SerializeField] private Transform muzzle;
        [SerializeField] private GamepadVibration recoilVibration;
        [SerializeField] private GamepadVibration impactVibration;
        [SerializeField, Min(0f)] private float projectileSpeed = 12f;
        [SerializeField, Min(0f)] private float projectileLifetime = 3f;
        [SerializeField, Min(0f)] private float fireCooldown = 0.2f;

        private float nextFireTime;

        public Transform Muzzle { get => muzzle; set => muzzle = value; }
        public GamepadVibration RecoilVibration { get => recoilVibration; set => recoilVibration = value; }
        public GamepadVibration ImpactVibration { get => impactVibration; set => impactVibration = value; }

        private void Update()
        {
            bool firePressed = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
            firePressed |= Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            firePressed |= Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame;

            if (firePressed) Fire();
        }

        /// <summary>Fires one projectile. Can also be called by a UnityEvent.</summary>
        public void Fire()
        {
            if (Time.unscaledTime < nextFireTime) return;
            nextFireTime = Time.unscaledTime + fireCooldown;

            Transform origin = muzzle != null ? muzzle : transform;
            GameObject projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectileObject.name = "Demo Projectile";
            projectileObject.transform.position = origin.position;
            projectileObject.transform.localScale = Vector3.one * 0.25f;

            SphereCollider collider = projectileObject.GetComponent<SphereCollider>();
            collider.isTrigger = true;
            Rigidbody body = projectileObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.useGravity = false;

            Renderer renderer = projectileObject.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.75f, 0.1f);

            GamepadVibrationDemoProjectile projectile = projectileObject.AddComponent<GamepadVibrationDemoProjectile>();
            projectile.Initialize(origin.forward, projectileSpeed, projectileLifetime, impactVibration);

            if (recoilVibration != null) recoilVibration.Play();
        }
    }
}
