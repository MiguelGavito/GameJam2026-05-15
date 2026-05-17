using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Header("Warning Line")]
    

    public float warningTime = 1f;

    public float lineLength = 20f;

    private bool isPreparingShot = false;

    [Header("References")]
    public GameObject bulletPrefab;

    public Transform firePoint;

    [Header("Shooting")]
    public float bulletSpeed = 15f;

    public float fireCooldown = 1f;

    [Header("Multi Shot")]
    public int bulletsPerShot = 1;

    public float spreadAngle = 10f;

    private float nextFireTime;

    [Header("Shoot Check")]
    public bool avoidFriendlyFire = true;

    public float shootCheckDistance = 20f;

    private Transform player;

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

        AimAtPlayer();

        if (CanShootPlayer())
{
            if (!isPreparingShot)
            {
            StartCoroutine(PrepareShot());
            }
}

    }

    void AimAtPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

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

    void Shoot()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime =
            Time.time + fireCooldown;

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

            Rigidbody2D bulletRb =
                bullet.GetComponent<Rigidbody2D>();

            bulletRb.linearVelocity =
                bullet.transform.right
                * bulletSpeed;
        }
    }
    System.Collections.IEnumerator PrepareShot()
{
    if (Time.time < nextFireTime)
        yield break;

    isPreparingShot = true;


    

    Vector3 startPos =
        firePoint.position;

    Vector3 endPos =
        firePoint.position
        + firePoint.right * lineLength;

    Debug.DrawRay(
    firePoint.position,
    firePoint.right * lineLength,
    Color.red,
    warningTime
);

    // ⏳ esperar antes de disparar
    yield return new WaitForSeconds(warningTime);

    // 💥 disparar
    Shoot();

  

    isPreparingShot = false;
}

   bool CanShootPlayer()
{
    if (!avoidFriendlyFire)
        return true;

    Vector2 direction =
        (player.position - firePoint.position)
        .normalized;

    RaycastHit2D[] hits =
        Physics2D.RaycastAll(
            firePoint.position,
            direction,
            shootCheckDistance
        );

    foreach (RaycastHit2D hit in hits)
    {
        // ignorarse a sí mismo
        if (hit.collider.gameObject == gameObject)
            continue;

        // si ve player primero → disparar
        if (hit.collider.CompareTag("Player"))
        {
            return true;
        }

        // si ve enemigo primero → no disparar
        if (hit.collider.GetComponent<EnemyHealth>() != null)
        {
            return false;
        }
    }

    return false;
}
}