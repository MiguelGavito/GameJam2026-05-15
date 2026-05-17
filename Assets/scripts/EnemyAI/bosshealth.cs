using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 1000f;

    public float currentHealth;

    [Header("Phases")]
    public bool phase2;

    public bool phase3;

    [Header("Phase Thresholds")]
    public float phase2Percent = 0.7f;

    public float phase3Percent = 0.3f;

    [Header("References")]
    public BossGun bossGun;

    public BossDash bossDash;

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

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log(
            "BOSS HP: " + currentHealth
        );

        CheckPhases();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void CheckPhases()
    {
        float hpPercent =
            currentHealth / maxHealth;

        // 🔥 PHASE 2
        if (!phase2 &&
            hpPercent <= phase2Percent)
        {
            phase2 = true;

            EnterPhase2();
        }

        // 🔥 PHASE 3
        if (!phase3 &&
            hpPercent <= phase3Percent)
        {
            phase3 = true;

            EnterPhase3();
        }
    }

    void EnterPhase2()
    {
        Debug.Log("PHASE 2");

        // ⚡ más agresivo
        bossGun.fireCooldown *= 0.7f;

        bossGun.burstShots += 2;

        bossDash.dashCooldown *= 0.8f;
    }

    void EnterPhase3()
    {
        Debug.Log("PHASE 3");

        // 💀 caos total
        bossGun.bulletsPerShot += 3;

        bossGun.spreadAngle += 15f;

        bossDash.dashForce += 10f;

        bossDash.dashCooldown *= 0.6f;
    }

    void Die()
    {
        dead = true;

        Debug.Log("BOSS DEAD");

        Destroy(gameObject);
    }
}