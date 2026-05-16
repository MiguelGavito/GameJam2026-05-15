using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;

    private float currentHealth;

    private bool dead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (dead)
            return;

        currentHealth -= damage;

        Debug.Log("Enemy HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;

        Debug.Log("ENEMY DIED");

        Destroy(gameObject);
    }
}
