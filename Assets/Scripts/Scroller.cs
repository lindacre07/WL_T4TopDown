public class Scroller : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;

    private Vector2 _change = Vector2.zero;

    void Update()
    {
        Camera
                .main
                .gameObject
                .transform
                .Translate(_change * (speed * Time.deltaTime));
    }

    void OnMove(InputValue v)
    {
        _change = v.Get<Vector2>();
    }
}
