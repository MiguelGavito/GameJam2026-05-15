using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;

    private float currentHealth;

    private bool isDead = false;

    public Image healthFill; 


    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        Debug.Log("Player HP: " + currentHealth);
    }
    void UpdateUI()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = currentHealth / maxHealth;
        }
    }

    public void TakeDamage(float damage)
    {
        // Evitar daño después de morir
        if (isDead)
            return;

        currentHealth -= damage;

        // Evitar negativos
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateUI();
        Debug.Log("Player HP: " + currentHealth);

        // Morir
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        Debug.Log("PLAYER DIED");

        // Desactivar movimiento del jugador
        GetComponent<PlayerMovement>().enabled = false;

        // Detener velocidad
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Pausar TODO el juego
        Time.timeScale = 0f;
    }
}
