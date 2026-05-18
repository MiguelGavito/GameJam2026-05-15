using UnityEngine;

public class NearestEnemyPointer : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject arrowVisual; 
    
    // NUEVO: Referencia directa al componente que pinta la flecha
    public SpriteRenderer arrowSpriteRenderer; 

    void Update()
    {
        // 1. Buscar a todos los enemigos en el mapa
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 2. Si no hay enemigos vivos, ocultamos la flecha
        if (enemies.Length == 0)
        {
            if (arrowVisual.activeSelf) 
                arrowVisual.SetActive(false);
            return;
        }

        // 3. Variables para encontrar al más cercano
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        // 4. Medir la distancia contra cada enemigo
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(player.position, enemy.transform.position);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // 5. Apuntar y colorear hacia el enemigo ganador
        if (nearestEnemy != null)
        {
            if (!arrowVisual.activeSelf) 
                arrowVisual.SetActive(true);

            // --- NUEVO: CAMBIO DE COLOR DINÁMICO ---
            // Buscamos el dibujo del enemigo al que estamos apuntando
            SpriteRenderer enemySprite = nearestEnemy.GetComponent<SpriteRenderer>();
            
            // Si el enemigo tiene dibujo y nuestra flecha también, copiamos el color exacto
            if (enemySprite != null && arrowSpriteRenderer != null)
            {
                arrowSpriteRenderer.color = enemySprite.color;
            }
            // ---------------------------------------

            // Calculamos el ángulo
            Vector2 direction = nearestEnemy.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Rotamos el pivote
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}