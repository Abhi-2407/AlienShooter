using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
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

    [Header("Visual")]
    public Color spaceshipColor = Color.white;

    public enum SpaceshipType
    {
        Red,
        Blue
    }

    private Rigidbody2D rb;
    public bool isActive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        // Set spaceship color based on type
        SetSpaceshipColor();

        // Initialize random durations
        SetRandomDurations();

        // Initialize movement timing
        nextDirectionChangeTime = Time.time + Random.Range(currentMoveDuration * 0.5f, currentMoveDuration * 1.5f);
    }

    void OnEnable()
    {
        
    }

    void Update()
    {
        if (!isActive) return;

        if (GameManager.Instance.gameState == GameState.START)
            HandleMovement();
    }

    void HandleMovement()
    {
        if (rb == null) return;

        // Check if spaceship is off screen
        if (transform.position.y < -10f)
        {
            DestroySpaceship();
            return;
        }

        // Calculate horizontal offset from start position
        float horizontalOffset = transform.position.x - startPosition.x;

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
                Debug.Log($"{spaceshipType} spaceship stopped moving after {currentMoveDuration:F1}s");
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
                    Debug.Log($"{spaceshipType} spaceship changed direction to {(movingRight ? "right" : "left")} after {currentStopDuration:F1}s stop");
                }
            }
        }

        // Random direction change during movement
        if (isMoving && Time.time >= nextDirectionChangeTime)
        {
            if (Random.Range(0f, 1f) < directionChangeChance)
            {
                movingRight = !movingRight;
                Debug.Log($"{spaceshipType} spaceship randomly changed direction to {(movingRight ? "right" : "left")}");
            }
            nextDirectionChangeTime = Time.time + Random.Range(currentMoveDuration * 0.5f, currentMoveDuration * 1.5f);
        }
    }

    void SetRandomDurations()
    {
        // Calculate random durations based on base values and variations
        currentMoveDuration = Random.Range(moveDurationMin, moveDurationMax);

        currentStopDuration = Random.Range(stopDurationMin, stopDurationMax);

        Debug.Log($"{spaceshipType} spaceship: Move={currentMoveDuration:F1}s, Stop={currentStopDuration:F1}s");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        // Check collision with enemies
        if (spaceshipType == SpaceshipType.Red && other.CompareTag("RedEnemy"))
        {
            HandleEnemyCollision(other.gameObject);
        }
        else if (spaceshipType == SpaceshipType.Blue && other.CompareTag("BlueEnemy"))
        {
            HandleEnemyCollision(other.gameObject);
        }
    }

    void HandleEnemyCollision(GameObject enemy)
    {
        if (!isActive) return;

        // Get enemy controller
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null && !enemyController.isDead)
        {
            // Add score
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.AddScore(scoreValue);
                gameManager.AddSpaceshipScore(spaceshipType, scoreValue);
            }

            // Create explosion effects
            CreateExplosionEffect();

            // Play collision sound
            PlayCollisionSound();

            // Destroy both spaceship and enemy
            DestroyEnemy(enemy);
            DestroySpaceship();

            Debug.Log($"{spaceshipType} spaceship collided with {spaceshipType} enemy! +{scoreValue} points");
        }
    }

    void DestroyEnemy(GameObject enemy)
    {
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.isDead = true;
        }
        Destroy(enemy);
    }

    void DestroySpaceship()
    {
        if (!isActive) return;

        isActive = false;

        rb.linearVelocity = Vector2.zero;

        // Notify spaceship spawner
        //SpaceshipSpawner spawner = FindObjectOfType<SpaceshipSpawner>();
        //if (spawner != null)
        //{
        //    spawner.OnSpaceshipDestroyed(spaceshipType);
        //}

        // Create explosion effect
        CreateExplosionEffect();

        GameManager.Instance.HandleSpaceShip(gameObject, startPosition);

        // Destroy spaceship
        //Destroy(gameObject);
    }

    void CreateExplosionEffect()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }

    void PlayCollisionSound()
    {
        if (collisionSound != null)
        {
            AudioSource.PlayClipAtPoint(collisionSound, transform.position);
        }
    }

    void SetSpaceshipColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = spaceshipColor;
        }
    }

    public void SetSpaceshipType(SpaceshipType type)
    {
        spaceshipType = type;
        SetSpaceshipColor();
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetScoreValue(int value)
    {
        scoreValue = value;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void SetDirectionChangeChance(float chance)
    {
        directionChangeChance = Mathf.Clamp01(chance);
    }

    public void ForceStop()
    {
        isMoving = false;
        currentStopTime = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    public void ForceStart()
    {
        isMoving = true;
        currentMoveTime = 0f;
    }

    public bool IsCurrentlyMoving()
    {
        return isMoving;
    }

    public void ForceNewRandomDurations()
    {
        SetRandomDurations();
    }

    public float GetCurrentMoveDuration()
    {
        return currentMoveDuration;
    }

    public float GetCurrentStopDuration()
    {
        return currentStopDuration;
    }
}



