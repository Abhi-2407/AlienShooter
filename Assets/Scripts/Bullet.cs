using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public float lifetime = 5f;
    public int damage = 10;
    public bool isPlayerBullet = true;
    
    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject explosionEffect;
    
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
        
        // Destroy bullet after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // Handle different collision types
        if (isPlayerBullet)
        {
            // Player bullet hits enemy
            if (other.CompareTag("Enemy"))
            {
                EnemyController enemy = other.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                CreateHitEffect();
                Destroy(gameObject);
            }
        }
        else
        {
            // Enemy bullet hits player
            if (other.CompareTag("Player"))
            {
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
                CreateHitEffect();
                Destroy(gameObject);
            }
        }
        
        // Bullet hits boundary
        if (other.CompareTag("Boundary"))
        {
            Destroy(gameObject);
        }
    }
    
    void CreateHitEffect()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }
    
    public void SetBulletProperties(float bulletSpeed, int bulletDamage, bool playerBullet)
    {
        speed = bulletSpeed;
        damage = bulletDamage;
        isPlayerBullet = playerBullet;
        
        if (rb != null)
        {
            rb.linearVelocity = transform.up * speed;
        }
    }
}
