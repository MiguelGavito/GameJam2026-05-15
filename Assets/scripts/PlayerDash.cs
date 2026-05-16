using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public KeyCode dashKey = KeyCode.Q;
    public float dashCooldown = 1f;

    [Header("Optional")]
    public bool useCooldown = true;

    private float nextDashTime = 0f;
    private Camera cam;
    private Rigidbody2D rb;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleDash();
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(dashKey))
        {
            if (useCooldown && Time.time < nextDashTime)
                return;

            DashToMouse();

            nextDashTime = Time.time + dashCooldown;
        }
    }

    void DashToMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // 🔥 mover jugador directamente
        transform.position = mousePos;

        // opcional: reset velocity para que no siga "empujado"
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}