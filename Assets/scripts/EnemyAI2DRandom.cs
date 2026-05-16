using UnityEngine;

public class EnemyAI2D : MonoBehaviour
{
    private Transform player;

    private PlayerHealth playerHealth;

    public float speed = 3f;

    public float stopDistance = 1.5f;

    [Header("Movement")]
    public float wobbleStrength = 1.5f;

    public float wobbleSpeed = 2f;

    private Rigidbody2D rb;

    private float randomOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            playerHealth =
                playerObject.GetComponent<PlayerHealth>();
        }

        // Cada enemigo tendrá movimiento distinto
        randomOffset = Random.Range(0f, 100f);
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 toPlayer =
            ((Vector2)player.position - rb.position).normalized;

        // Movimiento lateral
        Vector2 perpendicular =
            new Vector2(-toPlayer.y, toPlayer.x);

        // Oscilación
        float wobble =
            Mathf.Sin(Time.time * wobbleSpeed + randomOffset);

        Vector2 finalDirection =
            (toPlayer + perpendicular * wobble * wobbleStrength)
            .normalized;

        float distance =
            Vector2.Distance(rb.position, player.position);

        if (distance > stopDistance)
        {
            rb.linearVelocity = finalDirection * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            Attack();
        }
    }

    void Attack()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(10f * Time.deltaTime);
        }
    }
}