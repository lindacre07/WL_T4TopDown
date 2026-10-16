using UnityEngine;

public class Tear : MonoBehaviour
{
    [SerializeField]
    private float range = 10f;

    private Vector3 _startPosition;
    private int     _layerIndex;
    private float   _bulletDamage = 4;

    void Start()
    {
        _startPosition = transform.position;
        _layerIndex    = LayerMask.NameToLayer("Enemy");
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

        if (other.IsTouchingLayers(_layerIndex))
        {
            Destroy(gameObject);

            if (other.gameObject.TryGetComponent<AH_Enemy>(out var enemy))
            {
                enemy.Damage(_bulletDamage);
            }
        }
    }
}
