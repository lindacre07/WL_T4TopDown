using UnityEngine;

public class RumbleTest : MonoBehaviour
{
    private void Update()
    {
        if (InputManager.instance.controls.Rumble.RumbleAction.WasCompletedThisFrame())
        {
            //rumble
            RumbleManager.instance.RumblePulse(0.25f, 1f, 0.25f);
        }
    }
}
