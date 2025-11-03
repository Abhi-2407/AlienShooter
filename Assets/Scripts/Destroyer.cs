using Fusion;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [Header("Effects")]
    public GameObject explosionEffect;

    NetworkRunner runner;

    void Start()
    {
        runner = FusionConnector.instance.NetworkRunner;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BlueEnemy"))
        {
            HandleBlueMissile(other.gameObject);
        }

        if (other.CompareTag("RedEnemy"))
        {
            HandleRedMissile(other.gameObject);
        }
    }

    void HandleBlueMissile(GameObject missile)
    {
        CreateExplosionEffect(missile.transform);

        PlayExplosionSound();

        if (GameManager.Instance.IsSinglePlayerMode)
        {
            Destroy(missile);
            GameManager.Instance.SpawnBlueMissile();
        }
        else
        {
            if (missile.GetComponent<NetworkObject>().HasStateAuthority)
            {
                GameManager.Instance.SpawnBlueMissile(runner);
                runner.Despawn(missile.GetComponent<NetworkObject>());
            }
        }
    }

    void HandleRedMissile(GameObject missile)
    {
        CreateExplosionEffect(missile.transform);

        PlayExplosionSound();

        if (GameManager.Instance.IsSinglePlayerMode)
        {
            Destroy(missile);
            GameManager.Instance.SpawnRedMissile();
        }
        else
        {
            if (missile.GetComponent<NetworkObject>().HasStateAuthority)
            {
                GameManager.Instance.SpawnRedMissile(runner);
                runner.Despawn(missile.GetComponent<NetworkObject>());
            }
        }
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
