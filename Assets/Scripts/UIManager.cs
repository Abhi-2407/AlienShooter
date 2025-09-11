using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI timeText;
    public Slider healthBar;
    public Image healthBarFill;
    
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI newHighScoreText;
    public Button restartButton;
    public Button quitButton;
    
    [Header("Pause UI")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button pauseQuitButton;
    
    [Header("Wave Complete UI")]
    public GameObject waveCompletePanel;
    public TextMeshProUGUI waveCompleteText;
    
    [Header("Health Bar Colors")]
    public Color healthyColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color dangerColor = Color.red;
    
    private PlayerController player;
    
    void Start()
    {
        // Find player
        player = FindObjectOfType<PlayerController>();
        
        // Setup button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        
        if (pauseQuitButton != null)
        {
            pauseQuitButton.onClick.AddListener(QuitGame);
        }
        
        // Initialize UI
        UpdateScore(0);
        UpdateLives(3);
        UpdateWave(1);
        UpdateTime(0f);
        
        // Hide panels
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (waveCompletePanel != null)
            waveCompletePanel.SetActive(false);
    }
    
    void Update()
    {
        // Update health bar
        if (player != null && healthBar != null)
        {
            float healthPercentage = (float)player.GetHealth() / player.GetMaxHealth();
            healthBar.value = healthPercentage;
            
            // Update health bar color
            if (healthBarFill != null)
            {
                if (healthPercentage > 0.6f)
                    healthBarFill.color = healthyColor;
                else if (healthPercentage > 0.3f)
                    healthBarFill.color = warningColor;
                else
                    healthBarFill.color = dangerColor;
            }
        }
    }
    
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
    
    public void UpdateHighScore(int highScore)
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }
    
    public void UpdateLives(int lives)
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives.ToString();
        }
    }
    
    public void UpdateWave(int wave)
    {
        if (waveText != null)
        {
            waveText.text = "Wave: " + wave.ToString();
        }
    }
    
    public void UpdateTime(float time)
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void ShowGameOver(int finalScore, int highScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + finalScore.ToString();
            }
            
            if (newHighScoreText != null)
            {
                if (finalScore >= highScore)
                {
                    newHighScoreText.text = "NEW HIGH SCORE!";
                    newHighScoreText.color = Color.yellow;
                }
                else
                {
                    newHighScoreText.text = "High Score: " + highScore.ToString();
                    newHighScoreText.color = Color.white;
                }
            }
        }
    }
    
    public void ShowPauseMenu(bool isPaused)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
    }
    
    public void ShowWaveComplete(int wave)
    {
        if (waveCompletePanel != null)
        {
            waveCompletePanel.SetActive(true);
            
            if (waveCompleteText != null)
            {
                waveCompleteText.text = "Wave " + wave + " Complete!";
            }
            
            // Hide after 3 seconds
            Invoke("HideWaveComplete", 3f);
        }
    }
    
    void HideWaveComplete()
    {
        if (waveCompletePanel != null)
        {
            waveCompletePanel.SetActive(false);
        }
    }
    
    public void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    public void QuitGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
    
    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }
    
    public void SetHealthBarVisible(bool visible)
    {
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(visible);
        }
    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }
}
