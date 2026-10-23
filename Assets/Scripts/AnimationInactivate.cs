/// <summary>
/// Used for animation events
/// </summary>
public class AnimationInactivation : MonoBehaviour
{
    /// <summary>
    /// Inactive an object with an animation
    /// </summary>
    public void Inactivate()
    {
        gameObject.SetActive(false);
    }
}
