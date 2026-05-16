using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Explosion")]
    public float explosionRadius = 3f;

    public float damage = 40f;

    public float timer = 2f;

    public float explosionDuration = 0.3f;

    [Header("Knockback")]
    public float knockbackForce = 15f; 
    public float knockbackDuration = 0.15f;

    [Header("Movement")]
    public float reflectedBulletSpeed = 25f;

    [Header("Screen Shake (Base)")]
    public float baseShakeMagnitude = 0.15f;
    public float baseShakeDuration = 0.15f;

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

        // --- NUEVA LÓGICA DE SCREEN SHAKE PROGRESIVO ---
        if (CameraFollow.instance != null)
        {
            float finalMagnitude = baseShakeMagnitude;
            float finalDuration = baseShakeDuration;

            if (UpgradeManager.instance != null)
            {
                // Calculamos cuántos "niveles" de mejora tiene el jugador basándonos en los incrementos que definimos antes
                float damageUpgradesCount = UpgradeManager.instance.bonusDamage / 20f;
                float radiusUpgradesCount = UpgradeManager.instance.bonusExplosionRadius / 1.2f;

                // Cada nivel de daño o radio añade un poco más de violencia al temblor (+0.04 de magnitud)
                finalMagnitude += (damageUpgradesCount * 0.04f) + (radiusUpgradesCount * 0.04f);
                
                // Las explosiones más grandes también duran un par de milisegundos más
                finalDuration += (radiusUpgradesCount * 0.02f);
            }

            CameraFollow.instance.TriggerShake(finalDuration, finalMagnitude); 
        }
        // -----------------------------------------------

        // Desactivar colisión
        bulletCollider.enabled = false;

        // Cambiar color
        spriteRenderer.color = Color.red;

        // Expandir visualmente
        transform.localScale = Vector3.one * explosionRadius;

        // Buscar objetos dentro del radio
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // Daño enemigo
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Knockback
            KnockBackReceiver knockback = hit.GetComponent<KnockBackReceiver>();
            if (knockback != null)
            {
                Vector2 pushDirection = (hit.transform.position - transform.position).normalized;
                
                float finalForce = knockbackForce;
                if (UpgradeManager.instance != null)
                {
                    finalForce += UpgradeManager.instance.bonusKnockback;
                }

                knockback.ApplyKnockback(pushDirection, finalForce, knockbackDuration);
            }

            // Daño jugador
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
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