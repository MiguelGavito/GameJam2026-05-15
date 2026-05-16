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
        spriteRenderer = GetComponent<SpriteRenderer>();
        bulletCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        
         // 🔥 REGISTRO EN MANAGER
     BulletManager.Instance.RegisterBullet(this);

        Invoke(nameof(Explode), timer);
    }


    // 🔥 CAMBIO IMPORTANTE: COLLISION en vez de TRIGGER
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
            return;

        EnemyHealth enemy = collision.collider.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            Debug.Log("TOUCHED ENEMY");

            if (enemy.reflectBullets)
            {
                ReflectBullet(collision);
                return;
            }

            if (enemy.explodeOnHit)
            {
                Debug.Log("EXPLODING");
                Explode();
                return;
            }
        }
    }

    void ReflectBullet(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;

        Vector2 newDir = Vector2.Reflect(rb.linearVelocity.normalized, normal);

        rb.linearVelocity = newDir * reflectedBulletSpeed;

        float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        spriteRenderer.color = Color.cyan;

        Debug.Log("BULLET REFLECTED");
    }

    void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        Debug.Log("BOOM");

        bulletCollider.enabled = false;

        spriteRenderer.color = Color.red;

        transform.localScale = Vector3.one * explosionRadius;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Destroy(gameObject, explosionDuration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void ForceExplode()
{
    Explode();
}

    void OnDestroy()
{
    if (BulletManager.Instance != null)
        BulletManager.Instance.UnregisterBullet(this);
}

    
}