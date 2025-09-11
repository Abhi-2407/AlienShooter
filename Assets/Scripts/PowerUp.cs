using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("PowerUp Settings")]
    public PowerUpType powerUpType;
    public float duration = 10f;
    public int value = 1;
    
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 90f;
    
    [Header("Effects")]
    public GameObject collectEffect;
    public AudioClip collectSound;
    
    public enum PowerUpType
    {
        Health,
        MultiShot,
        RapidFire,
        Shield,
        Score,
        ExtraLife
    }
    
    void Update()
    {
        // Move downward
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        
        // Rotate
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Destroy if off screen
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                ApplyPowerUp(player);
                CreateCollectEffect();
                PlayCollectSound();
                Destroy(gameObject);
            }
        }
    }
    
    void ApplyPowerUp(PlayerController player)
    {
        switch (powerUpType)
        {
            case PowerUpType.Health:
                player.currentHealth = Mathf.Min(player.currentHealth + (value * 25), player.maxHealth);
                break;
                
            case PowerUpType.MultiShot:
                // This would require additional implementation in PlayerController
                Debug.Log("Multi-shot power-up collected!");
                break;
                
            case PowerUpType.RapidFire:
                // This would require additional implementation in PlayerController
                Debug.Log("Rapid fire power-up collected!");
                break;
                
            case PowerUpType.Shield:
                // This would require additional implementation in PlayerController
                Debug.Log("Shield power-up collected!");
                break;
                
            case PowerUpType.Score:
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.AddScore(value * 100);
                }
                break;
                
            case PowerUpType.ExtraLife:
                GameManager gameManager2 = FindObjectOfType<GameManager>();
                if (gameManager2 != null)
                {
                    // Add extra life (this would need to be implemented in GameManager)
                    Debug.Log("Extra life power-up collected!");
                }
                break;
        }
    }
    
    void CreateCollectEffect()
    {
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
    }
    
    void PlayCollectSound()
    {
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
    }
    
    public void SetPowerUpType(PowerUpType type)
    {
        powerUpType = type;
    }
    
    public void SetValue(int newValue)
    {
        value = newValue;
    }
    
    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }
}
