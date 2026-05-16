using UnityEngine;
using UnityEngine.InputSystem; // Importante: Añadimos la librería del nuevo sistema

public class PlayerMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 5f;

    [Header("Componentes")]
    public Rigidbody2D rb;

    private Vector2 movement;

    void Update()
    {
        movement = Vector2.zero;

        // Leemos directamente el estado del teclado actual
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) movement.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) movement.y -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) movement.x += 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) movement.x -= 1;
        }

        // Normalizamos para evitar que el movimiento diagonal sea más rápido
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Aplicamos la física en FixedUpdate
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}