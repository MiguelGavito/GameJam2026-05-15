// BossGun.cs — Calculated, patient shooter
// The boss WAITS for the right moment before firing.
// It tracks aim quality over time and only commits when conditions are right.

using UnityEngine;
using System.Collections;

public class BossGun : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform  firePoint;
    public Transform  player;

    [Header("Bullet")]
    public float bulletSpeed = 18f;

    [Header("Burst")]
    public float fireCooldown = 2.5f;
    public int   burstShots   = 4;
    public float burstDelay   = 0.14f;

    [Header("Spread (per shot)")]
    public int   bulletsPerShot = 1;
    public float spreadAngle    = 5f;

    [Header("Aim")]
    public float rotationSpeed = 4f;

    [Header("Patience")]
    [Tooltip("Boss must hold aim within tolerance for this many seconds before firing")]
    public float aimHoldRequired  = 0.8f;
    [Tooltip("Angle the aim must stay within")]
    public float aimTolerance     = 6f;
    [Tooltip("Boss won't shoot if player is farther than this")]
    public float maxShootDistance = 14f;
    [Tooltip("Set true by BossAI when boss is repositioning at speed")]
    public bool  isMovingFast     = false;

    [Header("Special Attack")]
    public bool  enableRadialAttack = true;
    public int   radialBulletCount  = 18;
    public float radialCooldown     = 9f;

    [Header("Prediction")]
    public float shotPredictionTime = 0.22f;

    // internals
    private float   nextFireTime;
    private float   nextRadialTime;
    private bool    shooting       = false;
    private float   steadyAimTimer = 0f;

    private Vector2 lastPlayerPos;
    private Vector2 playerVelocity;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) { player = p.transform; lastPlayerPos = p.transform.position; }
    }

    void Update()
    {
        if (player == null) return;

        TrackPlayerVelocity();
        RotateTowardPredicted();

        if (!shooting)
            UpdateAimPatience();

        if (enableRadialAttack)
            TryRadialAttack();
    }

    void TrackPlayerVelocity()
    {
        playerVelocity = ((Vector2)player.position - lastPlayerPos) / Time.deltaTime;
        lastPlayerPos  = player.position;
    }

    Vector2 PredictedPlayerPos()
    {
        float dist     = Vector2.Distance(firePoint.position, player.position);
        float leadTime = Mathf.Clamp(dist / bulletSpeed + shotPredictionTime, 0f, 0.6f);
        return (Vector2)player.position + playerVelocity * leadTime;
    }

    void RotateTowardPredicted()
    {
        Vector2   dir   = PredictedPlayerPos() - (Vector2)transform.position;
        float     angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0f, 0f, angle),
            rotationSpeed * Time.deltaTime
        );

        // --- NUEVO: Voltear el arma para que no quede de cabeza ---
        Vector3 localScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            localScale.y = -1f; // Voltea el arma en el eje Y
        }
        else
        {
            localScale.y = 1f;  // La devuelve a la normalidad
        }
        transform.localScale = localScale;
        // ----------------------------------------------------------
    }

    // The boss only fires after holding a clean aim for aimHoldRequired seconds
    // AND the player is within range AND the boss isn't rushing around.
    void UpdateAimPatience()
    {
        if (Time.time < nextFireTime || isMovingFast)
        {
            steadyAimTimer = 0f;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > maxShootDistance)
        {
            steadyAimTimer = 0f;
            return;
        }

        if (IsAimedAtPredicted())
        {
            steadyAimTimer += Time.deltaTime;

            if (steadyAimTimer >= aimHoldRequired)
            {
                steadyAimTimer = 0f;
                StartCoroutine(BurstFire());
            }
        }
        else
        {
            // Lost aim -> decay patience quickly
            steadyAimTimer = Mathf.Max(0f, steadyAimTimer - Time.deltaTime * 2f);
        }
    }

    bool IsAimedAtPredicted()
    {
        Vector2 dir   = (PredictedPlayerPos() - (Vector2)transform.position).normalized;
        float   angle = Vector2.Angle(transform.right, dir);
        return angle <= aimTolerance;
    }

    IEnumerator BurstFire()
    {
        shooting     = true;
        nextFireTime = Time.time + fireCooldown;

        for (int i = 0; i < burstShots; i++)
        {
            ShootSpread();
            yield return new WaitForSeconds(burstDelay);
        }

        shooting = false;
    }

    void ShootSpread()
    {
        Vector2 aimDir   = (PredictedPlayerPos() - (Vector2)firePoint.position).normalized;
        float   aimAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float      offset = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rot    = Quaternion.Euler(0f, 0f, aimAngle + offset);
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = bullet.transform.right * bulletSpeed;
        }
    }

    void TryRadialAttack()
    {
        if (Time.time < nextRadialTime) return;
        nextRadialTime = Time.time + radialCooldown;
        StartCoroutine(RadialRoutine());
    }

    IEnumerator RadialRoutine()
    {
        shooting = true;
        float step = 360f / radialBulletCount;

        for (int ring = 0; ring < 2; ring++)
        {
            float offset = ring * (step * 0.5f);
            for (int i = 0; i < radialBulletCount; i++)
            {
                Quaternion rot    = Quaternion.Euler(0f, 0f, i * step + offset);
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rot);
                bullet.GetComponent<Rigidbody2D>().linearVelocity =
                    bullet.transform.right * bulletSpeed * (0.85f + ring * 0.15f);
            }
            yield return new WaitForSeconds(0.2f);
        }

        shooting = false;
    }

    public bool  IsShooting()        => shooting;
    public float SteadyAimProgress() => steadyAimTimer / aimHoldRequired;

    void OnDrawGizmos()
    {
        if (firePoint == null || player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * 10f);
        Gizmos.color = Color.yellow;
        Vector2 pred = PredictedPlayerPos();
        Gizmos.DrawWireSphere(pred, 0.3f);
        Gizmos.DrawLine(firePoint.position, pred);
    }
}