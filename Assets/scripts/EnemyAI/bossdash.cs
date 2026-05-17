// BossDash.cs — Predictive Dash with ForceDash API

using UnityEngine;
using System.Collections;

public class BossDash : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Dash")]
    public float dashForce    = 35f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 3.5f;

    [Header("Trail (optional)")]
    public TrailRenderer dashTrail;

    private Rigidbody2D rb;
    private bool  dashing     = false;
    private float nextDashTime;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        // Autonomous timed dash (fallback when BossAI doesn't call ForceDash)
        if (player == null || dashing) return;
        if (Time.time < nextDashTime)  return;

        // Default autonomous: dash toward current player position
        ForceDash(player.position);
    }

    // ── Called by BossAI with a predicted target position ─────────
    public void ForceDash(Vector2 targetPosition)
    {
        if (dashing) return;
        if (Time.time < nextDashTime) return;

        nextDashTime = Time.time + dashCooldown;
        StartCoroutine(DashRoutine(targetPosition));
    }

    IEnumerator DashRoutine(Vector2 targetPosition)
    {
        dashing = true;

        if (dashTrail != null) dashTrail.emitting = true;

        Vector2 dashDir = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = dashDir * dashForce;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;

        if (dashTrail != null) dashTrail.emitting = false;

        dashing = false;
    }

    public bool IsDashing() => dashing;
}