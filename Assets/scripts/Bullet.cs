using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 3f;

    public float damage = 40f;

    public float timer = 2f;

    private bool exploded = false;

    void Start()
    {
        // Explota automáticamente después de tiempo
        Invoke(nameof(Explode), timer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar player
        if (collision.CompareTag("Player"))
            return;

        // Si toca enemigo → explotar
        if (collision.GetComponent<EnemyHealth>() != null)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log("BOOM");

        // Buscar TODO dentro del radio
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

        foreach (Collider2D hit in hits)
        {
            // Daño a enemigos
            EnemyHealth enemy =
                hit.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Daño al jugador
            PlayerHealth player =
                hit.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    // Ver radio en editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
