using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] 
    private float hurtCooldown = 2f;

    private float _nextHurtTime;

    void OnCollisionEnter2D(Collision2D collision) => TryHurt(collision);
    void OnCollisionStay2D(Collision2D collision) => TryHurt(collision);
    void OnCollisionExit2D(Collision2D collision) => TryHurt(collision);
    
    void TryHurt(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (Time.time < _nextHurtTime)
        {
            return;
        }
        
        _nextHurtTime = Time.time + hurtCooldown;
        
        HandlePlayer.hearts--;
    }
}
