using UnityEngine;

public class CloudController : MonoBehaviour
{
    [Header("Cloud Movement Settings")]
    public float moveSpeed = 1f;
    public float directionChangeInterval = 5f;
    public float horizontalBoundary = 10f;
    
    [Header("Cloud Properties")]
    public bool isMainCloud = false; // Set to true for one cloud to control all others
    
    private Vector2 currentDirection = Vector2.right;
    private float lastDirectionChangeTime;
    private static Vector2 globalDirection = Vector2.right;
    private static float globalSpeed = 1f;
    private static bool directionChanged = false;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Initialize timing
        lastDirectionChangeTime = Time.time;
        
        // If this is the main cloud, initialize global settings
        if (isMainCloud)
        {
            globalSpeed = moveSpeed;
            globalDirection = Vector2.right;
        }
        
        // Set initial velocity
        UpdateMovement();
    }
    
    void Update()
    {
        // Only the main cloud handles direction changes
        if (isMainCloud)
        {
            HandleDirectionChange();
        }
        
        // All clouds update their movement based on global settings
        UpdateMovement();
        
        // Handle boundary wrapping
        HandleBoundaryWrapping();
    }
    
    void HandleDirectionChange()
    {
        // Check if it's time to change direction
        if (Time.time - lastDirectionChangeTime >= directionChangeInterval)
        {
            // Change direction
            globalDirection = globalDirection == Vector2.right ? Vector2.left : Vector2.right;
            directionChanged = true;
            lastDirectionChangeTime = Time.time;
            
            Debug.Log($"Cloud direction changed to: {(globalDirection == Vector2.right ? "Right" : "Left")}");
        }
    }
    
    void UpdateMovement()
    {
        // Use global speed and direction for all clouds
        if (rb != null)
        {
            rb.linearVelocity = globalDirection * globalSpeed;
        }
        
        // Update local direction for this cloud
        currentDirection = globalDirection;
    }
    
    void HandleBoundaryWrapping()
    {
        // Wrap clouds around screen boundaries
        Vector3 pos = transform.position;
        
        // If cloud goes too far left, wrap to right
        if (pos.x < -horizontalBoundary)
        {
            pos.x = horizontalBoundary;
            transform.position = pos;
        }
        // If cloud goes too far right, wrap to left
        else if (pos.x > horizontalBoundary)
        {
            pos.x = -horizontalBoundary;
            transform.position = pos;
        }
    }
    
    // Public methods for external control
    public void SetSpeed(float speed)
    {
        globalSpeed = speed;
        moveSpeed = speed;
    }
    
    public void SetDirectionChangeInterval(float interval)
    {
        directionChangeInterval = interval;
    }
    
    public void SetHorizontalBoundary(float boundary)
    {
        horizontalBoundary = boundary;
    }
    
    // Method to force direction change (can be called from other scripts)
    public void ForceDirectionChange()
    {
        if (isMainCloud)
        {
            globalDirection = globalDirection == Vector2.right ? Vector2.left : Vector2.right;
            directionChanged = true;
            lastDirectionChangeTime = Time.time;
        }
    }
    
    // Get current movement info
    public Vector2 GetCurrentDirection()
    {
        return currentDirection;
    }
    
    public float GetCurrentSpeed()
    {
        return globalSpeed;
    }
    
    public bool IsDirectionChanged()
    {
        bool changed = directionChanged;
        directionChanged = false; // Reset the flag after reading
        return changed;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw boundary lines in scene view
        Gizmos.color = Color.cyan;
        Vector3 leftBoundary = new Vector3(-horizontalBoundary, transform.position.y, 0);
        Vector3 rightBoundary = new Vector3(horizontalBoundary, transform.position.y, 0);
        
        Gizmos.DrawLine(leftBoundary + Vector3.up * 2, leftBoundary + Vector3.down * 2);
        Gizmos.DrawLine(rightBoundary + Vector3.up * 2, rightBoundary + Vector3.down * 2);
    }
}
