using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 3f;

    public float damage = 40f;

    public float timer = 2f;

    public float explosionDuration = 0.3f;

    [Header("Movement")]
    public float reflectedBulletSpeed = 25f;

    private bool exploded = false;

    private SpriteRenderer spriteRenderer;

    private Collider2D bulletCollider;

    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        bulletCollider =
            GetComponent<Collider2D>();

        rb =
            GetComponent<Rigidbody2D>();

        // Explosión automática
        Invoke(nameof(Explode), timer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar player
        if (collision.CompareTag("Player"))
            return;

        EnemyHealth enemy =
            collision.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            // REBOTAR bala
            if (enemy.reflectBullets)
            {
                ReflectBullet();

                return;
            }

            // EXPLOTAR instantáneamente
            if (enemy.explodeOnHit)
            {
                Explode();

                return;
            }
        }
    }

    void ReflectBullet()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        Vector2 direction =
            ((Vector2)player.transform.position
            - rb.position).normalized;

        // Hacer más rápida
        rb.linearVelocity =
            direction * reflectedBulletSpeed;

        // Rotar visualmente
        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        // Cambiar color visual
        spriteRenderer.color = Color.cyan;

        Debug.Log("BULLET REFLECTED");
    }

    void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log("BOOM");

        // Desactivar colisión
        bulletCollider.enabled = false;

        // Cambiar color
        spriteRenderer.color = Color.red;

        // Expandir visualmente
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
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Destruir después
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