using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip collisionSound;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check collision with enemies
        if (other.CompareTag("BlueEnemy") || other.CompareTag("RedEnemy"))
        {
            HandleEnemyCollision(other.gameObject);
        }
    }

    void HandleEnemyCollision(GameObject enemy)
    {
        // Create explosion effects
        CreateExplosionEffect(enemy.transform);

        // Play collision sound
        AudioManager.Instance.PlayExplosionSound2();

        // Destroy both spaceship and enemy
        DestroyEnemy(enemy);
    }

    void CreateExplosionEffect(Transform trans)
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, trans.position, Quaternion.identity);
        }
    }

    void PlayCollisionSound(Transform trans)
    {
        if (collisionSound != null)
        {
            AudioSource.PlayClipAtPoint(collisionSound, trans.position);
        }
    }

    void DestroyEnemy(GameObject enemy)
    {
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.isDead = true;
        }
        Destroy(enemy);
    }
}
