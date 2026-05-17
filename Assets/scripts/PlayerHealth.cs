using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;

    private float currentHealth;

    private bool isDead = false;

<<<<<<< HEAD
    public Animator animator;
=======
    public Image healthFill;

    public GameObject deathPanel;

>>>>>>> 663a09a2a370b2cc06d0126a2e9c294f46afe5ac

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

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

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
        if (isDead) return;

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

        if (deathPanel != null)
            deathPanel.SetActive(true);

        // Pausar TODO el juego
        StartCoroutine(DeathRoutine());
    }


    IEnumerator DeathRoutine()
    {

        yield return new WaitForSecondsRealtime(2f);


        if (deathPanel != null)
            deathPanel.SetActive(false);

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}
