using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;

    private float currentHealth;

    private bool isDead = false;

    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("Player HP: " + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        // Evitar daño después de morir
        if (isDead)
            return;

        currentHealth -= damage;

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        // Evitar negativos
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

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
