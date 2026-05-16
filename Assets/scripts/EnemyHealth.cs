using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    public float currentHealth;

    [Header("Bullet Reactions")]
    public bool explodeOnHit = false;

    public bool reflectBullets = false;

    [Header("Regeneration")]
    public float regenRate = 10f;

    public float regenDelay = 3f;

    private float lastDamageTime;

    private bool dead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (dead)
            return;

        RegenerateHealth();
    }

    public void TakeDamage(float damage)
    {
        if (dead)
            return;

        currentHealth -= damage;

        lastDamageTime = Time.time;

        // Evitar negativos
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void RegenerateHealth()
    {
        // Esperar tiempo sin daño
        if (Time.time >= lastDamageTime + regenDelay)
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += regenRate * Time.deltaTime;

                // Limitar máximo
                if (currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }
            }
        }
    }

    void Die()
    {
        dead = true;

        Debug.Log("ENEMY DIED");

        Destroy(gameObject);
    }
}
