using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;

    public Transform firePoint;

    [Header("Shooting")]
    public float bulletSpeed = 15f;

    public float fireCooldown = 0.5f;

    public int bulletsPerShot = 1;

    public float spreadAngle = 10f;

    [Header("Recoil")]
    public float recoilForce = 5f;

    private float nextFireTime;

    private Rigidbody2D playerRb;

    void Start()
    {
        playerRb =
            GetComponentInParent<Rigidbody2D>();

        Cursor.visible = true;
    }

    void Update()
    {
        AimAtMouse();

        Shoot();
    }

    void AimAtMouse()
    {
        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        Vector2 direction =
            mousePosition - transform.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        transform.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        // Cooldown
        if (Time.time < nextFireTime)
            return;

        // Click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            nextFireTime =
                Time.time + fireCooldown;

            // Disparar múltiples balas
            for (int i = 0; i < bulletsPerShot; i++)
            {
                // Spread aleatorio
                float randomAngle =
                    Random.Range(
                        -spreadAngle,
                        spreadAngle
                    );

                Quaternion spreadRotation =
                    firePoint.rotation *
                    Quaternion.Euler(
                        0f,
                        0f,
                        randomAngle
                    );

                GameObject bullet =
                    Instantiate(
                        bulletPrefab,
                        firePoint.position,
                        spreadRotation
                    );

                Rigidbody2D bulletRb =
                    bullet.GetComponent<Rigidbody2D>();

                bulletRb.linearVelocity =
                    bullet.transform.right
                    * bulletSpeed;
            }

            ApplyRecoil();
        }
    }

    void ApplyRecoil()
    {
        if (playerRb == null)
            return;

        // Dirección opuesta al arma
        Vector2 recoilDirection =
            -firePoint.right;

        playerRb.AddForce(
            recoilDirection * recoilForce,
            ForceMode2D.Impulse
        );
    }
}