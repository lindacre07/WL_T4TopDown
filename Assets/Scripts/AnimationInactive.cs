/// <summary>
/// Used for animation events
/// </summary>
public class AnimationInactive : MonoBehaviour
{
    /// <summary>
    /// Inactivate an object with an animation
    /// </summary>
    public void Inactivate()
    {
        gameObject.SetActive(false);
    }
}
