using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Mantenemos nuestra clase de probabilidades
[System.Serializable]
public class EnemyType
{
    public string name;
    public GameObject prefab;
    [Range(0f, 100f)]
    public float spawnChance;
}

// NUEVO: Definimos qué compone a una Oleada
[System.Serializable]
public class Wave
{
    public string waveName; // Ej: "Oleada 1 - Zombies"
    public float waveDuration; // Cuántos segundos durará la generación
    public float spawnInterval; // Cada cuántos segundos sale un enemigo
    public EnemyType[] waveEnemies; // Las probabilidades específicas de ESTA oleada
}

public class WaveManager : MonoBehaviour
{
    [Header("Configuración de Oleadas")]
    public Wave[] waves; // Tu lista de oleadas
    private int currentWaveIndex = 0;

    [Header("Límites del Mapa")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    [Header("Comprobación de Espacio")]
    public LayerMask obstacleLayer;
    public float checkRadius = 0.5f;

    [Header("Recompensas y tiempos")]
    public GameObject chestPrefab;
    public float timeBetweenWaves = 15f;
    public float healPerWave = 20f;

    private Camera mainCam;
    private bool isSpawning = false;
    private bool waitingForEnemiesToDie = false;


    void Start()
    {
        mainCam = Camera.main;
        
        // Iniciamos la primera oleada
        if (waves.Length > 0)
        {
            StartCoroutine(StartWave());
        }
    }

    void Update()
    {
        if (waitingForEnemiesToDie)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                // Dejamos de esperar
                waitingForEnemiesToDie = false;
                
                // Iniciamos la rutina de descanso
                StartCoroutine(WaveBreakRoutine());
            }
        }
    }

    // --- NUEVA RUTINA DE DESCANSO ---
    IEnumerator WaveBreakRoutine()
    {
        Debug.Log("Oleada limpiada. Generando cofre...");
        SpawnChest();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healPerWave);
                Debug.Log("Curando al jugador por sobrevivir la oleada.");
            }
        }

        currentWaveIndex++;

        

        // Verificamos si aún quedan oleadas por jugar
        if (currentWaveIndex < waves.Length)
        {
            Debug.Log("Iniciando descanso de " + timeBetweenWaves + " segundos.");
            
            // Esperamos los 15 segundos
            yield return new WaitForSeconds(timeBetweenWaves);

            // Una vez pasado el tiempo, arrancamos la siguiente oleada
            StartCoroutine(StartWave());
        }
        else
        {
            Debug.Log("¡Victoria! Has superado todas las oleadas.");
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("EndingScene");
        }
    }

    IEnumerator StartWave()
    {
        Wave currentWave = waves[currentWaveIndex];
        Debug.Log("Iniciando: " + currentWave.waveName);
        
        isSpawning = true;
        
        // Lanzamos la corrutina que se encarga de crear enemigos
        Coroutine spawnRoutine = StartCoroutine(SpawnRoutine(currentWave));

        // El temporizador de la oleada
        yield return new WaitForSeconds(currentWave.waveDuration);

        // Se acabó el tiempo de la oleada
        isSpawning = false;
        StopCoroutine(spawnRoutine); // Detenemos la generación
        
        // Cambiamos el estado para que el Update empiece a revisar si los enemigos murieron
        waitingForEnemiesToDie = true;
        Debug.Log("Temporizador terminado. Esperando a que el jugador limpie la sala...");
    }

    IEnumerator SpawnRoutine(Wave wave)
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(wave.spawnInterval);
            SpawnEnemy(wave.waveEnemies);
        }
    }

    void SpawnEnemy(EnemyType[] availableEnemies)
    {
        GameObject enemyToSpawn = ChooseRandomEnemy(availableEnemies);
        if (enemyToSpawn == null) return;

        Vector2 spawnPos = Vector2.zero;
        bool positionFound = false;

        for (int i = 0; i < 10; i++)
        {
            spawnPos = GetRandomPointAroundCamera();
            spawnPos.x = Mathf.Clamp(spawnPos.x, minBounds.x, maxBounds.x);
            spawnPos.y = Mathf.Clamp(spawnPos.y, minBounds.y, maxBounds.y);

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, checkRadius, obstacleLayer);
            if (hit == null)
            {
                positionFound = true;
                break;
            }
        }

        if (positionFound)
        {
            Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        }
    }

    GameObject ChooseRandomEnemy(EnemyType[] availableEnemies)
    {
        float totalWeight = 0f;
        foreach (EnemyType enemy in availableEnemies)
        {
            totalWeight += enemy.spawnChance;
        }

        float randomValue = Random.Range(0, totalWeight);

        foreach (EnemyType enemy in availableEnemies)
        {
            if (randomValue < enemy.spawnChance)
            {
                return enemy.prefab;
            }
            randomValue -= enemy.spawnChance;
        }

        return null;
    }

    Vector2 GetRandomPointAroundCamera()
    {
        float x = 0f, y = 0f;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: x = Random.Range(-0.1f, 1.1f); y = 1.1f; break;
            case 1: x = Random.Range(-0.1f, 1.1f); y = -0.1f; break;
            case 2: x = -0.1f; y = Random.Range(-0.1f, 1.1f); break;
            case 3: x = 1.1f; y = Random.Range(-0.1f, 1.1f); break;
        }

        return mainCam.ViewportToWorldPoint(new Vector3(x, y, mainCam.nearClipPlane));
    }

    void SpawnChest()
    {
        if (chestPrefab == null)
        {
            Debug.LogWarning("No has asignado el Prefab del cofre en el WaveManager");
            return;
        }

        Vector2 spawnPos = Vector2.zero;
        bool positionFound = false;
    
        int maxAttempts = 50; 

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            float randomY = Random.Range(minBounds.y, maxBounds.y);
            spawnPos = new Vector2(randomX, randomY);

            Vector3 viewportPos = mainCam.WorldToViewportPoint(spawnPos);
            bool isInsideCamera = viewportPos.x >= 0f && viewportPos.x <= 1f && 
                                  viewportPos.y >= 0f && viewportPos.y <= 1f;

            // Si la cámara lo está viendo, este punto no nos sirve. Saltamos al siguiente intento.
            if (isInsideCamera) continue;

            // Comprobar si hay un bloque sólido en ese punto
            Collider2D hit = Physics2D.OverlapCircle(spawnPos, checkRadius, obstacleLayer);
            
            // Si hay un muro, este punto tampoco nos sirve.
            if (hit != null) continue;

            // Si llegamos hasta aquí, el punto es perfecto: dentro del mapa, fuera de cámara y sin muros.
            positionFound = true;
            break;
        }

        if (positionFound)
        {
            Instantiate(chestPrefab, spawnPos, Quaternion.identity);
            Debug.Log("¡Cofre generado en la arena!");
        }
        else
        {
            Debug.LogWarning("No se encontró un lugar oculto para el cofre.");
        }
    }
}