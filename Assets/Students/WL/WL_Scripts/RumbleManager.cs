public class RumbleManager : MonoBehaviour
{
    public static RumbleManager Instance;

    private Gamepad pad;
    
    private Coroutine stopRumbleAfterTimeCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        //get reference to Gamepad
        pad = Gamepad.current;
        
        //if we have current Gamepad
        if (pad != null)
        {
            //start rumble
            pad.SetMotorSpeeds(lowFrequency, highFrequency);
            
            //stop rumble after certain amount of time
            stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(duration, pad));
        }
    }

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //once duration is over
        pad.SetMotorSpeeds(0f, 0f);
    }
}
