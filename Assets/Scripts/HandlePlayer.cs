using Unity.Scripting.LifecycleManagement;

public partial class HandlePlayer : MonoBehaviour
{
    [SerializeField]
    private float speed = 5;

    [SerializeField]
    private GameObject bomb;

    [SerializeField]
    private GameObject tear;

    [SerializeField]
    private float tearSpeed = 5;

    [SerializeField]
    private Transform tearPosition;

    [SerializeField]
    private float tearCooldown = .5f;

    [SerializeField]
    private int startingHearts = 3;

    [SerializeField]
    private int maxHearts = 10;

    [SerializeField]
    private Transform heartDisplay;

    [SerializeField]
    private GameObject heart;

    [SerializeField]
    private GameObject halfHeart;

    public static int hearts
    {
        get => _hearts;
        set => _hearts =
                Mathf.Min(Mathf.Max(0, value), _maxHearts);
    }

    [AutoStaticsCleanup]
    private static int _hearts;

    [AutoStaticsCleanup]
    private static int _maxHearts = 10;

    private static readonly int SpeedXHash =
            Animator.StringToHash("Speed X");

    private static readonly int SpeedYHash =
            Animator.StringToHash("Speed Y");

    private static readonly int DieHash =
            Animator.StringToHash("Die");

    private Rigidbody2D    _rb;
    private SpriteRenderer _renderer;
    private Animator       _bodyAnimator;
    private Animator       _headAnimator;

    private Vector2 _direction     = Vector2.zero;
    private Vector2 _tearDirection = Vector2.zero;

    private WaitForSeconds _tearCooldown;

    private int _displayedHearts = 0;

    private bool _canCry = true;
    private bool _isDead = false;

    void Start()
    {
        _hearts    = startingHearts;
        _maxHearts = maxHearts;

        _rb = GetComponent<Rigidbody2D>();

        _bodyAnimator = GetComponent<Animator>();
        _headAnimator = transform
                .Find("Head")
                .GetComponent<Animator>();

        _renderer = transform
                .Find("Body")
                .GetComponent<SpriteRenderer>();

        _tearCooldown = new WaitForSeconds(tearCooldown);
    }

    void Update()
    {
        if (_isDead)
        {
            return;
        }

        if (_displayedHearts != _hearts)
        {
            DisplayHearts();
        }

        _headAnimator.SetFloat(SpeedXHash, _tearDirection.x);
        _headAnimator.SetFloat(SpeedYHash, _tearDirection.y);

        if (_canCry && _tearDirection != Vector2.zero)
        {
            Shoot();
        }

        _rb.linearVelocity = _direction * speed;
    }

    void OnMove(InputValue input)
    {
        if (_isDead)
        {
            return;
        }

        _direction = input.Get<Vector2>();

        _bodyAnimator.SetFloat(SpeedXHash, _direction.x);
        _bodyAnimator.SetFloat(SpeedYHash, _direction.y);

        _renderer.flipX = _direction.x < 0;
    }

    void OnShoot(InputValue v)
    {
        Vector2 direction = v.Get<Vector2>();

        _tearDirection =
                (direction.x, direction.y) switch
                {
                        (< -.5f, _) => Vector2.left,
                        (> .5f, _) => Vector2.right,
                        (_, < -.5f) => Vector2.down,
                        (_, > .5f) => Vector2.up,
                        _ => Vector2.zero
                };
    }

    private void Shoot()
    {
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        _canCry = false;

        yield return null;

        GameObject newTear = Instantiate(
                tear,
                tearPosition.position,
                tear.transform.rotation
        );

        newTear.GetComponent<Rigidbody2D>().linearVelocity =
                _tearDirection * tearSpeed;

        yield return _tearCooldown;

        _canCry = true;
    }

    void OnBomb(InputValue v)
    {
        if (_isDead)
        {
            return;
        }

        Instantiate(
                bomb,
                transform.Find("Bomb Position").position,
                bomb.transform.rotation);
    }

    void DisplayHearts(bool addHalfHeart = false)
    {
        _displayedHearts = _hearts;

        foreach (Transform child in heartDisplay)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < _hearts; i++)
        {
            Instantiate(heart, heartDisplay);
        }

        if (addHalfHeart)
        {
            Instantiate(halfHeart, heartDisplay);
        }

        if (hearts == 0)
        {
            _rb.linearVelocity = Vector2.zero;

            _bodyAnimator.SetTrigger(DieHash);

            _isDead = true;
        }
    }
}
