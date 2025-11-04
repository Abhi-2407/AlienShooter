using Fusion;
using UnityEngine;

public class RedEnemy : NetworkBehaviour
{
    public bool canMove;
    public Rigidbody2D rb;
    public float horizontalSpeed = 1f;
    public float verticalSpeed = 3f;
    private bool movingRight = true;
    public float horizontalRange = 3f;
    public Transform spawnPoint;

    void Start()
    {
        canMove = true;

        gameObject.tag = "RedEnemy";
        spawnPoint = GameManager.Instance.missilePoints[1];

        GameManager.Instance.redMissile = this;
    }

    void Update()
    {
        if (canMove)
        {
            if (GameManager.Instance.IsSinglePlayerMode)
            {
                if (GameManager.Instance.gameState == GameState.START)
                {
                    HandleMovement(spawnPoint.position);
                }
                if (GameManager.Instance.gameState == GameState.OVER)
                {
                    StopMovement();
                }
            }
            else
            {
                if (Object.HasStateAuthority)
                {
                    //if (Input.GetMouseButtonDown(0))
                    //{
                    //    Debug.LogError("Check");
                    //    if (canMove)
                    //    {
                    //        rb.gravityScale = 10f;
                    //        //rb.constraints = RigidbodyConstraints.FreezePositionX;
                    //        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
                    //        canMove = false;
                    //        //  rb.linearVelocity = new Vector2(0f, -verticalSpeed * 10f);

                    //        AudioManager.Instance.PlayMissileDropSound();
                    //    }
                    //}

                    if (GameManager.Instance.gameState == GameState.START)
                    {
                        HandleMovement(spawnPoint.position);
                    }
                    if (GameManager.Instance.gameState == GameState.OVER)
                    {
                        StopMovement();
                    }
                }
            }
        }
    }

    void HandleMovement(Vector3 centerPos)
    {
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

    public void DropMissile()
    {
        //if (canMove)
        //{
        //    canMove = false;
        //    rb.linearVelocity = new Vector2(0f, -verticalSpeed * 10f);

        //    AudioManager.Instance.PlayMissileDropSound();
        //}

        rb.gravityScale = 10f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX;
        canMove = false;
    }

    public void StopMovement()
    {
        rb.linearVelocity = new Vector2(0f, 0f);
        canMove = false;
    }
}