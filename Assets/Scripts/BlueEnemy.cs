using UnityEngine;

public class BlueEnemy : EnemyController
{
    [Header("Blue Enemy Settings")]
    public Color blueColor = Color.blue;
    public int blueScoreValue = 10;
    public float blueHealth = 40f;
    public float blueMoveSpeed = 1f;
    
    void Start()
    {
        // Set blue enemy specific properties
        maxHealth = Mathf.RoundToInt(blueHealth);
        scoreValue = blueScoreValue;
        isHorizontalEnemy = true;
        //horizontalRange = 3f;
        //moveSpeed = blueMoveSpeed;
        //horizontalSpeed = 1f;
        //verticalSpeed = 1f;
        
        // Set blue color
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = blueColor;
        }
        
        // Set tag for collision detection
        gameObject.tag = "BlueEnemy";
        spawnPoint = EnemySpawner.Instance.spawnPoints[1];

        // Call base Start after setting properties
        base.Start();
    }

    void Update()
    {
        if (GameManager.Instance.IsSinglePlayerMode)
        {
            // Blue enemies are more defensive
            if (!isDead && GameManager.Instance.gameState == GameState.START)
            {
                HandleMovement(spawnPoint.position);  
            }
            if (!isDead && GameManager.Instance.gameState == GameState.OVER)
            {
                StopMovement();
            }
        }
        else
        {
            //if (Object.HasStateAuthority)
            {
                // Blue enemies are more defensive
                if (!isDead && GameManager.Instance.gameState == GameState.START)
                {
                    HandleMovement(spawnPoint.position);
                }
                if (!isDead && GameManager.Instance.gameState == GameState.OVER)
                {
                    StopMovement();
                }
            }
        }
    }
    
    public void StopHorizontalMovement()
    {
        // Call base method and add blue enemy specific behavior
        base.StopHorizontalMovement();
        
        // Blue enemies might have additional behavior when stopped
        //Debug.Log("Blue enemy stopped horizontal movement!");
    }
}
