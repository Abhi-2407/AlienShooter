using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [Header("Spaceship Settings")]
    public SpaceshipType spaceshipType;
    public float moveSpeed = 5f;
    public int scoreValue = 50;
    
    [Header("Movement")]
    public float horizontalRange = 4f;
    private Vector3 startPosition;
    private bool movingRight = true;
    
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
    private bool isActive = true;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        
        // Set spaceship color based on type
        SetSpaceshipColor();
        
        // Set initial movement
        if (rb != null)
        {
            //rb.linearVelocity = new Vector2(moveSpeed, -verticalSpeed);
        }
    }
    
    void Update()
    {
        if (!isActive) return;
        
        HandleMovement();
    }
    
    void HandleMovement()
    {
        if (rb == null) return;
        
        // Calculate horizontal movement within range
        float horizontalOffset = transform.position.x - startPosition.x;
        
        // Check if we need to change direction
        if (horizontalOffset >= horizontalRange && movingRight)
        {
            movingRight = false;
        }
        else if (horizontalOffset <= -horizontalRange && !movingRight)
        {
            movingRight = true;
        }
        
        // Set velocity based on direction
        float currentHorizontalSpeed = movingRight ? moveSpeed : -moveSpeed;
        rb.linearVelocity = new Vector2(currentHorizontalSpeed, 0);
        
        // Check if spaceship is off screen
        if (transform.position.y < -10f)
        {
            DestroySpaceship();
        }
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
        
        // Notify spaceship spawner
        SpaceshipSpawner spawner = FindObjectOfType<SpaceshipSpawner>();
        if (spawner != null)
        {
            spawner.OnSpaceshipDestroyed(spaceshipType);
        }
        
        // Create explosion effect
        CreateExplosionEffect();
        
        // Destroy spaceship
        Destroy(gameObject);
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
}
