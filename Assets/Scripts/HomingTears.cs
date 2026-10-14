public class HomingTears : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float liftime = 5f;
    [SerializeField] private float turnRate = 2f;

    private Rigidbody2D _rb;
    private Transform _target;
    private bool _hasHit;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindWithTag("Player").transform;
        Destroy(gameObject, liftime);
    }

    void FixedUpdate()
    {
        Vector2 toTarget = ((Vector2)_target.position - _rb.position).normalized;
        Vector2 newDir = Vector2.Lerp(_rb.linearVelocity.normalized, toTarget, turnRate * Time.deltaTime);
        _rb.linearVelocity = newDir.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (_hasHit) return;

        if (other.gameObject.CompareTag("Player"))
        {
            _hasHit = true;
            HandlePlayer.hearts--;
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            _hasHit = true;
            Destroy(gameObject);
        }
    }
}
