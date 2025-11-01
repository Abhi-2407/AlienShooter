using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [Header("Effects")]
    public GameObject explosionEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BlueEnemy"))
        {
            HandleBlueMissile(other.gameObject);
        }

        if(other.CompareTag("RedEnemy"))
        {
            HandleRedMissile(other.gameObject);
        }
    }

    void HandleBlueMissile(GameObject missile)
    {
        CreateExplosionEffect(missile.transform);

        PlayExplosionSound();

        GameManager.Instance.SpawnBlueMissile();

        Destroy(missile);
    }

    void HandleRedMissile(GameObject missile)
    {
        CreateExplosionEffect(missile.transform);

        PlayExplosionSound();

        GameManager.Instance.SpawnRedMissile();

        Destroy(missile);
    }

    void CreateExplosionEffect(Transform trans)
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, trans.position, Quaternion.identity);
        }
    }

    void PlayExplosionSound()
    {
        AudioManager.Instance.PlayExplosionSound();
    }
}
