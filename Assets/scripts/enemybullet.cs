using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 3f;
    public float damage = 25f;
    public float timer = 2f;
    public float explosionDuration = 0.3f;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.1f;

    [Header("Movement")]
    public float reflectedBulletSpeed = 20f;

    public float bulletSpeed = 15f;

    public float spreadAngle = 10f;

    [Header("Screen Shake (Base)")]
    public float baseShakeMagnitude = 0.1f;
    public float baseShakeDuration = 0.1f;

    private bool exploded = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D bulletCollider;
    private Rigidbody2D rb;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bulletCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        Invoke(nameof(Explode), timer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 💥 EXPLOTAR EN PLAYER
        PlayerHealth player =
            collision.collider.GetComponent<PlayerHealth>();

        if (player != null)
        {
            Explode();
            return;
        }

        // 🔄 REBOTAR EN ENEMIGOS
        EnemyHealth enemy =
            collision.collider.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            ReflectBullet(collision);
            return;
        }

        // 🔄 REBOTAR EN OBJETOS/PAREDES
        ReflectBullet(collision);
    }

    void ReflectBullet(Collision2D collision)
    {
        Vector2 normal =
            collision.contacts[0].normal;

        Vector2 newDir =
            Vector2.Reflect(
                rb.linearVelocity.normalized,
                normal
            );

        rb.linearVelocity =
            newDir * reflectedBulletSpeed;

        float angle =
            Mathf.Atan2(newDir.y, newDir.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);

        

        Debug.Log("ENEMY BULLET REFLECTED");
    }

    void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log("ENEMY BULLET BOOM");

        // 📳 SCREEN SHAKE
        if (CameraFollow.instance != null)
        {
            CameraFollow.instance.TriggerShake(
                baseShakeDuration,
                baseShakeMagnitude
            );
        }

        // ❌ desactivar collider
        bulletCollider.enabled = false;

        // 🔴 color explosión
        spriteRenderer.color = Color.red;

        // 📏 expandir visualmente
        transform.localScale =
            Vector3.one * explosionRadius;

        // 💥 detectar hits
        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

        foreach (Collider2D hit in hits)
        {
            // ❤️ daño jugador
            PlayerHealth player =
                hit.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            // 💨 knockback
            KnockBackReceiver knockback =
                hit.GetComponent<KnockBackReceiver>();

            if (knockback != null)
            {
                Vector2 pushDirection =
                    (hit.transform.position
                    - transform.position).normalized;

                knockback.ApplyKnockback(
                    pushDirection,
                    knockbackForce,
                    knockbackDuration
                );
            }
        }

        // 🛑 detener bala
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

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