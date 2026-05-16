using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 3f;

    public float damage = 40f;

    public float timer = 2f;

    public float explosionDuration = 0.3f;

    private bool exploded = false;

    private SpriteRenderer spriteRenderer;

    private Collider2D bulletCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        bulletCollider = GetComponent<Collider2D>();

        // Explota después del tiempo
        Invoke(nameof(Explode), timer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar player
        if (collision.CompareTag("Player"))
            return;

        // Explota al tocar enemigo
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

        // Quitar colisión física
        bulletCollider.enabled = false;

        // Cambiar color
        spriteRenderer.color = Color.red;

        // Hacer grande visualmente
        transform.localScale =
            Vector3.one * explosionRadius;

        // Buscar objetos dentro del radio
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

        foreach (Collider2D hit in hits)
        {
            // Daño enemigo
            EnemyHealth enemy =
                hit.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Daño jugador
            PlayerHealth player =
                hit.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        // Detener movimiento
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Destruir después de animación
        Destroy(gameObject, explosionDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
