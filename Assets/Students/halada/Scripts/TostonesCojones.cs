using System;
using UnityEngine;

public class TostonesCojones : MonoBehaviour
{
    private Vector2 _direction;
    private Transform _player;
    private Rigidbody2D _rb;
    private CircleCollider2D _burnRadius;
    
    [SerializeField] private float speed = 10f;
    private float _targetAngle, _currentAngle, _newAngle;
    private float _burnStacks;

    private bool _inAngleSpan;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _burnRadius = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _direction = _player.position - transform.position;
        _rb.linearVelocity = _direction * speed;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        _burnStacks += Time.deltaTime;

        if (_burnStacks >= 3)
        {
            // Damage player once, damage system not yet implemented
            _burnStacks = 0;
        }
    }
}
