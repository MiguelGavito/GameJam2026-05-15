// BossHealth.cs — Phase System + Enraged Trigger

using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth     = 1000f;
    public float currentHealth;

    [Header("Phases")]
    public bool phase2;
    public bool phase3;
    public bool enraged;

    [Header("Phase Thresholds")]
    public float phase2Percent  = 0.70f;
    public float phase3Percent  = 0.30f;
    public float enragedPercent = 0.15f;

    [Header("References")]
    public BossGun    bossGun;
    public BossDash   bossDash;
    // BossAI reads bossHealth directly — no circular ref needed here

    private bool dead = false;
    public Animator animator;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (dead) return;

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        CheckPhases();
        if (currentHealth <= 0f) Die();
    }

    void CheckPhases()
    {
        float pct = currentHealth / maxHealth;

        if (!phase2 && pct <= phase2Percent)
        {
            phase2 = true;
            EnterPhase2();
        }

        if (!phase3 && pct <= phase3Percent)
        {
            phase3 = true;
            EnterPhase3();
        }

        if (!enraged && pct <= enragedPercent)
        {
            enraged = true;
            EnterEnraged();
        }
    }

    // ── Phase 2: faster burst fire ────────────────────────────────
    void EnterPhase2()
    {
        if (bossGun == null) return;
        bossGun.fireCooldown *= 0.75f;
        bossGun.burstShots   += 2;
        bossGun.rotationSpeed += 2f;        // aims faster too
    }

    // ── Phase 3: shotgun spread + quicker dashes ──────────────────
    void EnterPhase3()
    {
        if (bossGun != null)
        {
            bossGun.bulletsPerShot += 3;
            bossGun.spreadAngle    += 15f;
            bossGun.fireCooldown   *= 0.85f;
        }

        if (bossDash != null)
        {
            bossDash.dashCooldown *= 0.65f;
            bossDash.dashForce    += 10f;
        }
    }

    // ── Enraged: minimum cooldowns, maximum aggression ────────────
    void EnterEnraged()
    {
        if (bossGun != null)
        {
            bossGun.fireCooldown        = Mathf.Min(bossGun.fireCooldown, 0.9f);
            bossGun.burstShots          = Mathf.Max(bossGun.burstShots, 6);
            bossGun.shotPredictionTime  = 0.35f;   // sharper lead
            bossGun.radialCooldown     *= 0.6f;
        }

        if (bossDash != null)
        {
            bossDash.dashCooldown = Mathf.Min(bossDash.dashCooldown, 1.8f);
            bossDash.dashForce    = Mathf.Max(bossDash.dashForce, 45f);
        }
    }

    void Die()
    {
        dead = true;
        // Add death VFX / event here
        Destroy(gameObject);
    }
}