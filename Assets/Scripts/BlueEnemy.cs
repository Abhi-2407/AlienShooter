using UnityEngine;

public class BlueEnemy : EnemyController
{
    public float blueMoveSpeed = 1f;
    
    void Start()
    {
        isHorizontalEnemy = true;

        gameObject.tag = "BlueEnemy";
        spawnPoint = GameManager.Instance.missilePoints[0];

        // Call base Start after setting properties
        base.Start();
    }

    void Update()
    {
        if (GameManager.Instance.IsSinglePlayerMode)
        {
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
            if (Object.HasStateAuthority)
            {
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
    }
}