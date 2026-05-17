using UnityEngine;

public class EnemyAI2D : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    private PlayerHealth playerHealth;

    [Header("Movement")]
    public float speed = 3f;
    public float stopDistance = 1.5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (UpgradeManager.instance != null)
        {
            speed += UpgradeManager.instance.bonusEnemySpeed;
        }

        // Obtener script de vida del jugador
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            Debug.Log("PLAYER NOT ASSIGNED");
            return;
        }

        // Dirección hacia jugador
        Vector2 direction = ((Vector2)player.position - rb.position).normalized;

        // Distancia al jugador
        float distance = Vector2.Distance(rb.position, player.position);

        // Perseguir jugador
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
            // Daño por segundo
            playerHealth.TakeDamage(10f * Time.deltaTime);
            Debug.Log("Enemy attacking");
        }
    }
}