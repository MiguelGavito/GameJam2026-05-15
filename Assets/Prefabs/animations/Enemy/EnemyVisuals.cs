using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (rb.linearVelocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }

        if (animator != null)
        {
            float moving = rb.linearVelocity.sqrMagnitude > 0.1f ? 1f : 0f;
            animator.SetFloat("Moving", moving);
        }
    }
}