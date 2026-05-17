// BossAI.cs — Tactical State Machine
// Boss positions itself deliberately, tells BossGun when it's safe to shoot,
// and only commits to attacking when it has a real opening.

using UnityEngine;

public class BossAI : MonoBehaviour
{
    public enum State { Orbit, Reposition, PrepareShot, Dash, Idle, Enraged }

    [Header("References")]
    public BossGun    bossGun;
    public BossDash   bossDash;
    public BossHealth bossHealth;

    [Header("Movement")]
    public float moveSpeed         = 4f;
    public float preferredDistance = 8f;   // ideal combat range
    public float tooCloseDistance  = 4f;   // retreat if inside this
    public float strafeSpeed       = 2.5f;

    [Header("Shoot Prep")]
    [Tooltip("Boss slows to this speed while lining up a shot")]
    public float prepMoveSpeed     = 1.2f;
    [Tooltip("How long the boss spends positioning before allowing gun to fire")]
    public float prepDuration      = 1.0f;

    [Header("State Durations")]
    public float orbitMinTime      = 1.5f;
    public float orbitMaxTime      = 2.8f;
    public float repositionTime    = 1.2f;
    public float idleMinTime       = 0.3f;
    public float idleMaxTime       = 0.7f;

    // internals
    private Rigidbody2D rb;
    private State       state      = State.Reposition;
    private float       stateTimer;
    private float       strafeSign = 1f;
    private float       strafeFlipTimer;

    private Vector2 lastPlayerPos;
    private Vector2 playerVelocity;

    private bool enragedTriggered = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) { lastPlayerPos = p.transform.position; }

        strafeSign      = Random.value > 0.5f ? 1f : -1f;
        strafeFlipTimer = Random.Range(1.5f, 3f);

        Enter(State.Reposition, repositionTime);
    }

    void Update()
    {
        if (bossHealth != null)
            TrackPlayerVelocity();

        CheckEnraged();
        stateTimer -= Time.deltaTime;
        RunState();
    }

    void TrackPlayerVelocity()
    {
        if (bossGun == null || bossGun.player == null) return;
        Vector2 pos = bossGun.player.position;
        playerVelocity = (pos - lastPlayerPos) / Time.deltaTime;
        lastPlayerPos  = pos;
    }

    Transform Player => bossGun != null ? bossGun.player : null;

    float DistToPlayer()
    {
        if (Player == null) return 999f;
        return Vector2.Distance(transform.position, Player.position);
    }

    // ── Enraged ───────────────────────────────────────────────────
    void CheckEnraged()
    {
        if (enragedTriggered || bossHealth == null) return;
        if (bossHealth.currentHealth / bossHealth.maxHealth <= 0.15f)
        {
            enragedTriggered = true;
            Enter(State.Enraged, 999f);
        }
    }

    // ── State machine ─────────────────────────────────────────────
    void RunState()
    {
        switch (state)
        {
            case State.Orbit:        StateOrbit();        break;
            case State.Reposition:   StateReposition();   break;
            case State.PrepareShot:  StatePrepareShot();  break;
            case State.Dash:         StateDash();         break;
            case State.Idle:         StateIdle();         break;
            case State.Enraged:      StateEnraged();      break;
        }
    }

    // ORBIT — strafe around the player at preferred distance
    void StateOrbit()
    {
        SetGunMoving(true);   // block gun from firing while moving

        strafeFlipTimer -= Time.deltaTime;
        if (strafeFlipTimer <= 0f)
        {
            strafeSign      = -strafeSign;
            strafeFlipTimer = Random.Range(1.5f, 3f);
        }

        if (Player == null) return;

        Vector2 toPlayer = ((Vector2)Player.position - (Vector2)transform.position);
        float   dist     = toPlayer.magnitude;
        Vector2 toNorm   = toPlayer.normalized;

        // radial correction
        Vector2 radial = Vector2.zero;
        if (dist > preferredDistance + 1.5f)      radial =  toNorm;
        else if (dist < tooCloseDistance)          radial = -toNorm * 1.5f;
        else if (dist < preferredDistance - 1.5f)  radial = -toNorm * 0.5f;

        // perpendicular strafe
        Vector2 perp = new Vector2(-toNorm.y, toNorm.x) * strafeSign;
        Vector2 move = (radial + perp * strafeSpeed).normalized;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, move * moveSpeed, 4f * Time.deltaTime);

        if (stateTimer <= 0f) ChooseNextState();
    }

    // REPOSITION — move to preferred distance directly
    void StateReposition()
    {
        SetGunMoving(true);

        if (Player != null)
        {
            Vector2 toPlayer = (Vector2)Player.position - (Vector2)transform.position;
            float   dist     = toPlayer.magnitude;
            Vector2 dir;

            if (dist < tooCloseDistance)
                dir = -toPlayer.normalized;
            else if (dist > preferredDistance + 2f)
                dir = toPlayer.normalized;
            else
                dir = Vector2.zero;

            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, dir * moveSpeed * 1.3f, 5f * Time.deltaTime);
        }

        if (stateTimer <= 0f) ChooseNextState();
    }

    // PREPARE SHOT — boss slows down and lets the gun build up patience
    void StatePrepareShot()
    {
        // Slow drift toward ideal range while gun is aiming
        SetGunMoving(false);   // gun is now ALLOWED to fire

        if (Player != null)
        {
            Vector2 toPlayer = (Vector2)Player.position - (Vector2)transform.position;
            float   dist     = toPlayer.magnitude;
            Vector2 correction = Vector2.zero;

            // Only do tiny corrections — don't move much
            if (dist < tooCloseDistance)
                correction = -toPlayer.normalized;
            else if (dist > preferredDistance + 3f)
                correction = toPlayer.normalized * 0.4f;

            rb.linearVelocity = Vector2.Lerp(
                rb.linearVelocity,
                correction * prepMoveSpeed,
                6f * Time.deltaTime
            );
        }

        // Wait for the gun to finish firing then transition
        bool gunDone = bossGun == null || !bossGun.IsShooting();
        if (stateTimer <= 0f && gunDone)
            ChooseNextState();
    }

    // DASH — brief pause after dash
    void StateDash()
    {
        SetGunMoving(true);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 6f * Time.deltaTime);
        if (stateTimer <= 0f) ChooseNextState();
    }

    // IDLE — short breath, then decide
    void StateIdle()
    {
        SetGunMoving(true);
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 8f * Time.deltaTime);
        if (stateTimer <= 0f) ChooseNextState();
    }

    // ENRAGED — constant aggressive approach + fire freely
    void StateEnraged()
    {
        SetGunMoving(false);   // gun can always fire

        strafeFlipTimer -= Time.deltaTime;
        if (strafeFlipTimer <= 0f)
        {
            strafeSign      = -strafeSign;
            strafeFlipTimer = Random.Range(0.6f, 1.5f);
        }

        if (Player == null) return;

        Vector2 toPlayer = ((Vector2)Player.position - (Vector2)transform.position).normalized;
        Vector2 perp     = new Vector2(-toPlayer.y, toPlayer.x) * strafeSign;
        Vector2 move     = (toPlayer * 0.7f + perp * 0.5f).normalized;

        rb.linearVelocity = Vector2.Lerp(
            rb.linearVelocity,
            move * moveSpeed * 1.7f,
            5f * Time.deltaTime
        );
    }

    // ── Decision logic ─────────────────────────────────────────────
    void ChooseNextState()
    {
        float dist    = DistToPlayer();
        float hpFrac  = bossHealth != null ? bossHealth.currentHealth / bossHealth.maxHealth : 1f;
        float aggr    = 1f - hpFrac;   // 0=full health, 1=dead

        // Must reposition if too far or too close
        if (dist < tooCloseDistance || Mathf.Abs(dist - preferredDistance) > 3.5f)
        {
            Enter(State.Reposition, repositionTime);
            return;
        }

        // Build weighted table
        float wOrbit   = 0.30f;
        float wShoot   = 0.35f + aggr * 0.25f;
        float wDash    = 0.15f + aggr * 0.10f;
        float wIdle    = Mathf.Max(0.05f, 0.20f - aggr * 0.18f);

        float total = wOrbit + wShoot + wDash + wIdle;
        float roll  = Random.value * total;

        if (roll < wOrbit)
        {
            Enter(State.Orbit, Random.Range(orbitMinTime, orbitMaxTime));
        }
        else if (roll < wOrbit + wShoot)
        {
            // PrepareShot: give it prepDuration to settle, then fire
            Enter(State.PrepareShot, prepDuration + 0.5f + aggr * 0.5f);
        }
        else if (roll < wOrbit + wShoot + wDash)
        {
            if (bossDash != null && Player != null)
            {
                Vector2 predicted = (Vector2)Player.position + playerVelocity * 0.25f;
                bossDash.ForceDash(predicted);
            }
            Enter(State.Dash, 1.0f);
        }
        else
        {
            Enter(State.Idle, Random.Range(idleMinTime, idleMaxTime));
        }
    }

    // ── Helpers ───────────────────────────────────────────────────
    void Enter(State next, float duration)
    {
        state      = next;
        stateTimer = duration;
    }

    // isMovingFast = true → gun will NOT fire (boss is repositioning)
    // isMovingFast = false → gun is free to accumulate aim patience and fire
    void SetGunMoving(bool moving)
    {
        if (bossGun != null) bossGun.isMovingFast = moving;
    }
}