using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip gameOverMusic;
    public AudioClip victoryMusic;
    
    [Header("Sound Effects")]
    public AudioClip shootSound;
    public AudioClip enemyHitSound;
    public AudioClip playerHitSound;
    public AudioClip explosionSound;
    public AudioClip powerUpSound;
    public AudioClip buttonClickSound;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    
    public static AudioManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Setup audio sources
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure audio sources
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        
        // Start background music
        PlayBackgroundMusic();
    }
    
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null && musicSource != null)
        {
            musicSource.clip = gameOverMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
    }
    
    public void PlayVictoryMusic()
    {
        if (victoryMusic != null && musicSource != null)
        {
            musicSource.clip = victoryMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
    }
    
    public void PlayShootSound()
    {
        PlaySFX(shootSound);
    }
    
    public void PlayEnemyHitSound()
    {
        PlaySFX(enemyHitSound);
    }
    
    public void PlayPlayerHitSound()
    {
        PlaySFX(playerHitSound);
    }
    
    public void PlayExplosionSound()
    {
        PlaySFX(explosionSound);
    }
    
    public void PlayPowerUpSound()
    {
        PlaySFX(powerUpSound);
    }
    
    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }
    
    void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void PauseMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }
    
    public void MuteAll()
    {
        if (musicSource != null)
            musicSource.mute = true;
        if (sfxSource != null)
            sfxSource.mute = true;
    }
    
    public void UnmuteAll()
    {
        if (musicSource != null)
            musicSource.mute = false;
        if (sfxSource != null)
            sfxSource.mute = false;
    }
}

