using Cysharp.Threading.Tasks;
using Unity.Scripting.LifecycleManagement;

public partial class AH_HandlePlayer : MonoBehaviour
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
    private GameObject heartDisplay;

    [SerializeField]
    private GameObject heart;

    [SerializeField]
    private GameObject halfHeart;

    private static readonly int SpeedXHash =
            Animator.StringToHash("Speed X");

    private static readonly int SpeedYHash =
            Animator.StringToHash("Speed Y");

    private Rigidbody2D    _rb;
    private SpriteRenderer _renderer;
    private Animator       _bodyAnimator;
    private Animator       _headAnimator;

    private Vector2 _direction     = Vector2.zero;
    private Vector2 _tearDirection = Vector2.zero;
    
    [AutoStaticsCleanup]
    private static int _hearts;

    [AutoStaticsCleanup]
    private static int _maxHearts = 10;

    public static int hearts
    {
        get => _hearts;
        set => _hearts =
                Mathf.Min(Mathf.Max(0, value), _maxHearts);
    }

    private int  _displayedHearts = 0;
    private bool _canCry          = true;

    void Start()
    {
        _hearts    = startingHearts;
        _maxHearts = maxHearts;

        _rb           = GetComponent<Rigidbody2D>();
        _bodyAnimator = GetComponent<Animator>();
        _headAnimator = transform
                .Find("Head")
                .GetComponent<Animator>();
        _renderer = transform
                .Find("Body")
                .GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if (_displayedHearts != _hearts)
        {
            DisplayHearts();
        }

        _headAnimator.SetFloat(SpeedXHash, _tearDirection.x);
        _headAnimator.SetFloat(SpeedYHash, _tearDirection.y);

        if (_canCry && _tearDirection != Vector2.zero)
        {
            Shoot(_tearDirection);
        }

        _rb.linearVelocity = _direction * speed;
    }

    void OnMove(InputValue input)
    {
        _direction = input.Get<Vector2>();

        _bodyAnimator.SetFloat(SpeedXHash, _direction.x);
        _bodyAnimator.SetFloat(SpeedYHash, _direction.y);

        _renderer.flipX = _direction.x < 0;
    }

    void OnBomb(InputValue v)
    {
        Instantiate(
                bomb,
                transform.Find("Bomb Position").position,
                bomb.transform.rotation);
    }

    void OnShoot(InputValue v)
    {
        Vector2 direction = v.Get<Vector2>();
        Vector2 finalDirection = Vector2.zero;

        if (direction.x < -.5f)
        {
            finalDirection.x = -1;
            finalDirection.y = 0;
        }

        if (direction.x > .5f)
        {
            finalDirection.x = 1;
            finalDirection.y = 0;
        }

        if (direction.y < -.5f)
        {
            finalDirection.x = 0;
            finalDirection.y = -1;
        }

        if (direction.y > .5f)
        {
            finalDirection.x = 0;
            finalDirection.y = 1;
        }

        _tearDirection = finalDirection;
    }

    private void Shoot(Vector2 direction)
    {
        Cooldown();
    }

    async UniTask Cooldown()
    {
        _canCry = false;

        await UniTask.Yield();
        
        GameObject newTear = Instantiate(
            tear,
            tearPosition.position,
            tear.transform.rotation
        );

        newTear.GetComponent<Rigidbody2D>().linearVelocity =
            _tearDirection * tearSpeed;
        
        await UniTask.Delay(TimeSpan.FromSeconds(tearCooldown));

        _canCry = true;
    }

    void DisplayHearts(bool addHalfHeart = false)
    {
        _displayedHearts = _hearts;

        foreach (Transform child in heartDisplay.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < _hearts; i++)
        {
            var newHeart = Instantiate(heart);

            newHeart.transform.SetParent(heartDisplay.transform, false);
        }

        if (addHalfHeart)
        {
            var newHeart = Instantiate(halfHeart);

            newHeart.transform.SetParent(heartDisplay.transform, false);
        }
    }
}
