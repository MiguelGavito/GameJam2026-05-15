using UnityEngine;

public class EnemyCoward : MonoBehaviour
{
    private Transform player;

    private PlayerHealth playerHealth;

    private EnemyHealth enemyHealth;

    private Rigidbody2D rb;

    [Header("Movement")]
    public float speed = 3f;

    public float stopDistance = 1.5f;

    public float fleeHealth = 30f;

    [Header("Combat")]
    public float damagePerSecond = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        enemyHealth = GetComponent<EnemyHealth>();

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

        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        float distance =
            Vector2.Distance(rb.position, player.position);

        // HUIR SI TIENE POCA VIDA
        if (enemyHealth.currentHealth <= fleeHealth)
        {
            direction = -direction;
        }

        if (distance > stopDistance)
        {
            rb.linearVelocity = direction * speed;
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