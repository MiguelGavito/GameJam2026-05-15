using UnityEngine;
using System.Collections;

public class BossDash : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Dash")]
    public float dashForce = 30f;

    public float dashDuration = 0.2f;

    public float dashCooldown = 3f;

    [Header("Effects")]
    public bool stopAfterDash = true;

    private Rigidbody2D rb;

    private bool dashing = false;

    private float nextDashTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        TryDash();
    }

    void TryDash()
    {
        if (dashing)
            return;

        if (Time.time < nextDashTime)
            return;

        nextDashTime =
            Time.time + dashCooldown;

        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        dashing = true;

        // 🎯 dirección al jugador
        Vector2 dashDirection =
            (player.position - transform.position)
            .normalized;

        // ⚡ velocidad instantánea
        rb.linearVelocity =
            dashDirection * dashForce;

        // ⏳ duración dash
        yield return new WaitForSeconds(
            dashDuration
        );

        // 🛑 detenerse
        if (stopAfterDash)
        {
            rb.linearVelocity =
                Vector2.zero;
        }

        dashing = false;
    }
}