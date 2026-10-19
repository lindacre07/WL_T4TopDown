using UnityEngine;

// ReSharper disable once InconsistentNaming
public abstract class AH_Enemy : MonoBehaviour
{
    private float _health = 10f;

    // ReSharper disable once InconsistentNaming
    public float Health
    {
        get => _health;
        set => _health = value;
    }

    public void Damage(float damage)
    {
        _health -= damage;
    }

    protected virtual void Update()
    {
        if (_health <= 0f)
        {
            Destroy(gameObject);
            // Spawn whatever drop
        }
    }
}
