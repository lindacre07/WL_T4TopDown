using Cysharp.Threading.Tasks;
using Unity.Scripting.LifecycleManagement;

public partial class HandlePlayer : MonoBehaviour
{
    [SerializeField]
    private float baseSpeed = 5;

    [SerializeField]
    private float baseTearSpeed = 5;

    [SerializeField]
    private float baseTearCooldown = 0.5f;

    [SerializeField]
    private int baseMaxHearts = 3;

    [SerializeField]
    private int baseTearDamage = 3;

    [SerializeField]
    private float baseTearRange = 5f;

    [SerializeField]
    private float baseTearSize = 1;

    [SerializeField]
    private int baseTearAmount = 1;

    [SerializeField]
    private float extraTearAngleRange = 30f;

    [SerializeField]
    private GameObject bomb;

    [SerializeField]
    private int maxBombs = 5;

    [SerializeField]
    private GameObject tear;

    [SerializeField]
    private Transform tearPosition;

    [SerializeField]
    private float startingFireRate = 0.5f;

    [SerializeField]
    private int startingHearts = 3;

    [SerializeField]
    private int maxHearts = 10;

    [SerializeField]
    private GameObject heart;

    [SerializeField]
    private GameObject emptyHeart;

    [SerializeField]
    private GameObject halfHeart;

    private static readonly int SpeedXHash =
            Animator.StringToHash("Speed X");

    private static readonly int SpeedYHash =
            Animator.StringToHash("Speed Y");

    private static readonly int DieHash =
            Animator.StringToHash("Die");

    [AutoStaticsCleanup]
    private static float minTearRate = 0.05f;

    [AutoStaticsCleanup]
    private static float moveSpeed = 5f;

    [AutoStaticsCleanup]
    private static float fireRate = 0.5f;

    [AutoStaticsCleanup]
    private static float _tearSize = 1;

    [AutoStaticsCleanup]
    private static float _tearDamage = 1;

    [AutoStaticsCleanup]
    private static float _tearRange = 5f;

    [AutoStaticsCleanup]
    private static float _tearSpeed = 5;

    [AutoStaticsCleanup]
    private static int _tearAmount = 1;

    [AutoStaticsCleanup]
    private static float _speed = 5;

    [AutoStaticsCleanup]
    private static float _minTearRate = 0.05f;

    [AutoStaticsCleanup]
    private static float _tearCooldown = .5f;

    [AutoStaticsCleanup]
    private static float _moveSpeed = 5f;

    [AutoStaticsCleanup]
    private static float _fireRate;

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

    private Rigidbody2D    _rb;
    private SpriteRenderer _renderer;
    private Animator       _bodyAnimator;
    private Animator       _headAnimator;

    private Transform _fullHeartDisplay;
    private Transform _emptyHeartDisplay;

    private Vector2 _direction     = Vector2.zero;
    private Vector2 _tearDirection = Vector2.zero;

    private int _displayedHearts = 0;

    private bool _canCry = true;
    private bool _isDead = false;

    [AutoStaticsCleanup]
    private static List<GameObject> _cachedBomb = new();

    private int _currentBomb = 0;

    void Start()
    {
        for (int i = 0; i < maxBombs; i++)
        {
            var newBomb = Instantiate(bomb);

            _cachedBomb.Add(newBomb);
            newBomb.SetActive(false);
        }

        _maxHearts = baseMaxHearts;
        _speed     = baseSpeed;

        _tearSpeed    = baseTearSpeed;
        _tearDamage   = baseTearDamage;
        _hearts       = startingHearts;
        _tearSize     = baseTearSize;
        _tearRange    = baseTearRange;
        _maxHearts    = baseMaxHearts;
        _speed        = baseSpeed;
        _tearCooldown = baseTearCooldown;
        _tearSpeed    = baseTearSpeed;
        _hearts       = startingHearts;
        _tearAmount   = baseTearAmount;

        _rb = GetComponent<Rigidbody2D>();

        _bodyAnimator = GetComponent<Animator>();
        _headAnimator = transform
                .Find("Head")
                .GetComponent<Animator>();

        _renderer = transform
                .Find("Body")
                .GetComponent<SpriteRenderer>();

        _fullHeartDisplay  = GameObject.Find("Full Hearts").transform;
        _emptyHeartDisplay = GameObject.Find("Empty Hearts").transform;
    }

    void Update()
    {
        if (_displayedHearts != _hearts)
        {
            DisplayHearts();
        }

        if (_isDead)
        {
            return;
        }

        _headAnimator.SetFloat(SpeedXHash, _tearDirection.x);
        _headAnimator.SetFloat(SpeedYHash, _tearDirection.y);

        if (_canCry && _tearDirection != Vector2.zero)
        {
            Shoot();
        }

        _rb.linearVelocity = _direction * _speed;
    }

    void OnMove(InputValue input)
    {
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
        CoolDown();
    }

    async UniTask CoolDown()
    {
        _canCry = false;

        await UniTask.Yield();

        for (int i = 0; i < _tearAmount; i++)
        {
            float angle;
            if (i == 0)
                angle = 0f;
            else
                angle = Random.Range(-extraTearAngleRange, extraTearAngleRange);
            Vector2 direction = Quaternion.Euler(0f, 0f, angle) * _tearDirection;

            GameObject newTear = Instantiate(
                    tear,
                    tearPosition.position,
                    tear.transform.rotation
            );

            Tear tearScript = newTear.GetComponent<Tear>();
            tearScript.Initialize(_tearSpeed, _tearDamage, _tearRange, _tearSize);

            Rigidbody2D tearRb = newTear.GetComponent<Rigidbody2D>();
            tearRb.linearVelocity = direction * _tearSpeed;
        }

        await UniTask.WaitForSeconds(_tearCooldown);

        _canCry = true;
    }

    void OnBomb(InputValue v)
    {
        if (_cachedBomb[_currentBomb].activeSelf)
        {
            return;
        }

        _cachedBomb[_currentBomb].SetActive(true);
        _cachedBomb[_currentBomb].transform.position = transform.Find("Bomb Position").position;

        _currentBomb++;
        _currentBomb %= maxBombs;
    }

    void DisplayHearts(bool addHalfHeart = false)
    {
        _displayedHearts = _hearts;

        int lives = 0;
        int maxLives = 0;

        foreach (Transform child in _fullHeartDisplay)
        {
            child.gameObject.SetActive(lives < _hearts);

            lives++;
        }

        foreach (Transform child in _emptyHeartDisplay)
        {
            child.gameObject.SetActive(maxLives < _maxHearts);

            maxLives++;
        }

        /*
        if (addHalfHeart)
        {
            Instantiate(halfHeart, heartDisplay);
        }
        */

        if (hearts == 0)
        {
            _isDead = true;

            _rb.linearVelocity = Vector2.zero;

            _bodyAnimator.SetTrigger(DieHash);

            Destroy(GetComponent<PlayerInput>());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("flame"))
        {
            _hearts--;
        }
    }

    public static void MoveSpeedChange(float speedUp)
    {
        _speed += speedUp;
    }

    public static void FireRateChange(float fireRateUp)
    {
        _tearCooldown = Mathf.Max(_minTearRate, _tearCooldown - fireRateUp);
    }

    public static void TearSizeChange(float tearSizeUp)
    {
        _tearSize += tearSizeUp;
    }

    public static void HealthChange(int healthUp)
    {
        _maxHearts += healthUp;
        _hearts    += healthUp;
    }

    public static void DamageChange(int damageUp)
    {
        _tearDamage += damageUp;
    }

    public static void TearSpeedChange(float tearSpeedUp)
    {
        _tearSpeed += tearSpeedUp;
    }

    public static void TearRangeChange(float tearRangeUp)
    {
        _tearRange += tearRangeUp;
    }

    public static void TearAmountChange(int tearAmountUp)
    {
        _tearAmount += tearAmountUp;
    }
}
