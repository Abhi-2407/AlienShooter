using UnityEngine;

public class RedEnemy : EnemyController
{
    public float redMoveSpeed = 1.5f;

    void Start()
    {
        isHorizontalEnemy = true;

        gameObject.tag = "RedEnemy";
        spawnPoint = GameManager.Instance.missilePoints[1];

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
        // Call base method and add red enemy specific behavior
        base.StopHorizontalMovement();
    }
}