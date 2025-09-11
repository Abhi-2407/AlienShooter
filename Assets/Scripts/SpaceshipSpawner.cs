using UnityEngine;
using System.Collections;

public class SpaceshipSpawner : MonoBehaviour
{
    [Header("Spaceship Prefabs")]
    public GameObject redSpaceshipPrefab;
    public GameObject blueSpaceshipPrefab;
    
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public float spawnDelay = 5f;
    public float spawnInterval = 10f;
    public int maxSpaceships = 2; // 1 red + 1 blue
    
    [Header("Spaceship Settings")]
    public float redSpaceshipSpeed = 3f;
    public float blueSpaceshipSpeed = 3f;
    public int redSpaceshipScore = 50;
    public int blueSpaceshipScore = 50;
    
    [Header("Spawn Control")]
    public bool autoSpawn = true;
    public bool spawnOnStart = true;
    public bool autoRespawn = true;
    public float respawnDelay = 1f;
    
    private Camera mainCamera;
    private Vector2 screenBounds;
    private int currentSpaceshipCount = 0;
    private bool redSpaceshipActive = false;
    private bool blueSpaceshipActive = false;
    
    void Start()
    {
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        
        // Create spawn points if none exist
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            CreateSpawnPoints();
        }
        
        if (spawnOnStart)
        {
            SpaceShipPair();
        }
    }
    
    void Update()
    {
        // Count current spaceships
        currentSpaceshipCount = GameObject.FindGameObjectsWithTag("Spaceship").Length;
        
        // Check if spaceships are active
        CheckSpaceshipStatus();
    }
    
    void CreateSpawnPoints()
    {
        // Create spawn points for spaceships
        GameObject spawnParent = new GameObject("SpaceshipSpawnPoints");
        spawnParent.transform.SetParent(transform);
        
        // Create 2 spawn points: left and right
        spawnPoints = new Transform[2];
        
        // Left side spawn point
        GameObject leftSpawn = new GameObject("LeftSpaceshipSpawn");
        leftSpawn.transform.SetParent(spawnParent.transform);
        leftSpawn.transform.position = new Vector3(-screenBounds.x + 1f, screenBounds.y + 2f, 0f);
        spawnPoints[0] = leftSpawn.transform;
        
        // Right side spawn point
        GameObject rightSpawn = new GameObject("RightSpaceshipSpawn");
        rightSpawn.transform.SetParent(spawnParent.transform);
        rightSpawn.transform.position = new Vector3(screenBounds.x - 1f, screenBounds.y + 2f, 0f);
        spawnPoints[1] = rightSpawn.transform;
    }

    void SpaceShipPair()
    {
        // Spawn red spaceship if not active
        if (!redSpaceshipActive && redSpaceshipPrefab != null)
        {
            SpawnRedSpaceship();
        }

        // Spawn blue spaceship if not active
        if (!blueSpaceshipActive && blueSpaceshipPrefab != null)
        {
            SpawnBlueSpaceship();
        }
    }
    
    IEnumerator SpawnSpaceships()
    {
        while (autoSpawn)
        {
            yield return new WaitForSeconds(spawnDelay);
            
            // Spawn red spaceship if not active
            if (!redSpaceshipActive && redSpaceshipPrefab != null)
            {
                SpawnRedSpaceship();
            }
            
            yield return new WaitForSeconds(1f); // Small delay between spawns
            
            // Spawn blue spaceship if not active
            if (!blueSpaceshipActive && blueSpaceshipPrefab != null)
            {
                SpawnBlueSpaceship();
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    public void SpawnRedSpaceship()
    {
        if (redSpaceshipPrefab == null || spawnPoints[0] == null) return;
        
        GameObject redSpaceship = Instantiate(redSpaceshipPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
        SpaceshipController controller = redSpaceship.GetComponent<SpaceshipController>();
        
        if (controller != null)
        {
            controller.SetSpaceshipType(SpaceshipController.SpaceshipType.Red);
            controller.SetMoveSpeed(redSpaceshipSpeed);
            controller.SetScoreValue(redSpaceshipScore);
        }
        
        // Tag the spaceship
        redSpaceship.tag = "Spaceship";
        
        redSpaceshipActive = true;
        Debug.Log("Red spaceship spawned!");
    }
    
    public void SpawnBlueSpaceship()
    {
        if (blueSpaceshipPrefab == null || spawnPoints[1] == null) return;
        
        GameObject blueSpaceship = Instantiate(blueSpaceshipPrefab, spawnPoints[1].position, spawnPoints[1].rotation);
        SpaceshipController controller = blueSpaceship.GetComponent<SpaceshipController>();
        
        if (controller != null)
        {
            controller.SetSpaceshipType(SpaceshipController.SpaceshipType.Blue);
            controller.SetMoveSpeed(blueSpaceshipSpeed);
            controller.SetScoreValue(blueSpaceshipScore);
        }
        
        // Tag the spaceship
        blueSpaceship.tag = "Spaceship";
        
        blueSpaceshipActive = true;
        Debug.Log("Blue spaceship spawned!");
    }
    
    public void OnSpaceshipDestroyed(SpaceshipController.SpaceshipType type)
    {
        switch (type)
        {
            case SpaceshipController.SpaceshipType.Red:
                redSpaceshipActive = false;
                if (autoRespawn)
                {
                    StartCoroutine(RespawnRedSpaceship());
                }
                break;
            case SpaceshipController.SpaceshipType.Blue:
                blueSpaceshipActive = false;
                if (autoRespawn)
                {
                    StartCoroutine(RespawnBlueSpaceship());
                }
                break;
        }
        
        Debug.Log($"{type} spaceship destroyed! Respawning in {respawnDelay} seconds...");
    }
    
    void CheckSpaceshipStatus()
    {
        // Check if red spaceship is still active
        GameObject[] spaceships = GameObject.FindGameObjectsWithTag("Spaceship");
        bool redFound = false;
        bool blueFound = false;
        
        foreach (GameObject spaceship in spaceships)
        {
            SpaceshipController controller = spaceship.GetComponent<SpaceshipController>();
            if (controller != null)
            {
                if (controller.spaceshipType == SpaceshipController.SpaceshipType.Red)
                    redFound = true;
                else if (controller.spaceshipType == SpaceshipController.SpaceshipType.Blue)
                    blueFound = true;
            }
        }
        
        redSpaceshipActive = redFound;
        blueSpaceshipActive = blueFound;
    }
    
    public void SetAutoSpawn(bool auto)
    {
        autoSpawn = auto;
    }
    
    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
    }
    
    public void SetSpawnDelay(float delay)
    {
        spawnDelay = delay;
    }
    
    public bool IsRedSpaceshipActive()
    {
        return redSpaceshipActive;
    }
    
    public bool IsBlueSpaceshipActive()
    {
        return blueSpaceshipActive;
    }
    
    public int GetCurrentSpaceshipCount()
    {
        return currentSpaceshipCount;
    }
    
    IEnumerator RespawnRedSpaceship()
    {
        // Wait for respawn delay
        yield return new WaitForSeconds(respawnDelay);
        
        // Check if red spaceship is still not active (not manually spawned)
        if (!redSpaceshipActive && redSpaceshipPrefab != null)
        {
            SpawnRedSpaceship();
            Debug.Log("Red spaceship respawned!");
        }
    }
    
    IEnumerator RespawnBlueSpaceship()
    {
        // Wait for respawn delay
        yield return new WaitForSeconds(respawnDelay);
        
        // Check if blue spaceship is still not active (not manually spawned)
        if (!blueSpaceshipActive && blueSpaceshipPrefab != null)
        {
            SpawnBlueSpaceship();
            Debug.Log("Blue spaceship respawned!");
        }
    }
    
    public void SetAutoRespawn(bool auto)
    {
        autoRespawn = auto;
    }
    
    public void SetRespawnDelay(float delay)
    {
        respawnDelay = delay;
    }
}
