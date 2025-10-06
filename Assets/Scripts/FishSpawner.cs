using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Prefabs")]
    public GameObject[] fishPrefabs;
    public int fishCount = 5;
    public float spawnDelay = 1f;
    
    [Header("Spawn Area")]
    public Vector2 spawnAreaCenter = Vector2.zero;
    public Vector2 spawnAreaSize = new Vector2(12f, 8f);
    public bool useCameraBounds = true;
    
    [Header("Swimming Area")]
    public Vector2 swimAreaCenter = Vector2.zero;
    public Vector2 swimAreaSize = new Vector2(10f, 6f);
    public bool useSameAreaForSwimming = true;
    
    [Header("Fish Behavior Settings")]
    public float minSwimSpeed = 1f;
    public float maxSwimSpeed = 3f;
    public float changeDirectionInterval = 2f;
    private bool canSwimUpDown = false;
    public bool canSwimLeftRight = true;
    
    [Header("Spawn Control")]
    public bool spawnOnStart = true;
    public bool continuousSpawning = false;
    public float respawnDelay = 10f;
    public int maxFishCount = 10;
    
    [Header("Auto Respawn")]
    public bool autoRespawnOnDestroy = true;
    public float respawnDelayOnDestroy = 2f;
    public int targetFishCount = 5;
    
    private List<GameObject> spawnedFish = new List<GameObject>();
    private Camera mainCamera;
    private Vector2 screenBounds;
    private bool isSpawning = false;
    private int previousFishCount = 0;
    private float lastRespawnTime = 0f;
    
    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        }
        
        // Set up spawn and swim areas based on camera bounds if enabled
        if (useCameraBounds && mainCamera != null)
        {
            //spawnAreaCenter = Vector2.zero;
            //spawnAreaSize = new Vector2(screenBounds.x * 2f, screenBounds.y * 2f);
            
            if (useSameAreaForSwimming)
            {
                swimAreaCenter = spawnAreaCenter;
                swimAreaSize = spawnAreaSize;
            }
        }

        // Initialize fish count tracking
        previousFishCount = 0;
    }
    
    void Update()
    {
        if (GameManager.Instance.gameState == GameState.START)
        {
            // Clean up destroyed fish from the list
            int currentFishCount = spawnedFish.Count;
            spawnedFish.RemoveAll(fish => fish == null);
            int actualFishCount = spawnedFish.Count;

            // Check if any fish were destroyed
            if (autoRespawnOnDestroy && actualFishCount < previousFishCount)
            {
                int fishDestroyed = previousFishCount - actualFishCount;
                //Debug.Log($"Fish destroyed! Count: {previousFishCount} -> {actualFishCount}. Destroyed: {fishDestroyed}");

                // Schedule respawn if enough time has passed
                if (Time.time - lastRespawnTime >= respawnDelayOnDestroy)
                {
                    StartCoroutine(RespawnDestroyedFish(fishDestroyed));
                    lastRespawnTime = Time.time;
                }
            }

            // Update previous count
            previousFishCount = actualFishCount;

            // Handle continuous spawning
            if (continuousSpawning && actualFishCount < maxFishCount)
            {
                if (!isSpawning)
                {
                    StartCoroutine(ContinuousSpawnRoutine());
                }
            }

            // Auto-respawn to maintain target count
            if (autoRespawnOnDestroy && actualFishCount < targetFishCount && Time.time - lastRespawnTime >= respawnDelayOnDestroy)
            {
                int fishNeeded = targetFishCount - actualFishCount;
                StartCoroutine(RespawnDestroyedFish(fishNeeded));
                lastRespawnTime = Time.time;
            }
        }
    }
    
    public void StartSpawning()
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            //Debug.LogWarning("No fish prefabs assigned to FishSpawner!");
            return;
        }
        
        StartCoroutine(SpawnFishRoutine());
    }
    
    IEnumerator SpawnFishRoutine()
    {
        isSpawning = true;
        
        for (int i = 0; i < fishCount; i++)
        {
            if (spawnedFish.Count >= maxFishCount)
            {
                break;
            }
            
            SpawnRandomFish();
            yield return new WaitForSeconds(spawnDelay);
        }
        
        isSpawning = false;
    }
    
    IEnumerator ContinuousSpawnRoutine()
    {
        isSpawning = true;
        
        while (continuousSpawning && spawnedFish.Count < maxFishCount)
        {
            SpawnRandomFish();
            yield return new WaitForSeconds(respawnDelay);
        }
        
        isSpawning = false;
    }
    
    IEnumerator RespawnDestroyedFish(int fishToRespawn)
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0) yield break;
        
        //Debug.Log($"Respawning {fishToRespawn} fish...");
        
        for (int i = 0; i < fishToRespawn; i++)
        {
            if (spawnedFish.Count >= maxFishCount) break;
            
            SpawnRandomFish();
            
            // Small delay between respawns for visual effect
            yield return new WaitForSeconds(0.5f);
        }
        
        //Debug.Log($"Respawn complete. Total fish: {spawnedFish.Count}");
    }
    
    void SpawnRandomFish()
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0) return;
        
        // Choose random fish prefab
        GameObject fishPrefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        
        // Generate random spawn position within spawn area
        Vector2 spawnPosition = GetRandomSpawnPosition();
        
        // Spawn the fish
        GameObject newFish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
        
        // Configure the fish
        ConfigureFish(newFish);
        
        // Add to spawned fish list
        spawnedFish.Add(newFish);
        
        //Debug.Log($"Spawned fish at {spawnPosition}. Total fish: {spawnedFish.Count}");
    }
    
    Vector2 GetRandomSpawnPosition()
    {
        Vector2 min = new Vector2(
            spawnAreaCenter.x - spawnAreaSize.x / 2f,
            spawnAreaCenter.y - spawnAreaSize.y / 2f
        );
        Vector2 max = new Vector2(
            spawnAreaCenter.x + spawnAreaSize.x / 2f,
            spawnAreaCenter.y + spawnAreaSize.y / 2f
        );
        
        return new Vector2(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y)
        );
    }
    
    void ConfigureFish(GameObject fish)
    {
        FishController fishController = fish.GetComponent<FishController>();
        if (fishController == null)
        {
            fishController = fish.AddComponent<FishController>();
        }
        
        // Set swimming area
        fishController.SetSwimArea(swimAreaCenter, swimAreaSize);
        
        //// Set random swim speed
        //float randomSpeed = Random.Range(minSwimSpeed, maxSwimSpeed);
        //fishController.SetSwimSpeed(randomSpeed);
        
        // Set behavior settings
        //fishController.canSwimUpDown = canSwimUpDown;
        fishController.canSwimLeftRight = canSwimLeftRight;
        fishController.changeDirectionInterval = changeDirectionInterval;
        
        //// Set random color if fish has a SpriteRenderer
        //SpriteRenderer spriteRenderer = fish.GetComponent<SpriteRenderer>();
        //if (spriteRenderer != null)
        //{
        //    spriteRenderer.color = new Color(
        //        Random.Range(0.3f, 1f),
        //        Random.Range(0.3f, 1f),
        //        Random.Range(0.3f, 1f),
        //        1f
        //    );
        //}
        
        // Set random scale for variety
        float randomScale = Random.Range(0.8f, 1.2f);
        fish.transform.localScale = Vector3.one * randomScale;
    }
    
    public void SpawnFishAtPosition(Vector2 position)
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0) return;
        
        GameObject fishPrefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];
        GameObject newFish = Instantiate(fishPrefab, position, Quaternion.identity);
        
        ConfigureFish(newFish);
        spawnedFish.Add(newFish);
    }
    
    public void ClearAllFish()
    {
        foreach (GameObject fish in spawnedFish)
        {
            if (fish != null)
            {
                Destroy(fish);
            }
        }
        spawnedFish.Clear();
    }
    
    public void SetSpawnArea(Vector2 center, Vector2 size)
    {
        spawnAreaCenter = center;
        spawnAreaSize = size;
    }
    
    public void SetSwimArea(Vector2 center, Vector2 size)
    {
        swimAreaCenter = center;
        swimAreaSize = size;
        
        // Update all existing fish
        foreach (GameObject fish in spawnedFish)
        {
            if (fish != null)
            {
                FishController controller = fish.GetComponent<FishController>();
                if (controller != null)
                {
                    controller.SetSwimArea(center, size);
                }
            }
        }
    }
    
    public void SetFishCount(int count)
    {
        fishCount = count;
    }
    
    public void SetMaxFishCount(int maxCount)
    {
        maxFishCount = maxCount;
    }
    
    public void EnableContinuousSpawning(bool enable)
    {
        continuousSpawning = enable;
    }
    
    public int GetCurrentFishCount()
    {
        return spawnedFish.Count;
    }
    
    public void SetAutoRespawnOnDestroy(bool enable)
    {
        autoRespawnOnDestroy = enable;
    }
    
    public void SetRespawnDelayOnDestroy(float delay)
    {
        respawnDelayOnDestroy = delay;
    }
    
    public void SetTargetFishCount(int count)
    {
        targetFishCount = count;
    }
    
    public void ForceRespawnToTarget()
    {
        if (autoRespawnOnDestroy)
        {
            int fishNeeded = targetFishCount - spawnedFish.Count;
            if (fishNeeded > 0)
            {
                StartCoroutine(RespawnDestroyedFish(fishNeeded));
            }
        }
    }
    
    public void RespawnFishImmediately(int count)
    {
        StartCoroutine(RespawnDestroyedFish(count));
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        
        // Draw swim area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(swimAreaCenter, swimAreaSize);
        
        // Draw spawn points
        Gizmos.color = Color.red;
        for (int i = 0; i < 10; i++)
        {
            Vector2 pos = GetRandomSpawnPosition();
            Gizmos.DrawWireSphere(pos, 0.2f);
        }
    }
}



