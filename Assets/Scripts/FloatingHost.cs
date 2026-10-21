public class FloatingHost : MonoBehaviour
{
    [SerializeField]
    private float patrolSpeed = 3f;

    [SerializeField]
    private float chargeSpeed = 7f;

    [SerializeField]
    private float shotSpeed = 4f;

    [SerializeField]
    private float alignTolerance = 0.3f;

    [SerializeField]
    private float coolDownTime = 0.5f;

    [SerializeField]
    private float pauseTime = 1f;

    [SerializeField]
    private float detectionRange = 5f;

    [SerializeField]
    private GameObject homingShot;

    [SerializeField]
    public LayerMask obstacleMask;

    private static int ShootHash = Animator.StringToHash("Shoot");

    enum State
    {
        Patrol,
        Charge,
        Pause
    }

    private Rigidbody2D _rb;
    private Transform _player;
    private Animator _anim;

    private State   _state   = State.Patrol;
    private Vector2 _moveDir = Vector2.right;
    private Vector2 _chargeDir;
    private WaitForSeconds _coolDown;
    private WaitForSeconds _pause;
    
    

    void Start()
    {
        _rb     = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _anim = GetComponent<Animator>();
        
        _coolDown = new WaitForSeconds(coolDownTime);
        _pause = new WaitForSeconds(pauseTime);
    }

    void FixedUpdate()
    {
        switch (_state)
        {
            case State.Patrol:
                _rb.linearVelocity = _moveDir * patrolSpeed;
                CheckForPlayer();
                break;
            case State.Charge:
                _rb.linearVelocity = _chargeDir * chargeSpeed;
                break;
            case State.Pause:
                _rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void CheckForPlayer()
    {
        Vector2 diff = (Vector2)_player.position - _rb.position;
        float diffMag = diff.magnitude;
        if (diffMag * diffMag > detectionRange * detectionRange)
        {
            return;
        }

        Vector2 dir;

        if (Mathf.Abs(diff.x) < alignTolerance)
        {
            dir = new Vector2(0, Mathf.Sign(diff.y));
        }
        else if (Mathf.Abs(diff.y) < alignTolerance)
        {
            dir = new Vector2(Mathf.Sign(diff.x), 0);
        }
        else
        {
            return;
        }

        if (Physics2D.Raycast(_rb.position, dir, diffMag, obstacleMask))
        {
            return;
        }

        _chargeDir = dir;
        _state = State.Charge;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (_state == State.Patrol)
        {
            _moveDir = new Vector2(-_moveDir.y, _moveDir.x);
        }
        else if (_state == State.Charge)
        {
            StartCoroutine(AfterCharge());
        }
    }

    IEnumerator AfterCharge()
    {
        _state = State.Pause;
        _anim.SetTrigger(ShootHash);
        yield return _pause;
        
        Fire();
        yield return _coolDown;

        _moveDir = new Vector2(-_chargeDir.y, _chargeDir.x);
        _state   = State.Patrol;
    }

    void Fire()
    {
        Vector2 dir = ((Vector2)_player.position - _rb.position).normalized;
        GameObject shot = Instantiate(homingShot, _rb.position, Quaternion.identity);
        shot.GetComponent<Rigidbody2D>().linearVelocity = dir * shotSpeed;
    }
}
