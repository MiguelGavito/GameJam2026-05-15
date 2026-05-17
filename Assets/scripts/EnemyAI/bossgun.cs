using UnityEngine;
using System.Collections;

public class BossGun : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;

    public Transform firePoint;

    public Transform player;

    [Header("Basic Shooting")]
    public float bulletSpeed = 18f;

    public float fireCooldown = 2f;

    [Header("Burst")]
    public int burstShots = 5;

    public float burstDelay = 0.12f;

    [Header("Spread")]
    public int bulletsPerShot = 5;

    public float spreadAngle = 20f;

    [Header("Special Attack")]
    public bool enableRadialAttack = true;

    public int radialBulletCount = 20;

    public float radialCooldown = 8f;

    private float nextFireTime;

    private float nextRadialTime;

    private bool shooting = false;

    void Start()
    {
        GameObject playerObj =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        RotateToPlayer();

        if (!shooting)
        {
            StartCoroutine(BurstFire());
        }

        if (enableRadialAttack)
        {
            TryRadialAttack();
        }
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

    IEnumerator BurstFire()
    {
        if (Time.time < nextFireTime)
            yield break;

        shooting = true;

        nextFireTime =
            Time.time + fireCooldown;

        for (int i = 0; i < burstShots; i++)
        {
            ShootSpread();

            yield return new WaitForSeconds(
                burstDelay
            );
        }

        shooting = false;
    }

    void ShootSpread()
    {
        for (int i = 0; i < bulletsPerShot; i++)
        {
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

            Rigidbody2D rb =
                bullet.GetComponent<Rigidbody2D>();

            rb.linearVelocity =
                bullet.transform.right
                * bulletSpeed;
        }
    }

    void TryRadialAttack()
    {
        if (Time.time < nextRadialTime)
            return;

        nextRadialTime =
            Time.time + radialCooldown;

        RadialAttack();
    }

    void RadialAttack()
    {
        float angleStep =
            360f / radialBulletCount;

        for (int i = 0; i < radialBulletCount; i++)
        {
            float angle =
                i * angleStep;

            Quaternion rotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle
                );

            GameObject bullet =
                Instantiate(
                    bulletPrefab,
                    firePoint.position,
                    rotation
                );

            Rigidbody2D rb =
                bullet.GetComponent<Rigidbody2D>();

            rb.linearVelocity =
                bullet.transform.right
                * bulletSpeed;
        }
    }
}