using UnityEngine;

public class Tear : MonoBehaviour
{
    private float range;
    private float damage;
    private float speed;
    private Vector3 _startPosition;
    [SerializeField]
    private LayerMask _layerIndex;
    private float   _bulletDamage = 4;

    public void Initialize(float tearSpeed, float tearDamage, float tearRange, float tearSize)
    {
        speed = tearSpeed;
        damage = tearDamage;
        range = tearRange;
        transform.localScale *= tearSize;
        _startPosition = transform.position;
    }
    
    void Start()
    {
        _startPosition = transform.position;
        print(range);
    }

    void Update()
    {
        if (Vector3.Distance(_startPosition, transform.position) > range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
        {
            print("wqfwef");
            Destroy(gameObject);
        }

        if (gameObject.GetComponent<Collider2D>().IsTouchingLayers(_layerIndex))
        {
            print(other.gameObject.name);
       
            Destroy(gameObject);

            if (other.gameObject.TryGetComponent<AH_Enemy>(out var enemy))
            {
                enemy.Damage(_bulletDamage);
            }
        }
    }
}
