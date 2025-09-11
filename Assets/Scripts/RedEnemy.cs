using UnityEngine;

public class RedEnemy : EnemyController
{
    [Header("Red Enemy Settings")]
    public Color redColor = Color.red;
    public int redScoreValue = 15;
    public float redHealth = 60f;
    public float redMoveSpeed = 1.5f;
    
    void Start()
    {
        // Set red enemy specific properties
        maxHealth = Mathf.RoundToInt(redHealth);
        scoreValue = redScoreValue;
        isHorizontalEnemy = true;
        //horizontalRange = 3f;
        //moveSpeed = redMoveSpeed;
        //horizontalSpeed = 1f;
        //verticalSpeed = 1f;
        
        // Set red color
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = redColor;
        }
        
        // Set tag for collision detection
        gameObject.tag = "RedEnemy";
        
        // Call base Start after setting properties
        base.Start();
    }
    
    void Update()
    {
        // Red enemies are more aggressive
        if (!isDead)
        {
            HandleMovement();
            HandleShooting();
            HandleRotation();
        }
    }
    
    void HandleShooting()
    {
        // Red enemies shoot more frequently
        if (!canShoot || enemyBulletPrefab == null || firePoint == null) return;
        
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + (fireRate * 0.7f); // 30% faster shooting
            }
        }
    }
    
    public void StopHorizontalMovement()
    {
        // Call base method and add red enemy specific behavior
        base.StopHorizontalMovement();
        
        // Red enemies might have additional behavior when stopped
        Debug.Log("Red enemy stopped horizontal movement!");
    }
}
