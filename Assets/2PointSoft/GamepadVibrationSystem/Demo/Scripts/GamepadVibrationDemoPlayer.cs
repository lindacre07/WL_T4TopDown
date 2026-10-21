using UnityEngine;
using UnityEngine.InputSystem;

namespace TwoPointSoft.GamepadVibration
{
    /// <summary>Small Input System movement controller used only by the demo scene.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public sealed class GamepadVibrationDemoPlayer : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float moveSpeed = 4f;
        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.isKinematic = true;
        }

        private void Update()
        {
            Vector2 input = Vector2.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            }

            if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().sqrMagnitude > input.sqrMagnitude)
            {
                input = Gamepad.current.leftStick.ReadValue();
            }

            Vector3 movement = new Vector3(input.x, 0f, input.y);
            if (movement.sqrMagnitude > 1f) movement.Normalize();
            transform.position += movement * (moveSpeed * Time.unscaledDeltaTime);
        }
    }
}
