using UnityEngine;

public class EnemyAI2D : MonoBehaviour
{
    public Transform player;

    public float speed = 3f;
    public float stopDistance = 1.5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(transform.position, player.position);

        // SI ESTÁ LEJOS → PERSEGUIR
        if (distance > stopDistance)
        {
            Vector2 direction =
                (player.position - transform.position).normalized;

            rb.linearVelocity = direction * speed;
        }
        else
        {
            // DETENERSE CERCA
            rb.linearVelocity = Vector2.zero;

            Attack();
        }
    }

    void Attack()
    {
        Debug.Log("Enemy attacking");
    }
}
