using Fusion;
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
        spawnPoint = EnemySpawner.Instance.spawnPoints[0];

        // Call base Start after setting properties
        base.Start();
    }

    void Update()
    {
        if (GameManager.Instance.IsSinglePlayerMode)
        {
            // Red enemies are more aggressive
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
                // Red enemies are more aggressive
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
        // Call base method and add red enemy specific behavior
        base.StopHorizontalMovement();

        // Red enemies might have additional behavior when stopped
        Debug.Log("Red enemy stopped horizontal movement!");
    }
}
