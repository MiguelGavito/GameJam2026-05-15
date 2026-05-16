using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;

    public Transform firePoint;

    public float bulletSpeed = 15f;

    [Header("Mouse")]
    public bool mouseUnlocked = false;

    void Update()
    {
        AimAtMouse();

        Shoot();

        ToggleMouse();
    }

    void AimAtMouse()
    {
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction =
            mousePosition - transform.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GameObject bullet =
                Instantiate(
                    bulletPrefab,
                    firePoint.position,
                    firePoint.rotation
                );

            Rigidbody2D rb =
                bullet.GetComponent<Rigidbody2D>();

            rb.linearVelocity =
                firePoint.right * bulletSpeed;
        }
    }

    void ToggleMouse()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            mouseUnlocked = !mouseUnlocked;

            if (mouseUnlocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
