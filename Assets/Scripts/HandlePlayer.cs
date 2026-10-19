using Unity.Scripting.LifecycleManagement;

public partial class HandlePlayer : MonoBehaviour
{
    [SerializeField]
    private static float speed = 5;

    [SerializeField] private float baseSpeed = 5;
    [SerializeField] private float baseTearSpeed = 5;
    [SerializeField] private float baseTearCooldown =0.5f;
    [SerializeField] private int baseMaxHearts = 3;
    [SerializeField] private int baseTearDamage = 3;
    [SerializeField] private float baseTearRange = 5f;
    [SerializeField] private float baseTearSize = 1;
    
    private static float minTearRate = 0.05f;
    private GameObject bomb;

    [SerializeField]
    private GameObject tear;

    
    

    [SerializeField]
    private Transform tearPosition;

    [SerializeField]
    private static float tearCooldown = .5f;

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

    private static float moveSpeed = 5f;
    private static float fireRate = 0.5f;
    private static float tearSize = 1;
    private static float tearDamage = 1;
    private static float tearRange = 5f;
    private static float tearSpeed = 5;
    
    public static int hearts
    {
        get => _hearts;
        set => _hearts =
                Mathf.Min(Mathf.Max(0, value), _maxHearts);
    }
    public static float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    public static float FireRate
    {
        get => fireRate;
        set => fireRate = value;
    }
    public static float TearSize
    {
        get => tearSize;
        set => tearSize = value;
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

    private static WaitForSeconds _tearCooldown;

    private int _displayedHearts = 0;

    private bool _canCry = true;
    private bool _isDead = false;

    void Start()
    {
        _maxHearts = baseMaxHearts;
        speed = baseSpeed;
        tearCooldown = baseTearCooldown;
        tearSpeed = baseTearSpeed;
        tearDamage = baseTearDamage;
        _hearts    = startingHearts;
        tearSize = baseTearSize;
        tearRange = baseTearRange;
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

        Tear tearScript = newTear.GetComponent<Tear>();

        tearScript.Initialize(tearSpeed, tearDamage, tearRange, tearSize);

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

    public static void MoveSpeedChange(float speedUp)
    {
        speed += speedUp;
    }
    public static void FireRateChange(float fireRateUp)
    {
        tearCooldown = Mathf.Max(minTearRate, tearCooldown - fireRateUp);
        _tearCooldown = new WaitForSeconds(tearCooldown);
    }
    public static void TearSizeChange(float TearSizeUp)
    {
        tearSize += TearSizeUp;
    }
    public static void HealthChange(int HealthUp)
    {
        _maxHearts += HealthUp;
        _hearts += HealthUp;
    }
    public static void DamageChange(int DamageUp)
    {
        tearDamage += DamageUp;
    }

    public static void tearSpeedChange(float TearSpeedUp)
    {
        tearSpeed += TearSpeedUp;
    }

    public static void tearRangeChange(float tearRangeUp)
    {
        tearRange += tearRangeUp;
    }
}
