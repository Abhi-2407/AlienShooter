using UnityEngine;

public class EnemyController : MonoBehaviour
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
    public Transform player;
    private Rigidbody2D rb;
    public bool isDead = false;

    public void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        //startPosition = transform.position;
        
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        //if (!isDead && GameManager.Instance.gameState == GameState.START)
        //{
        //    HandleMovement();
        //    HandleShooting();
        //    HandleRotation();
        //}
    }

    public void HandleMovement(Vector3 centerPos)
    {
        if (isHorizontalEnemy)
        {
            HandleHorizontalMovement(centerPos);
        }
        else
        {
            // Basic downward movement
            if (rb != null)
            {
                //rb.linearVelocity = moveDirection * moveSpeed;
            }
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
    
    void HandleShooting()
    {
        if (!canShoot || enemyBulletPrefab == null || firePoint == null) return;
        
        // Check if player is in range
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    public void HandleRotation()
    {
        if (!canRotate || player == null) return;
        
        // Rotate towards player
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    public void Shoot()
    {
        GameObject bullet = Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        
        if (bulletScript != null)
        {
            bulletScript.SetBulletProperties(bulletSpeed, 10, false);
        }
    }
    
    public void StopHorizontalMovement()
    {
        if (isHorizontalEnemy && rb != null)
        {
            // Stop horizontal movement and make enemy come straight down
            rb.linearVelocity = new Vector2(0f, -verticalSpeed * 10f); // Double vertical speed when coming down
            isHorizontalEnemy = false; // Disable horizontal movement
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        // Add score
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
        }
        
        // Create explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        
        // Drop power-up
        DropPowerUp();
        
        // Destroy enemy
        Destroy(gameObject);
    }
    
    void DropPowerUp()
    {
        if (powerUpPrefabs.Length > 0 && Random.Range(0f, 1f) < powerUpDropChance)
        {
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            Instantiate(powerUpPrefabs[randomIndex], transform.position, Quaternion.identity);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(20); // Collision damage
            }
            Die();
        }
    }
    
    public int GetHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
