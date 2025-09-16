using UnityEngine;

public class Boundary : MonoBehaviour
{
    [Header("Boundary Settings")]
    public bool destroyOnExit = true;
    public bool isTopBoundary = false;
    public bool isBottomBoundary = false;
    public bool isLeftBoundary = false;
    public bool isRightBoundary = false;
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (destroyOnExit)
        {
            // Destroy bullets and enemies that exit the screen
            if (other.CompareTag("Bullet") || other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Handle special boundary behaviors
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // If enemy hits bottom boundary, player loses a life
                if (isBottomBoundary)
                {
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.LoseLife();
                    }
                }
            }
        }
    }
}

