using Fusion;
using UnityEngine;

public class SpaceshipController : NetworkBehaviour
{
    [Header("Spaceship Settings")]
    public SpaceshipType spaceshipType;
    public int scoreValue = 50;

    [Header("Movement")]
    public float horizontalRange = 4f;
    public float moveSpeed = 2f;
    public float stopDurationMin = 0f;
    public float stopDurationMax = 0f;
    public float moveDurationMin = 0f;
    public float moveDurationMax = 0f;
    public float directionChangeChance = 0.3f;

    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isMoving = true;
    private float currentMoveTime = 0f;
    private float currentStopTime = 0f;
    private float nextDirectionChangeTime = 0f;

    // Random timing variables
    private float currentMoveDuration = 0f;
    private float currentStopDuration = 0f;

    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip collisionSound;

    NetworkRunner runner;

    public enum SpaceshipType
    {
        Red,
        Blue
    }

    private Rigidbody2D rb;
    public bool isActive = true;

    void Start()
    {
        runner = FusionConnector.instance.NetworkRunner;
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        // Initialize random durations
        SetRandomDurations();

        // Initialize movement timing
        nextDirectionChangeTime = Time.time + Random.Range(currentMoveDuration * 0.5f, currentMoveDuration * 1.5f);
    }

    void Update()
    {
        if (!isActive) return;

        if (GameManager.Instance.IsSinglePlayerMode)
        {
            if (GameManager.Instance.gameState == GameState.START)
            {
                HandleMovement();
            }
            if (GameManager.Instance.gameState == GameState.OVER)
            {
                rb.linearVelocity = new Vector2(0, 0);
            }
        }
        else
        {
            if(Object.HasStateAuthority)
            {
                if (GameManager.Instance.gameState == GameState.START)
                {
                    HandleMovement();
                }
                if (GameManager.Instance.gameState == GameState.OVER)
                {
                    rb.linearVelocity = new Vector2(0, 0);
                }
            }
        }
    }

    void HandleMovement()
    {
        if (rb == null) return;

        float centerpoint = 4.5f;

        if(spaceshipType == SpaceshipType.Blue)
        {
            centerpoint = -4.5f;
        }
        else
        {
            centerpoint = 4.5f;
        }

        // Calculate horizontal offset from start position
        float horizontalOffset = transform.position.x - centerpoint;

        // Check if we've reached the horizontal range limits
        if (horizontalOffset >= horizontalRange && movingRight)
        {
            movingRight = false;
        }
        else if (horizontalOffset <= -horizontalRange && !movingRight)
        {
            movingRight = true;
        }

        // Handle movement and stopping behavior
        if (isMoving)
        {
            // Move horizontally
            float currentHorizontalSpeed = movingRight ? moveSpeed : -moveSpeed;
            rb.linearVelocity = new Vector2(currentHorizontalSpeed, 0);

            // Update move timer
            currentMoveTime += Time.deltaTime;

            // Check if it's time to stop
            if (currentMoveTime >= currentMoveDuration)
            {
                isMoving = false;
                currentStopTime = 0f;
                rb.linearVelocity = Vector2.zero;
                //Debug.Log($"{spaceshipType} spaceship stopped moving after {currentMoveDuration:F1}s");
            }
        }
        else
        {
            // Currently stopped
            rb.linearVelocity = Vector2.zero;
            currentStopTime += Time.deltaTime;

            // Check if it's time to start moving again
            if (currentStopTime >= currentStopDuration)
            {
                isMoving = true;
                currentMoveTime = 0f;

                // Set new random durations for next cycle
                SetRandomDurations();

                // Randomly decide whether to change direction
                if (Random.Range(0f, 1f) < directionChangeChance)
                {
                    movingRight = !movingRight;
                    //Debug.Log($"{spaceshipType} spaceship changed direction to {(movingRight ? "right" : "left")} after {currentStopDuration:F1}s stop");
                }
            }
        }

        // Random direction change during movement
        if (isMoving && Time.time >= nextDirectionChangeTime)
        {
            if (Random.Range(0f, 1f) < directionChangeChance)
            {
                movingRight = !movingRight;
                //Debug.Log($"{spaceshipType} spaceship randomly changed direction to {(movingRight ? "right" : "left")}");
            }
            nextDirectionChangeTime = Time.time + Random.Range(currentMoveDuration * 0.5f, currentMoveDuration * 1.5f);
        }
    }

    void SetRandomDurations()
    {
        // Calculate random durations based on base values and variations
        currentMoveDuration = Random.Range(moveDurationMin, moveDurationMax);

        currentStopDuration = Random.Range(stopDurationMin, stopDurationMax);

        //Debug.Log($"{spaceshipType} spaceship: Move={currentMoveDuration:F1}s, Stop={currentStopDuration:F1}s");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (spaceshipType == SpaceshipType.Blue && other.CompareTag("BlueEnemy"))
        {
            HandleBluePlayer(other.gameObject);
        }

        if (spaceshipType == SpaceshipType.Red && other.CompareTag("RedEnemy"))
        {
            HandleRedPlayer(other.gameObject);
        }
    }

    public void HandleBluePlayer(GameObject missile)
    {
        CreateExplosionEffect();

        PlayExplosionSound();

        if (GameManager.Instance.IsSinglePlayerMode)
        {
            Destroy(missile);
            GameManager.Instance.SpawnBlueMissile();
        }
        else
        {
            runner.Despawn(missile.GetComponent<NetworkObject>());
            GameManager.Instance.SpawnBlueMissile(runner);
        }

        HandleSpaceShip();
    }

    public void HandleRedPlayer(GameObject missile)
    {
        CreateExplosionEffect();

        PlayExplosionSound();

        if (GameManager.Instance.IsSinglePlayerMode)
        {
            Destroy(missile);
            GameManager.Instance.SpawnRedMissile();
        }
        else
        {
            runner.Despawn(missile.GetComponent<NetworkObject>());
            GameManager.Instance.SpawnRedMissile(runner); 
        }

        HandleSpaceShip();
    }

    void HandleSpaceShip()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<Collider2D>().enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        GameManager.Instance.AddSpaceshipScore(spaceshipType, scoreValue);

        GameManager.Instance.HandleSpaceShip(spaceshipType, startPosition);

        if (!GameManager.Instance.IsSinglePlayerMode)
        {
            if (gameObject.GetComponent<NetworkObject>().HasStateAuthority)
            {
                GameManager.Instance.localPlayer.RPC_AddSpaceshipScore(spaceshipType, scoreValue);

                runner.Despawn(gameObject.GetComponent<NetworkObject>());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void CreateExplosionEffect()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }

    void PlayExplosionSound()
    {
        AudioManager.Instance.PlayExplosionSound();
    }

    public void SetSpaceshipType(SpaceshipType type)
    {
        spaceshipType = type;
    }
}



