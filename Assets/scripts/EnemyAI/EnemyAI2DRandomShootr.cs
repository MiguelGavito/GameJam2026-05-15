using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        RandomShooter,
        Sniper,
        Tactical
    }

    public EnemyType type;

    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;

    public float minDistance = 2f;
    public float maxDistance = 6f;

    private Rigidbody2D rb;
    private EnemyGun gun;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gun = GetComponentInChildren<EnemyGun>();

        gun.target = player;
    }

    void Update()
    {
        if (player == null) return;

        switch (type)
        {
            case EnemyType.RandomShooter:
                RandomShooter();
                break;

            case EnemyType.Sniper:
                Sniper();
                break;

            case EnemyType.Tactical:
                Tactical();
                break;
        }
    }

    // 🔫 1. loco, te sigue y dispara
    void RandomShooter()
    {
        MoveTowards(player.position);
    }

    // 🎯 2. sniper: se aleja y dispara rápido
    void Sniper()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < maxDistance)
        {
            MoveAway(player.position);
        }
    }

    // 🧠 3. táctico: mantiene distancia media
    void Tactical()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < minDistance)
        {
            MoveAway(player.position);
        }
        else if (dist > maxDistance)
        {
            MoveTowards(player.position);
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector2 dir = (target - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    void MoveAway(Vector3 target)
    {
        Vector2 dir = (transform.position - target).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }
}