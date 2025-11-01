using UnityEngine;
using System.Collections;

public class SpaceshipSpawner : MonoBehaviour
{
    [Header("Spaceship Prefabs")]
    public GameObject redSpaceshipPrefab;
    public GameObject blueSpaceshipPrefab;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

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
        if (GameManager.Instance.IsSinglePlayerMode)
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

    public void SpawnRedSpaceship()
    {
        if (redSpaceshipPrefab == null || spawnPoints[0] == null) return;

        //Vector3 offset = new Vector3(Random.Range(-2.5f, 2.5f), 0, 0);

        GameObject redSpaceship = Instantiate(redSpaceshipPrefab, spawnPoints[0].position, spawnPoints[0].rotation);
        SpaceshipController controller = redSpaceship.GetComponent<SpaceshipController>();
         
        if (controller != null)
        {
            controller.SetSpaceshipType(SpaceshipController.SpaceshipType.Red);
            //controller.SetMoveSpeed(redSpaceshipSpeed);
            //controller.SetScoreValue(redSpaceshipScore);
        }

        // Tag the spaceship
        redSpaceship.tag = "Spaceship";

        redSpaceshipActive = true;
        //Debug.Log("Red spaceship spawned!");
    }

    public void SpawnBlueSpaceship()
    {
        if (blueSpaceshipPrefab == null || spawnPoints[1] == null) return;

        //Vector3 offset = new Vector3(Random.Range(-2.5f, 2.5f), 0, 0);

        GameObject blueSpaceship = Instantiate(blueSpaceshipPrefab, spawnPoints[1].position, spawnPoints[1].rotation);
        SpaceshipController controller = blueSpaceship.GetComponent<SpaceshipController>();

        if (controller != null)
        {
            controller.SetSpaceshipType(SpaceshipController.SpaceshipType.Blue);
            //controller.SetMoveSpeed(blueSpaceshipSpeed);
            //controller.SetScoreValue(blueSpaceshipScore);
        }

        // Tag the spaceship
        blueSpaceship.tag = "Spaceship";

        blueSpaceshipActive = true;
        //Debug.Log("Blue spaceship spawned!");
    }
}
