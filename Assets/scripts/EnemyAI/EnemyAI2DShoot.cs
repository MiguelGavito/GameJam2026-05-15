using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;

    // 📏 distancia mínima para detenerse
    public float stopDistance = 5f;

    [Header("Rotation")]
    public bool rotateToPlayer = true;

    private Rigidbody2D rb;

    [Header("Separation")]
    public float separationRadius = 2f;

    public float separationForce = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 🔥 buscar player automáticamente
        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // 🚫 evitar gravedad
        rb.gravityScale = 0f;
    }

    void Update()
    {
        if (player == null)
            return;

        FollowPlayer();

        if (rotateToPlayer)
        {
            RotateToPlayer();
        }
    }

    void FollowPlayer()
{
    float distance =
        Vector2.Distance(
            transform.position,
            player.position
        );

    // 🛑 detenerse cerca
    if (distance <= stopDistance)
    {
        rb.linearVelocity = Vector2.zero;
        return;
    }

    // 🎯 dirección al jugador
    Vector2 direction =
        (player.position - transform.position)
        .normalized;

    // 🔥 separación entre enemigos
    Collider2D[] nearby =
        Physics2D.OverlapCircleAll(
            transform.position,
            separationRadius
        );

    Vector2 separationDirection = Vector2.zero;

    foreach (Collider2D col in nearby)
    {
        if (col.gameObject == gameObject)
            continue;

        EnemyAI otherEnemy =
            col.GetComponent<EnemyAI>();

        if (otherEnemy != null)
        {
            Vector2 away =
                transform.position
                - col.transform.position;

            separationDirection +=
                away.normalized;
        }
    }

    // 🧠 combinar movimiento
    Vector2 finalDirection =
        direction +
        separationDirection * separationForce;

    finalDirection.Normalize();

    rb.linearVelocity =
        finalDirection * moveSpeed;
}

void RotateToPlayer()
{
    Vector2 direction =
        player.position - transform.position;

    float angle =
        Mathf.Atan2(direction.y, direction.x)
        * Mathf.Rad2Deg;

    transform.rotation =
        Quaternion.Euler(0f, 0f, angle);
}
}