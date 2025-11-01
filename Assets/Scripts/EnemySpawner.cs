  using UnityEngine;
using System.Collections;
using Fusion;

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

    public static EnemySpawner Instance;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        SpawnEnemyPair();
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

    void SpawnEnemyPair()
    {
        if (GameManager.Instance.IsSinglePlayerMode)
        {
            if (redEnemyPrefab != null && spawnPoints[0] != null)
            {
                GameObject leftRed = Instantiate(redEnemyPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
            }

            if (blueEnemyPrefab != null && spawnPoints[1] != null)
            {
                GameObject leftBlue = Instantiate(blueEnemyPrefab, spawnPoints[1].position, spawnPoints[1].rotation);
            }
        }
    }

    public void SpawnEnemyForMultiplayer()
    {
        if (GameManager.Instance.localPlayer.localPlayerID == 0)
        {
            if (blueEnemyPrefab != null && spawnPoints[1] != null)
            {
                GameObject leftBlue = Instantiate(blueEnemyPrefab, spawnPoints[1].position, spawnPoints[1].rotation);
            }
        }
        else
        {
            if (redEnemyPrefab != null && spawnPoints[0] != null)
            {
                GameObject leftRed = Instantiate(redEnemyPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
            }
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

    public IEnumerator SpawnRedEnemy()
    {
        yield return new WaitForSeconds(0.5f);

        // Choose a random spawn point for red enemy
        Vector3 spawnPointPos = spawnPoints[0].position + new Vector3(Random.Range(-2.5f, 2.5f), 0, 0); // Left side spawn points

        SpawnRedEnemy_(spawnPointPos);

        if (!GameManager.Instance.IsSinglePlayerMode)
        {
            GameManager.Instance.localPlayer.RPC_RedEnemyCreate(spawnPointPos);
        }
    }

    public void SpawnRedEnemy_(Vector3 spawnPointPos)
    {
        //if (!GameManager.Instance.IsSinglePlayerMode)
        //{
        //    NetworkRunner runner = FusionConnector.instance.NetworkRunner;

        //    NetworkObject blueEnemy = runner.Spawn(redEnemyPrefab, spawnPointPos, Quaternion.identity);
        //}
        //else
        //{
            if (redEnemyPrefab != null)
            {
                GameObject leftRed = Instantiate(redEnemyPrefab, spawnPointPos, Quaternion.identity);
            }
        //}
        //Debug.Log("Red enemy spawned!");
    }
    
    public IEnumerator SpawnBlueEnemy()
    {
        yield return new WaitForSeconds(1.0f);
        
        // Choose a random spawn point for blue enemy
        Vector3  spawnPointPos = spawnPoints[1].position + new Vector3(Random.Range(-2.5f,2.5f),0,0); // Left side spawn points

        SpawnBlueEnemy_(spawnPointPos);

        if (!GameManager.Instance.IsSinglePlayerMode)
        {
            GameManager.Instance.localPlayer.RPC_BlueEnemyCreate(spawnPointPos);
        }
    }

    public void SpawnBlueEnemy_(Vector3 spawnPointPos)
    {
        //if (!GameManager.Instance.IsSinglePlayerMode)
        //{
        //    NetworkRunner runner = FusionConnector.instance.NetworkRunner;

        //    NetworkObject blueEnemy = runner.Spawn(blueEnemyPrefab, spawnPointPos, Quaternion.identity);
        //}
        //else
        //{
            if (blueEnemyPrefab != null)
            {
                GameObject leftBlue = Instantiate(blueEnemyPrefab, spawnPointPos, Quaternion.identity);
            }
        //}
        //Debug.Log("Blue enemy spawned!");
    }
}
