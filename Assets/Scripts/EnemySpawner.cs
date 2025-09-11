  using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject redEnemyPrefab;
    public GameObject blueEnemyPrefab;
    public Transform[] spawnPoints;
    public float spawnRate = 2f;
    public int maxEnemies = 4; // 2 red + 2 blue
    public float spawnDelay = 1f;
    
    [Header("Difficulty Settings")]
    public float difficultyIncreaseRate = 0.1f;
    public float maxSpawnRate = 0.5f;
    public int maxEnemiesIncrease = 5;
    
    [Header("Wave Settings")]
    public float waveDelay = 10f; // Longer delay since we spawn 4 enemies at once
    public int enemiesPerWave = 4; // 2 red + 2 blue
    
    private float nextSpawnTime;
    private int currentEnemyCount = 0;
    private int enemiesSpawnedThisWave = 0;
    private bool isSpawning = true;
    private Camera mainCamera;
    private Vector2 screenBounds;
    
    void Start()
    {
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        
        // Create spawn points if none exist
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            CreateSpawnPoints();
        }
        
        //StartCoroutine(SpawnWaves());

        SpawnEnemy();
    }
    
    void Update()
    {
        // Count current enemies
        currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
    
    void CreateSpawnPoints()
    {
        // Create spawn points for left and right sides
        GameObject spawnParent = new GameObject("SpawnPoints");
        spawnParent.transform.SetParent(transform);
        
        // Create 4 spawn points: left red, left blue, right red, right blue
        spawnPoints = new Transform[4];
        
        // Left side spawn points
        GameObject leftRedSpawn = new GameObject("LeftRedSpawn");
        leftRedSpawn.transform.SetParent(spawnParent.transform);
        leftRedSpawn.transform.position = new Vector3(-screenBounds.x + 2f, screenBounds.y + 1f, 0f);
        spawnPoints[0] = leftRedSpawn.transform;
        
        GameObject leftBlueSpawn = new GameObject("LeftBlueSpawn");
        leftBlueSpawn.transform.SetParent(spawnParent.transform);
        leftBlueSpawn.transform.position = new Vector3(-screenBounds.x + 1f, screenBounds.y + 1f, 0f);
        spawnPoints[1] = leftBlueSpawn.transform;
        
        // Right side spawn points
        GameObject rightRedSpawn = new GameObject("RightRedSpawn");
        rightRedSpawn.transform.SetParent(spawnParent.transform);
        rightRedSpawn.transform.position = new Vector3(screenBounds.x - 2f, screenBounds.y + 1f, 0f);
        spawnPoints[2] = rightRedSpawn.transform;
        
        GameObject rightBlueSpawn = new GameObject("RightBlueSpawn");
        rightBlueSpawn.transform.SetParent(spawnParent.transform);
        rightBlueSpawn.transform.position = new Vector3(screenBounds.x - 1f, screenBounds.y + 1f, 0f);
        spawnPoints[3] = rightBlueSpawn.transform;
    }
    
    IEnumerator SpawnWaves()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnDelay);
            
            // Spawn one red and one blue enemy on each side
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemyPair();
                enemiesSpawnedThisWave += 4; // 2 red + 2 blue
            }
            
            // Wait for wave to complete
            yield return new WaitForSeconds(waveDelay);
        }
    }
    
    void SpawnEnemyPair()
    {
        // Spawn left side enemies
        if (redEnemyPrefab != null && spawnPoints[0] != null)
        {
            GameObject leftRed = Instantiate(redEnemyPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
            ConfigureEnemyForWave(leftRed);
        }
        
        if (blueEnemyPrefab != null && spawnPoints[1] != null)
        {
            GameObject leftBlue = Instantiate(blueEnemyPrefab, spawnPoints[1].position, spawnPoints[1].rotation);
            ConfigureEnemyForWave(leftBlue);
        }
    }
    
    void SpawnEnemy()
    {
        // This method is kept for compatibility but not used in the new system
        SpawnEnemyPair();
    }
    
    void ConfigureEnemyForWave(GameObject enemy)
    {
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            // Increase enemy health based on wave
            int wave = GameManager.Instance ? GameManager.Instance.GetWave() : 1;
            enemyController.maxHealth = Mathf.RoundToInt(enemyController.maxHealth * (1 + wave * 0.2f));
            enemyController.currentHealth = enemyController.maxHealth;
            
            // Increase score value
            enemyController.scoreValue = Mathf.RoundToInt(enemyController.scoreValue * (1 + wave * 0.1f));
            
            // Increase movement speed
            enemyController.moveSpeed = Mathf.Min(enemyController.moveSpeed * (1 + wave * 0.1f), 8f);
        }
    }
    
    public void IncreaseDifficulty()
    {
        // Increase spawn rate
        spawnRate = Mathf.Max(spawnRate - difficultyIncreaseRate, maxSpawnRate);
        
        // Increase max enemies
        maxEnemies = Mathf.Min(maxEnemies + maxEnemiesIncrease, 50);
        
        // Increase enemies per wave
        enemiesPerWave = Mathf.Min(enemiesPerWave + 5, 100);
    }
    
    public void StopSpawning()
    {
        isSpawning = false;
    }
    
    public void StartSpawning()
    {
        isSpawning = true;
        StartCoroutine(SpawnWaves());
    }
    
    public void SetSpawnRate(float rate)
    {
        spawnRate = rate;
    }
    
    public void SetMaxEnemies(int max)
    {
        maxEnemies = max;
    }
    
    public int GetCurrentEnemyCount()
    {
        return currentEnemyCount;
    }
    
    public int GetEnemiesSpawnedThisWave()
    {
        return enemiesSpawnedThisWave;
    }
    
    public void ResetWave()
    {
        enemiesSpawnedThisWave = 0;
    }
    
    public void SpawnRedEnemy()
    {
        if (redEnemyPrefab == null) return;
        
        // Choose a random spawn point for red enemy
        Transform spawnPoint = spawnPoints[0]; // Left side spawn points
        
        GameObject redEnemy = Instantiate(redEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
        ConfigureEnemyForWave(redEnemy);
        
        Debug.Log("Red enemy spawned!");
    }
    
    public void SpawnBlueEnemy()
    {
        if (blueEnemyPrefab == null) return;
        
        // Choose a random spawn point for blue enemy
        Transform spawnPoint = spawnPoints[1]; // Left side spawn points
        
        GameObject blueEnemy = Instantiate(blueEnemyPrefab, spawnPoint.position, spawnPoint.rotation);
        ConfigureEnemyForWave(blueEnemy);
        
        Debug.Log("Blue enemy spawned!");
    }
}
