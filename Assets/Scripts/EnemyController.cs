using Fusion;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 50f;
    public Vector2 moveDirection = Vector2.down;
    
    [Header("Horizontal Movement")]
    public bool isHorizontalEnemy = false;
    public float horizontalRange = 3f;
    public float horizontalSpeed = 1f;
    public float verticalSpeed = 0.5f;
    //private Vector3 startPosition;
    public Transform spawnPoint;
    private bool movingRight = true;
    
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;
    public int scoreValue = 10;
    
    [Header("Shooting Settings")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float bulletSpeed = 5f;
    public float shootingRange = 8f;
    
    [Header("AI Settings")]
    public bool canShoot = true;
    public bool canRotate = false;
    public float detectionRange = 10f;
    
    [Header("Effects")]
    public GameObject explosionEffect;
    public GameObject[] powerUpPrefabs;
    public float powerUpDropChance = 0.1f;
    
    public float nextFireTime;
    public Rigidbody2D rb;
    public bool isDead = false;

    public void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

    }

    public void HandleMovement(Vector3 centerPos)
    {
        if (isHorizontalEnemy)
        {
            HandleHorizontalMovement(centerPos);
        }

        // Check if enemy is off screen
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
    
    void HandleHorizontalMovement(Vector3 centerPos)
    {
        if (rb == null) return;
        
        // Calculate horizontal movement within range
        float horizontalOffset = transform.position.x - centerPos.x;
        
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
        float currentHorizontalSpeed = movingRight ? horizontalSpeed : -horizontalSpeed;
        rb.linearVelocity = new Vector2(currentHorizontalSpeed, 0);
    }
    
    public void StopHorizontalMovement()
    {
        if (isHorizontalEnemy && rb != null)
        {
            Debug.Log("StopHorizontalMovement");

            // Stop horizontal movement and make enemy come straight down
            rb.linearVelocity = new Vector2(0f, -verticalSpeed * 10f); // Double vertical speed when coming down
            isHorizontalEnemy = false; // Disable horizontal movement

            AudioManager.Instance.PlayMissileDropSound();
        }
    }

    public void StopMovement()
    {
        rb.linearVelocity = new Vector2(0f, 0f); // Double vertical speed when coming down
        isHorizontalEnemy = false; // Disable horizontal movement
    }
}
