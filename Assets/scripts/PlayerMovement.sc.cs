using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // INPUT WASD
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalizar para evitar velocidad diagonal exagerada
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // MOVER JUGADOR
        rb.linearVelocity = movement * moveSpeed;
    }
}
