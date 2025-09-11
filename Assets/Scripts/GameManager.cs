using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int score = 0;
    public int highScore = 0;
    public int lives = 3;
    public bool isGameOver = false;
    public bool isPaused = false;
    
    [Header("Spaceship Scoring")]
    public int redSpaceshipScore = 0;
    public int blueSpaceshipScore = 0;
    public int totalSpaceshipScore = 0;
    
    [Header("Game State")]
    public float gameTime = 0f;
    public int wave = 1;
    public int enemiesKilled = 0;
    
    [Header("References")]
    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    
    public static GameManager Instance { get; private set; }
    
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
        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Initialize UI
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        
        // Initialize enemy spawner
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }
        
        // Start the game
        StartGame();
    }
    
    void Update()
    {
        if (!isGameOver && !isPaused)
        {
            gameTime += Time.deltaTime;
            UpdateUI();
        }
        
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Handle restart input
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }
    
    public void StartGame()
    {
        isGameOver = false;
        isPaused = false;
        score = 0;
        lives = 3;
        gameTime = 0f;
        wave = 1;
        enemiesKilled = 0;
        
        Time.timeScale = 1f;
        UpdateUI();
    }
    
    public void AddScore(int points)
    {
        if (isGameOver) return;
        
        score += points;
        enemiesKilled++;
        
        // Check for high score
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        
        // Check for wave progression
        CheckWaveProgression();
        
        UpdateUI();
    }
    
    public void AddSpaceshipScore(SpaceshipController.SpaceshipType spaceshipType, int points)
    {
        if (isGameOver) return;
        
        switch (spaceshipType)
        {
            case SpaceshipController.SpaceshipType.Red:
                redSpaceshipScore += points;
                break;
            case SpaceshipController.SpaceshipType.Blue:
                blueSpaceshipScore += points;
                break;
        }
        
        totalSpaceshipScore = redSpaceshipScore + blueSpaceshipScore;
        
        // Add to main score as well
        AddScore(points);
        
        Debug.Log($"{spaceshipType} spaceship scored {points} points! Total: {totalSpaceshipScore}");
    }
    
    void CheckWaveProgression()
    {
        // Increase wave every 10 enemies killed
        int newWave = (enemiesKilled / 10) + 1;
        if (newWave > wave)
        {
            wave = newWave;
            OnWaveComplete();
        }
    }
    
    void OnWaveComplete()
    {
        // Increase difficulty
        if (enemySpawner != null)
        {
            enemySpawner.IncreaseDifficulty();
        }
        
        // Show wave complete message
        if (uiManager != null)
        {
            uiManager.ShowWaveComplete(wave);
        }
    }
    
    public void LoseLife()
    {
        if (isGameOver) return;
        
        lives--;
        UpdateUI();
        
        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn player or show respawn message
            RespawnPlayer();
        }
    }
    
    void RespawnPlayer()
    {
        // Find player and reset position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.currentHealth = playerController.maxHealth;
            }
            
            // Reset player position
            player.transform.position = Vector3.zero;
        }
    }
    
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        
        if (uiManager != null)
        {
            uiManager.ShowGameOver(score, highScore);
        }
    }
    
    public void TogglePause()
    {
        if (isGameOver) return;
        
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (uiManager != null)
        {
            uiManager.ShowPauseMenu(isPaused);
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    void UpdateUI()
    {
        if (uiManager != null)
        {
            uiManager.UpdateScore(score);
            uiManager.UpdateLives(lives);
            uiManager.UpdateWave(wave);
            uiManager.UpdateTime(gameTime);
        }
    }
    
    public int GetScore()
    {
        return score;
    }
    
    public int GetHighScore()
    {
        return highScore;
    }
    
    public int GetLives()
    {
        return lives;
    }
    
    public int GetWave()
    {
        return wave;
    }
    
    public float GetGameTime()
    {
        return gameTime;
    }
}
