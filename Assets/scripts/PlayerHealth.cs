using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("Player HP: " + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("PLAYER DIED");
    }
}
