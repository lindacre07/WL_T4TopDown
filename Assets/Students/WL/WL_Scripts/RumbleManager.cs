using UnityEngine;

public class RumbleManager : MonoBehaviour
{
   public static RumbleManager instance;

   private Gamepad pad;

   private Coroutine stopRumbleAfterTimeCoroutine;
   
   private void Awake()
   {
      if (instance == null)
      {
         instance = this;
      }
   }

   public void RumblePulse(float lowFrequency, float highFrequency, float duration)
   {
      //get reference to Gamepad
      pad = Gamepad.current;

      //if we have current Gamepad
      if (pad != null)
      {
         //starts rumble
         pad.SetMotorSpeeds(lowFrequency, highFrequency);
      }
      
      //stops rumble after certain amount of time
      stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(duration, pad));
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
      pad.SetMotorSpeeds(0f,0f);
   }
}
