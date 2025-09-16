using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float boundaryPadding = 0.5f;
    
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    public float bulletSpeed = 10f;
    
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
    private float nextFireTime;
    private Camera mainCamera;
    private Vector2 screenBounds;
    
    void Start()
    {
        mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        currentHealth = maxHealth;
    }
    
    void Update()
    {
        HandleMovement();
        HandleShooting();
    }
    
    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * moveSpeed * Time.deltaTime;
        transform.position += movement;
        
        // Keep player within screen bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x + boundaryPadding, screenBounds.x - boundaryPadding);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y + boundaryPadding, screenBounds.y - boundaryPadding);
        transform.position = clampedPosition;
    }
    
    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = transform.up * bulletSpeed;
            }
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // Trigger game over
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
        }
        
        Destroy(gameObject);
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

