using System.Collections;
using UnityEngine;

public class KnockBackReceiver : MonoBehaviour
{
    private Rigidbody2D rb;
    private MonoBehaviour aiScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        aiScript = GetComponent("EnemyAI2D") as MonoBehaviour ??         // Buscamos automáticamente cuál de todos tus scripts de IA tiene este enemigo específico
                
                   GetComponent("EnemyRandom") as MonoBehaviour ??
                   GetComponent("PlayerMovement") as MonoBehaviour ??
                   GetComponent("EnemyDasher") as MonoBehaviour ??
                   GetComponent("EnemyCoward") as MonoBehaviour ??
                   GetComponent("EnemyOrbiter") as MonoBehaviour;
    }

    public void ApplyKnockback(Vector2 pushDirection, float force, float duration)
    {
        // Detenemos cualquier retroceso anterior si recibe varios impactos rápidos
        StopAllCoroutines(); 
        StartCoroutine(KnockbackRoutine(pushDirection, force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration)
    {
        // 1. Apagamos la IA para que deje de caminar
        if (aiScript != null) aiScript.enabled = false;

        // 2. Aplicamos el empujón brusco
        rb.linearVelocity = direction * force;

        // 3. Esperamos el tiempo que dura el empujón
        yield return new WaitForSeconds(duration);

        // 4. Detenemos el deslizamiento y volvemos a encender la IA
        rb.linearVelocity = Vector2.zero;
        if (aiScript != null) aiScript.enabled = true;
    }
}