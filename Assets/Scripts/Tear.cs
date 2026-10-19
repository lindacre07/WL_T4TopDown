using UnityEngine;

public class Tear : MonoBehaviour
{
    private float range;
    private float damage;
    private float speed;
    private Vector3 _startPosition;

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
            Destroy(gameObject);
        }
    }
}
