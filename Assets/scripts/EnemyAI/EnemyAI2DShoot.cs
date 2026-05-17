using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 5f;

    [Header("Separation")]
    public float separationRadius = 2f;
    public float separationForce = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // Cambiamos Update a FixedUpdate porque estamos usando físicas (rb.linearVelocity)
    void FixedUpdate()
    {
        if (player == null)
            return;

        FollowPlayer();
    }

    void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // 🛑 detenerse cerca
        if (distance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 🎯 dirección al jugador
        Vector2 direction = (player.position - transform.position).normalized;

        // 🔥 separación entre enemigos
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        Vector2 separationDirection = Vector2.zero;

        foreach (Collider2D col in nearby)
        {
            if (col.gameObject == gameObject)
                continue;

            // Revisamos si el otro objeto también es un enemigo (tiene EnemyHealth)
            if (col.GetComponent<EnemyHealth>() != null)
            {
                Vector2 away = transform.position - col.transform.position;
                separationDirection += away.normalized;
            }
        }

        // 🧠 combinar movimiento
        Vector2 finalDirection = direction + separationDirection * separationForce;

        rb.linearVelocity = finalDirection.normalized * moveSpeed;
    }
}