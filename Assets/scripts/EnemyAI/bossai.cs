using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 4f;

    public float preferredDistance = 8f;

    public float strafeSpeed = 3f;

    [Header("Dash")]
    public float dashForce = 20f;

    public float dashCooldown = 4f;

    private float nextDashTime;

    private Rigidbody2D rb;

    private Vector2 strafeDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // dirección aleatoria inicial
        strafeDirection =
            Random.value > 0.5f
            ? Vector2.left
            : Vector2.right;
    }

    void Update()
    {
        if (player == null)
            return;

        RotateToPlayer();

        MoveSmart();

        TryDash();
    }

    void MoveSmart()
    {
        Vector2 toPlayer =
            (player.position - transform.position)
            .normalized;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position
            );

        Vector2 movement = Vector2.zero;

        // 🧠 mantener distancia inteligente
        if (distance > preferredDistance)
        {
            movement += toPlayer;
        }
        else if (distance < preferredDistance - 2f)
        {
            movement -= toPlayer;
        }

        // 🔥 movimiento lateral
        Vector2 perpendicular =
            new Vector2(
                -toPlayer.y,
                toPlayer.x
            );

        movement +=
            perpendicular
            * strafeDirection.x
            * strafeSpeed;

        movement.Normalize();

        rb.linearVelocity =
            movement * moveSpeed;
    }

    void RotateToPlayer()
    {
        Vector2 direction =
            player.position - transform.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    void TryDash()
    {
        if (Time.time < nextDashTime)
            return;

        nextDashTime =
            Time.time + dashCooldown;

        Vector2 dashDirection =
            (player.position - transform.position)
            .normalized;

        rb.AddForce(
            dashDirection * dashForce,
            ForceMode2D.Impulse
        );
    }
}
