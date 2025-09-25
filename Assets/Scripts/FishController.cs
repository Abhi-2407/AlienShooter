using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    [Header("Fish Settings")]
    public float swimSpeed = 2f;
    public float rotationSpeed = 5f;
    public float changeDirectionInterval = 3f;
    public float minSwimSpeed = 1f;
    public float maxSwimSpeed = 4f;

    [Header("Swimming Area")]
    public Vector2 swimAreaCenter = Vector2.zero;
    public Vector2 swimAreaSize = new Vector2(10f, 6f);

    [Header("Fish Behavior")]
    public bool canSwimUpDown = true;
    public bool canSwimLeftRight = true;
    public float verticalSwimRange = 2f;
    public float horizontalSwimRange = 4f;

    [Header("Light Catching System")]
    public float catchSpeed = 3f;
    public float catchDistance = 0.5f;
    public int catchScoreValue = 5;

    private Vector2 currentDirection;
    private float nextDirectionChangeTime;
    private Vector2 targetPosition;
    private bool isMovingToTarget = false;
    private Vector2 swimBoundsMin;
    private Vector2 swimBoundsMax;
    private SpriteRenderer spriteRenderer;

    // Light catching variables
    private bool isCaught = false;
    private Transform catchingSpaceship = null;
    private Vector2 originalSwimSpeed;
    private bool wasMovingToTarget;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Calculate swimming bounds
        CalculateSwimBounds();

        // Set initial random direction
        SetRandomDirection();

        // Set random swim speed
        swimSpeed = Random.Range(minSwimSpeed, maxSwimSpeed);

        // Set initial target position
        SetRandomTargetPosition();

		// Randomize initial rotation (yaw only)
		transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    void Update()
    {
        if (isCaught)
        {
            HandleCatching();
        }
        else
        {
            if (isMovingToTarget)
            {
                MoveToTarget();
            }
		else
		{
			SwimInDirection();
		}

            // Check if it's time to change direction
            if (Time.time >= nextDirectionChangeTime)
            {
                ChangeDirection();
            }

			// Keep fish within bounds
			KeepWithinBounds();
        }

		// Enforce yaw-only rotation: zero X and Z, keep Y
		ConstrainRotationToYaw();
    }

    void CalculateSwimBounds()
    {
        swimBoundsMin = new Vector2(
            swimAreaCenter.x - swimAreaSize.x / 2f,
            swimAreaCenter.y - swimAreaSize.y / 2f
        );
        swimBoundsMax = new Vector2(
            swimAreaCenter.x + swimAreaSize.x / 2f,
            swimAreaCenter.y + swimAreaSize.y / 2f
        );
    }

    void SetRandomDirection()
    {
        float angle = Random.Range(0f, 360f);
        currentDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        currentDirection.Normalize();

        // Adjust direction based on allowed movement
        if (!canSwimLeftRight)
        {
            currentDirection.x = 0;
        }
        if (!canSwimUpDown)
        {
            currentDirection.y = 0;
        }

        currentDirection.Normalize();
    }

    void SetRandomTargetPosition()
    {
        targetPosition = new Vector2(
            Random.Range(swimBoundsMin.x, swimBoundsMax.x),
            Random.Range(swimBoundsMin.y, swimBoundsMax.y)
        );
        isMovingToTarget = true;
    }

    void MoveToTarget()
    {
        Vector2 directionToTarget = (targetPosition - (Vector2)transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, swimSpeed * Time.deltaTime);

        // Rotate towards movement direction
        if (directionToTarget != Vector2.zero)
        {
			float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
			Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, swimSpeed * Time.deltaTime);
        }

        // Check if reached target
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            isMovingToTarget = false;
            SetRandomDirection();
        }
    }

    void SwimInDirection()
    {
        // Move in current direction
        Vector2 movement = currentDirection * swimSpeed * Time.deltaTime;
        transform.position += (Vector3)movement;

        // Rotate towards movement direction
        if (currentDirection != Vector2.zero)
        {
			float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
			Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

	private void ConstrainRotationToYaw()
	{
		Vector3 euler = transform.eulerAngles;
		if (euler.x != 0f || euler.z != 0f)
		{
			transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
		}
	}

    void ChangeDirection()
    {
        // Randomly choose between direction change or target-based movement
        if (Random.Range(0f, 1f) < 0.7f) // 70% chance for target-based movement
        {
            SetRandomTargetPosition();
        }
        else
        {
            SetRandomDirection();
        }

        // Set next direction change time
        nextDirectionChangeTime = Time.time + Random.Range(changeDirectionInterval * 0.5f, changeDirectionInterval * 1.5f);

        // Occasionally change swim speed
        if (Random.Range(0f, 1f) < 0.3f) // 30% chance
        {
            swimSpeed = Random.Range(minSwimSpeed, maxSwimSpeed);
        }
    }

    void KeepWithinBounds()
    {
        Vector2 currentPos = transform.position;
        bool needsReposition = false;

        // Check horizontal bounds
        if (currentPos.x < swimBoundsMin.x)
        {
            currentPos.x = swimBoundsMin.x;
            needsReposition = true;
        }
        else if (currentPos.x > swimBoundsMax.x)
        {
            currentPos.x = swimBoundsMax.x;
            needsReposition = true;
        }

        // Check vertical bounds
        if (currentPos.y < swimBoundsMin.y)
        {
            currentPos.y = swimBoundsMin.y;
            needsReposition = true;
        }
        else if (currentPos.y > swimBoundsMax.y)
        {
            currentPos.y = swimBoundsMax.y;
            needsReposition = true;
        }

        if (needsReposition)
        {
            transform.position = currentPos;
            // Change direction when hitting bounds
            SetRandomDirection();
        }
    }

    public void SetSwimArea(Vector2 center, Vector2 size)
    {
        swimAreaCenter = center;
        swimAreaSize = size;
        CalculateSwimBounds();
    }

    public void SetSwimSpeed(float speed)
    {
        swimSpeed = Mathf.Clamp(speed, minSwimSpeed, maxSwimSpeed);
    }

    public void SetSwimBounds(Vector2 min, Vector2 max)
    {
        swimBoundsMin = min;
        swimBoundsMax = max;
        swimAreaCenter = (min + max) / 2f;
        swimAreaSize = max - min;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if fish enters a light collider
        if (other.CompareTag("BlueLight") || other.CompareTag("RedLight"))
        {
            if (!isCaught)
            {
                StartCatching(other.transform);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Check if fish exits a light collider
        if ((other.CompareTag("BlueLight") || other.CompareTag("RedLight")) && isCaught)
        {
            if (catchingSpaceship == other.transform)
            {
                StopCatching();
            }
        }
    }

    void StartCatching(Transform spaceship)
    {
        if (isCaught) return;

        isCaught = true;
        catchingSpaceship = spaceship;

        // Store original state
        originalSwimSpeed = new Vector2(swimSpeed, 0);
        wasMovingToTarget = isMovingToTarget;

        // Stop normal swimming behavior
        isMovingToTarget = false;
        currentDirection = Vector2.zero;

        Debug.Log($"Fish caught by {spaceship.tag}!");
    }

    void StopCatching()
    {
        if (!isCaught) return;

        isCaught = false;
        catchingSpaceship = null;

        // Resume normal swimming behavior
        isMovingToTarget = wasMovingToTarget;
        if (!isMovingToTarget)
        {
            SetRandomDirection();
        }

        Debug.Log("Fish released from light!");
    }

    void HandleCatching()
    {
        if (catchingSpaceship == null)
        {
            StopCatching();
            return;
        }

        // Move fish towards the spaceship
        Vector2 directionToSpaceship = (catchingSpaceship.position - transform.position).normalized;
        Vector2 movement = directionToSpaceship * catchSpeed * Time.deltaTime;
        transform.position += (Vector3)movement;

        // Rotate fish towards spaceship
        if (directionToSpaceship != Vector2.zero)
        {
            float angle = Mathf.Atan2(directionToSpaceship.y, directionToSpaceship.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.up);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 2f * Time.deltaTime);
        }

        // Check if fish is close enough to spaceship to be "caught"
        float distanceToSpaceship = Vector2.Distance(transform.position, catchingSpaceship.position);
        if (distanceToSpaceship <= catchDistance)
        {
            OnFishCaught();
        }
    }

    void OnFishCaught()
    {
        // Add score based on spaceship type
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(catchScoreValue);

            // Add specific spaceship score if we can determine the type
            if (catchingSpaceship != null)
            {
                SpaceshipController spaceshipController = catchingSpaceship.GetComponent<SpaceshipController>();
                if (spaceshipController != null)
                {
                    gameManager.AddSpaceshipScore(spaceshipController.spaceshipType, catchScoreValue);
                }
            }
        }

        AudioManager.Instance.PlayExplosionFishCaptureSound();

        Debug.Log($"Fish caught! +{catchScoreValue} points");

        // Destroy the fish
        Destroy(gameObject);
    }

    public bool IsCaught()
    {
        return isCaught;
    }

    public Transform GetCatchingSpaceship()
    {
        return catchingSpaceship;
    }

    void OnDrawGizmosSelected()
    {
        // Draw swimming area bounds
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(swimAreaCenter, swimAreaSize);

        // Draw current direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, currentDirection * 2f);

        // Draw catch distance if being caught
        if (isCaught && catchingSpaceship != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(catchingSpaceship.position, catchDistance);
        }
    }
}

