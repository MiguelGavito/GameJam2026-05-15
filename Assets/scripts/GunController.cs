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
        if (Time.timeScale == 0f) return;
        AimAtMouse();

        Shoot();
    }

    void AimAtMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;
        }
        transform.localScale = localScale;
    }

    void Shoot()
    {
    // Cooldown modificado por la mejora
        float actualCooldown = fireCooldown;
        if (UpgradeManager.instance != null)
        {
        // Reducimos el cooldown, pero ponemos un límite mínimo (ej: 0.05s) para que no sea infinito
            actualCooldown = Mathf.Max(0.05f, fireCooldown - UpgradeManager.instance.bonusCooldownReduction);
        }

        if (Time.time < nextFireTime)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            nextFireTime = Time.time + actualCooldown;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                float randomAngle = Random.Range(-spreadAngle, spreadAngle);
                Quaternion spreadRotation = firePoint.rotation * Quaternion.Euler(0f, 0f, randomAngle);

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, spreadRotation);
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                bulletRb.linearVelocity = bullet.transform.right * bulletSpeed;

                // Aplicar mejoras de daño y radio a la bala
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null && UpgradeManager.instance != null)
                {
                    bulletScript.explosionRadius += UpgradeManager.instance.bonusExplosionRadius;
                    bulletScript.damage += UpgradeManager.instance.bonusDamage;
                }
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