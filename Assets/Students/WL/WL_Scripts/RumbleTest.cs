public class RumbleTest : MonoBehaviour
{
    private void Update()
    {
        if (InputManager.instance.controls.Rumble.RumbleAction.WasPressedThisFrame())
        {
            //rumblings
            RumbleManager.Instance.RumblePulse(0.25f, 1f, 0.25f);
        }
    }
}
