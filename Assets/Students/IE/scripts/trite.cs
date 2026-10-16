using UnityEngine;
using System.Collections;

public class trite : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] private float jumpRadius = 1f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float MaxJumpCooldown = 2f;
    private GameObject player;
    private float jumpCooldown;
    private bool isJumping;

    void Start()
    {
        jumpCooldown = MaxJumpCooldown;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Trite_idle"))
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (isJumping)
            return;

        jumpCooldown -= Time.deltaTime;

        if (jumpCooldown <= 0f)
        {
            isJumping = true;
            anim.SetTrigger("Jump");
            StartCoroutine(WaitAndJump());
        }
    }

    IEnumerator WaitAndJump()
    {
        yield return new WaitForSeconds(0.2f);
        JumpRandom();
    }

    void JumpRandom()
    {
        jumpCooldown = MaxJumpCooldown;

        Vector2 direction = player.transform.position - transform.position;
        
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * jumpForce, ForceMode2D.Impulse);

        isJumping = false;
    }
}