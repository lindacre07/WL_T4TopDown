using Unity.Tilemaps;

public class PooBottle : AH_Enemy
{
    [SerializeField]
    private float speed = 2;

    [SerializeField]
    private float sightRange = 4;

    [SerializeField]
    private int maxRayChecks = 3;

    private RaycastHit2D[] _hits;

    private Rigidbody2D _rb;
    private GameObject  _player;

    void Start()
    {
        _hits = new RaycastHit2D[maxRayChecks];

        _rb     = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    protected override void Update()
    {
        base.Update();
        
        if (!_player)
        {
            return;
        }

        Vector2 direction =
                _player.transform.position -
                transform.position;

        _rb.linearVelocity = Vector2.zero;

        if (direction.sqrMagnitude < sightRange)
        {
            Physics2D.CircleCast(
                    transform.position,
                    .5f,
                    direction,
                    new ContactFilter2D(),
                    _hits,
                    sightRange);

            foreach (var hit in _hits)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    continue;
                }

                if (hit
                    .collider
                    .gameObject
                    .CompareTag("Player"))
                {
                    break;
                }
                else
                {
                    return;
                }
            }

            _rb.linearVelocity =
                    direction.normalized
                    * speed;
        }
    }
}
