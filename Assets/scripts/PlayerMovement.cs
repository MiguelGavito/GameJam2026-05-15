using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Cursor.visible = true;
    }

    void Update()
    {
        // Movimiento WASD
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        // 🎮 ANIMACIÓN
        float moving = movement != Vector2.zero ? 1f : 0f;
        animator.SetFloat("Moving", moving);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}