using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Generación")]
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;

    [Header("Límites del Mapa")]
    public Vector2 minBounds; // Ej: (0, 0)
    public Vector2 maxBounds; // Ej: (20, 20)

    [Header("Comprobación de Espacio")]
    public LayerMask obstacleLayer; // Asigna aquí la capa "Obstaculos"
    public float checkRadius = 0.5f; // El tamaño del enemigo para comprobar que cabe

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        // Iniciar la rutina que se ejecutará cada X segundos
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Esperar 5 segundos antes de generar el siguiente
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = Vector2.zero;
        bool positionFound = false;
        int maxAttempts = 10; // Evitar que el juego se congele buscando un hueco

        for (int i = 0; i < maxAttempts; i++)
        {
            // 1. Obtener un punto aleatorio en el borde de la cámara
            spawnPos = GetRandomPointAroundCamera();

            // 2. Restringir el punto a los límites de la habitación
            spawnPos.x = Mathf.Clamp(spawnPos.x, minBounds.x, maxBounds.x);
            spawnPos.y = Mathf.Clamp(spawnPos.y, minBounds.y, maxBounds.y);

            // 3. Comprobar si hay un muro en esa posición
            // OverlapCircle devuelve un colisionador si encuentra algo en esa capa
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, checkRadius, obstacleLayer);

            if (hit == null)
            {
                // No hay muros, la posición es válida
                positionFound = true;
                break; // Salir del bucle for
            }
        }

        // Si después de 10 intentos encontramos un hueco válido, instanciamos el enemigo
        if (positionFound)
        {
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        }
    }

    Vector2 GetRandomPointAroundCamera()
    {
        float x = 0f, y = 0f;
        
        // Elegir aleatoriamente uno de los 4 bordes de la pantalla (0=Arriba, 1=Abajo, 2=Izquierda, 3=Derecha)
        int side = Random.Range(0, 4);

        // Usamos el Viewport: 0,0 es la esquina inferior izquierda y 1,1 es la superior derecha de la pantalla.
        // Elegimos valores como -0.1 o 1.1 para que aparezcan justo AFUERA de la vista del jugador.
        switch (side)
        {
            case 0: // Borde Superior
                x = Random.Range(-0.1f, 1.1f);
                y = 1.1f; 
                break;
            case 1: // Borde Inferior
                x = Random.Range(-0.1f, 1.1f);
                y = -0.1f; 
                break;
            case 2: // Borde Izquierdo
                x = -0.1f; 
                y = Random.Range(-0.1f, 1.1f); 
                break;
            case 3: // Borde Derecho
                x = 1.1f; 
                y = Random.Range(-0.1f, 1.1f); 
                break;
        }

        

        // Convertir esas coordenadas de pantalla a coordenadas reales del mundo 2D
        return mainCam.ViewportToWorldPoint(new Vector3(x, y, mainCam.nearClipPlane));
    }
}