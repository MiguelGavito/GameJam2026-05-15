using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    public Animator animator;
    
    // NUEVO: Referencia al dibujo del jugador
    private SpriteRenderer spriteRenderer; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Obtenemos el componente al iniciar el juego
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        
        Cursor.visible = true;
    }

    void Update()
    {
        // Movimiento WASD
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        // --- NUEVO: Lógica de volteo (Flip) ---
        // Si nos movemos a la izquierda (negativo), volteamos el sprite
        if (movement.x < 0)
        {
            spriteRenderer.flipX = false;
        }
        // Si nos movemos a la derecha (positivo), lo regresamos a la normalidad
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        // Nota: No ponemos "else" a secas para que si el jugador se detiene (movement.x == 0), 
        // mantenga la última dirección a la que estaba mirando.
        // ------------------------------------

        // 🎮 ANIMACIÓN
        float moving = movement != Vector2.zero ? 1f : 0f;
        animator.SetFloat("Moving", moving);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}