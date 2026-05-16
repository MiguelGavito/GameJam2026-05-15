using UnityEngine;

public class EnemyDasher : MonoBehaviour
{
    private Transform player;

    private PlayerHealth playerHealth;

    private Rigidbody2D rb;

    [Header("Movement")]
    public float moveSpeed = 2f;

    public float dashSpeed = 12f;

    public float dashCooldown = 3f;

    public float dashDuration = 0.4f;

    public float stopDistance = 1.5f;

    [Header("Combat")]
    public float damagePerSecond = 15f;

    private bool isDashing = false;

    private float dashTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            playerHealth =
                playerObject.GetComponent<PlayerHealth>();
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        dashTimer += Time.deltaTime;

        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        float distance =
            Vector2.Distance(rb.position, player.position);

        // DASH
        if (dashTimer >= dashCooldown)
        {
            StartCoroutine(Dash(direction));
            dashTimer = 0f;
        }

        // Movimiento normal
        if (!isDashing)
        {
            if (distance > stopDistance)
            {
                rb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;

                Attack();
            }
        }
    }

    System.Collections.IEnumerator Dash(Vector2 direction)
    {
        isDashing = true;

        rb.linearVelocity = direction * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }

    void Attack()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(
                damagePerSecond * Time.deltaTime
            );
        }
    }
}