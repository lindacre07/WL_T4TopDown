using UnityEngine;

public abstract class AH_Enemy : MonoBehaviour
{
    private Collider2D _collider;
    private float _health = 10f;
    
    public float Health
    {
        get => _health; 
        set => _health = value;
    }

    public void Damage(float damage)
    {
        _health -= damage;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Tear"))
        {
            return;
        }
        
        _health--;

        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
