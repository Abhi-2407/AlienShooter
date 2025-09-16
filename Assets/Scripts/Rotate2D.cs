using UnityEngine;

/// <summary>
/// A flexible 2D rotation script that provides various rotation modes and controls
/// </summary>
public class Rotate2D : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float baseRotationSpeed = 90f;
    [SerializeField] private RotationDirection rotationDirection = RotationDirection.Clockwise;
    [SerializeField] private bool startRotatingOnAwake = true;
    [SerializeField] private bool useUnscaledTime = false;
    
    [Header("Pulsing Effect")]
    [SerializeField] private bool enablePulsing = false;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseIntensity = 0.5f;
    
    [Header("Randomization")]
    [SerializeField] private bool randomizeSpeedOnStart = false;
    [SerializeField] private float minRandomSpeed = 30f;
    [SerializeField] private float maxRandomSpeed = 180f;
    
    public enum RotationDirection
    {
        Clockwise = -1,
        CounterClockwise = 1
    }
    
    private bool isRotating = true;
    private float currentRotationSpeed;
    private float pulseTimer = 0f;
    private float originalBaseSpeed;
    
    void Awake()
    {
        originalBaseSpeed = baseRotationSpeed;
        currentRotationSpeed = baseRotationSpeed;
        
        if (randomizeSpeedOnStart)
        {
            RandomizeSpeed();
        }
    }
    
    void Start()
    {
        isRotating = startRotatingOnAwake;
    }
    
    void Update()
    {
        if (!isRotating) return;
        
        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float rotationSpeed = currentRotationSpeed * (int)rotationDirection;
        
        // Apply pulsing effect if enabled
        if (enablePulsing)
        {
            pulseTimer += deltaTime * pulseSpeed;
            float pulseMultiplier = 1f + Mathf.Sin(pulseTimer) * pulseIntensity;
            rotationSpeed *= pulseMultiplier;
        }
        
        transform.Rotate(0, 0, rotationSpeed * deltaTime);
    }
    
    /// <summary>
    /// Start rotating the object
    /// </summary>
    public void StartRotating()
    {
        isRotating = true;
    }
    
    /// <summary>
    /// Stop rotating the object
    /// </summary>
    public void StopRotating()
    {
        isRotating = false;
    }
    
    /// <summary>
    /// Toggle rotation on/off
    /// </summary>
    public void ToggleRotating()
    {
        isRotating = !isRotating;
    }
    
    /// <summary>
    /// Check if the object is currently rotating
    /// </summary>
    public bool IsCurrentlyRotating()
    {
        return isRotating;
    }
    
    /// <summary>
    /// Set the rotation speed
    /// </summary>
    public void SetRotationSpeed(float speed)
    {
        currentRotationSpeed = speed;
    }
    
    /// <summary>
    /// Get the current base rotation speed
    /// </summary>
    public float GetBaseRotationSpeed()
    {
        return currentRotationSpeed;
    }
    
    /// <summary>
    /// Reset rotation speed to original value
    /// </summary>
    public void ResetRotationSpeed()
    {
        currentRotationSpeed = originalBaseSpeed;
    }
    
    /// <summary>
    /// Set the rotation direction
    /// </summary>
    public void SetRotationDirection(RotationDirection direction)
    {
        rotationDirection = direction;
    }
    
    /// <summary>
    /// Get the current rotation direction
    /// </summary>
    public RotationDirection GetRotationDirection()
    {
        return rotationDirection;
    }
    
    /// <summary>
    /// Reverse the current rotation direction
    /// </summary>
    public void ReverseDirection()
    {
        rotationDirection = rotationDirection == RotationDirection.Clockwise 
            ? RotationDirection.CounterClockwise 
            : RotationDirection.Clockwise;
    }
    
    /// <summary>
    /// Enable or disable the pulsing effect
    /// </summary>
    public void EnablePulsing(bool enable)
    {
        enablePulsing = enable;
    }
    
    /// <summary>
    /// Set the pulse effect parameters
    /// </summary>
    public void SetPulseSettings(float speed, float intensity)
    {
        pulseSpeed = speed;
        pulseIntensity = intensity;
    }
    
    /// <summary>
    /// Randomize the rotation speed within the specified range
    /// </summary>
    public void RandomizeSpeed()
    {
        currentRotationSpeed = Random.Range(minRandomSpeed, maxRandomSpeed);
    }
    
    /// <summary>
    /// Set the random speed range
    /// </summary>
    public void SetRandomSpeedRange(float min, float max)
    {
        minRandomSpeed = min;
        maxRandomSpeed = max;
    }
    
    /// <summary>
    /// Rotate to a specific angle over time
    /// </summary>
    public void RotateToAngle(float targetAngle, float duration)
    {
        StartCoroutine(RotateToAngleCoroutine(targetAngle, duration));
    }
    
    /// <summary>
    /// Smoothly rotate to a target angle
    /// </summary>
    private System.Collections.IEnumerator RotateToAngleCoroutine(float targetAngle, float duration)
    {
        float startAngle = transform.eulerAngles.z;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsedTime += deltaTime;
            
            float t = elapsedTime / duration;
            float currentAngle = Mathf.LerpAngle(startAngle, targetAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            
            yield return null;
        }
        
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
    
    /// <summary>
    /// Set whether to use unscaled time
    /// </summary>
    public void SetUseUnscaledTime(bool useUnscaled)
    {
        useUnscaledTime = useUnscaled;
    }
    
    /// <summary>
    /// Get the current rotation angle in degrees
    /// </summary>
    public float GetCurrentRotationAngle()
    {
        return transform.eulerAngles.z;
    }
    
    /// <summary>
    /// Set the rotation angle directly
    /// </summary>
    public void SetRotationAngle(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
