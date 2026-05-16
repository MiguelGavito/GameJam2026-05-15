using UnityEngine;

public class EnemyOrbiter : MonoBehaviour
{
    private Transform player;

    private PlayerHealth playerHealth;

    private Rigidbody2D rb;

    [Header("Movement")]
    public float speed = 3f;

    public float stopDistance = 2f;

    public float orbitStrength = 2f;

    [Header("Combat")]
    public float damagePerSecond = 10f;

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
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        Vector2 toPlayer =
            ((Vector2)player.position - rb.position).normalized;

        // Movimiento circular
        Vector2 orbit =
            new Vector2(-toPlayer.y, toPlayer.x);

        Vector2 finalDirection =
            (toPlayer + orbit * orbitStrength).normalized;

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
            playerHealth.TakeDamage(
                damagePerSecond * Time.deltaTime
            );
        }
    }
}