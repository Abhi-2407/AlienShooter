using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource CountDownSource;
    public AudioSource sfxSource;
    public AudioSource fishCapture;
    
    [Header("Music")]
    public AudioClip countDownMusic;
    public AudioClip goMusic;
    public AudioClip backgroundMusic;
    public AudioClip victoryMusic;
    public AudioClip losMusic;
    
    [Header("Sound Effects")]
    public AudioClip explosionSound;
    public AudioClip missileDropSound;
    public AudioClip FishCaptureSound;
   
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

    }

    public void CountDownMusic()
    {
        if (countDownMusic != null && CountDownSource != null)
        {
            CountDownSource.clip = countDownMusic;
            CountDownSource.loop = false;
            CountDownSource.Play();
        }
    }

    public void GoMusic()
    {
        if (goMusic != null && CountDownSource != null)
        {
            CountDownSource.clip = goMusic;
            CountDownSource.loop = false;
            CountDownSource.Play();
        }
    }

    public void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayVictoryMusic()
    {
        musicSource.clip = victoryMusic;
        musicSource.loop = false;
        musicSource.Play();
    }

    public void PlayLosMusic()
    {
        musicSource.clip = losMusic;
        musicSource.loop = false;
        musicSource.Play();
    }

    public void PlayDrawMusic()
    {
        musicSource.clip = losMusic;
        musicSource.loop = false;
        musicSource.Play();
    }

    public void PlayFishCaptureSound()
    {
        fishCapture.clip = FishCaptureSound;
        fishCapture.loop = false;
        fishCapture.PlayOneShot(FishCaptureSound);
    }

    public void PlayExplosionSound()
    {
        sfxSource.clip = explosionSound;
        sfxSource.volume = 1.0f;
        sfxSource.loop = false;
        sfxSource.PlayOneShot(explosionSound);
    }

    public void PlayMissileDropSound()
    {
        sfxSource.clip = missileDropSound;
        sfxSource.volume = 0.4f;
        sfxSource.loop = false;
        sfxSource.PlayOneShot(missileDropSound);
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

