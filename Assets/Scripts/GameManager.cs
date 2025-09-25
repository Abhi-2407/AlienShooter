using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using static Fusion.Sockets.NetBitBuffer;

public enum GameState
{
    DEFAULT,
    START,
    OVER
}

public class GameManager : MonoBehaviour
{
    //[Header("Game Settings")]
    //public int score = 0;
    //public int highScore = 0;
    //public int lives = 3;

    [Header("Bot Settings")]
    public int botDifficulty = 0;
    public bool IsSinglePlayerMode;
    public GameObject waitingForPlayersUI;

    [Header("Game Logic")]
    public bool isPlayer1Turn = true; // true = player1's turn, false = player2's turn
    public GameState gameState = GameState.DEFAULT;
    public bool isPaused = false;

    [Header("Countdown Settings")]
    private bool isCountdownRunning = false; // Track if countdown is currently running   
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 1f; // Duration for each countdown number

    [Header("Specific references")]
    public GameTimer gameTimer;

    [Header("Game Over UI")]
    public GameObject GameoverScreen;
    public GameObject YouWin;
    public GameObject YouLoss;
    public GameObject Tie;

    [Header("Spaceship Scoring")]
    public int player1Score = 0;
    public int player2Score = 0;

    [Header("UI References")]
    public TextMeshProUGUI player1Txt;
    public TextMeshProUGUI player2Txt;

    [Header("Game State")]
    public float gameTime = 0f;
    public int wave = 1;
    public int enemiesKilled = 0;
    
    [Header("References")]
    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public FishSpawner fishSpawner;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    public NetworkPlayer localPlayer;

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
        //highScore = PlayerPrefs.GetInt("HighScore", 0);
        
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
        //StartGame();
    }
    
    void Update()
    {
        if (gameState == GameState.START)
        {
            gameTime += Time.deltaTime;
            UpdateUI();
        }
        
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void StartGame()
    {
        player1Score = 0;
        player2Score = 0;
        gameState = GameState.START;

        //score = 0;
        //lives = 3;
        gameTime = 0f;
        wave = 1;
        enemiesKilled = 0;
        
        Time.timeScale = 1f;
        UpdateUI();

        fishSpawner.StartSpawning();
    }


    public void StartCountdown()
    {
        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        isCountdownRunning = true;

        // Show countdown panel
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        for (int i = 3; i >= 0; i--)
        {
            string txt = i.ToString();

            if (i == 0) txt = "GO!";
            AssignTime(txt);
            yield return new WaitForSeconds(countdownDuration);
        }

        // Hide countdown panel
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        isCountdownRunning = false;

        // Start multiplayer timer
        if (gameTimer != null)
        {
            gameTimer.StartTimer();
        }

        // Initialize the game after countdown
        InitializeGame();
    }

    public void InitializeGame()
    {
        // Start the game
        StartGame();
    }


    void AssignTime(string txt)
    {
        if (countdownText != null)
        {
            countdownText.text = txt;
            countdownText.transform.localScale = Vector3.zero;
            countdownText.transform.DOScale(Vector3.one, countdownDuration * 0.3f).SetEase(Ease.OutBack);
        }
    }

    // Method to change bot difficulty during runtime
    public void SetBotDifficulty(int difficulty)
    {
        botDifficulty = difficulty;
    }

    public void InitializeSinglePlayerGame()
    {
        Debug.Log("[GameManager] Initializing single player game");

        IsSinglePlayerMode = true;
        StartCountdown();
    }

    public void InitializeMultiplayerGame()
    {
        Debug.Log("[GameInitializer] Initializing multiplayer game");

        // Hide waiting UI, show game start UI
        if (waitingForPlayersUI != null)
            waitingForPlayersUI.SetActive(false);

        IsSinglePlayerMode = false;
        StartCountdown();
    }

    public void AddScore(int points)
    {
       // if (gameState == GameState.OVER) return;

       // score += points;
       // enemiesKilled++;

       // // Check for high score
       // if (score > highScore)
       // {
       //     highScore = score;
       //     PlayerPrefs.SetInt("HighScore", highScore);
       // }

       // Check for wave progression

       //CheckWaveProgression();

       //UpdateUI();
    }
    
    public void AddSpaceshipScore(SpaceshipController.SpaceshipType spaceshipType, int points)
    {
        if (gameState == GameState.OVER) return;
        
        switch (spaceshipType)
        {
            case SpaceshipController.SpaceshipType.Red:
                player2Score += points;
                uiManager.player2ScoreTxt.text = "" + player2Score;
                break;
            case SpaceshipController.SpaceshipType.Blue:
                player1Score += points;
                uiManager.player1ScoreTxt.text = "" + player1Score;
                break;
        }
        
        // Add to main score as well
        AddScore(points);
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
        //if (gameState == GameState.OVER) return;
        
        //lives--;
        //UpdateUI();
        
        //if (lives <= 0)
        //{
        //    GameOver();
        //}
        //else
        //{
        //    // Respawn player or show respawn message
        //    RespawnPlayer();
        //}
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
        gameState = GameState.OVER;
        Time.timeScale = 0f;

        int player1Score_ = 0;
        int player2Score_ = 0;

        string message = "Game Over!";

        if (!IsSinglePlayerMode && localPlayer.playerID == 1)
        {
            player1Score_ = player2Score;
            player2Score_ = player1Score;
        }
        else
        {
            player1Score_ = player1Score;
            player2Score_ = player2Score;
        }

        if (player1Score_ > player2Score_)
        {
            YouWin.SetActive(true);
            YouLoss.SetActive(false);
            Tie.SetActive(false);

            message = "won";

            AudioManager.Instance.PlayVictoryMusic();
        }
        else if(player1Score_ < player2Score_)
        {
            YouLoss.SetActive(true);
            YouWin.SetActive(false);
            Tie.SetActive(false);

            message = "lost";

            AudioManager.Instance.PlayLosMusic();
        }
        else
        {
            Tie.SetActive(true);
            YouWin.SetActive(false);
            YouLoss.SetActive(false);

            message = "draw";

            AudioManager.Instance.PlayGameOverMusic();
        }

        // Send game state update
        SendGameStateUpdate(player1Score_, player2Score_);
        

        IFrameBridge.Instance.PostMatchResult(message, player1Score_, player2Score_);

        GameoverScreen.SetActive(true);
    }

    // Method to send game state updates (following MotorKick pattern)
    public void SendGameStateUpdate(int player1Score_, int player2Score_)
    {
        if (IFrameBridge.Instance != null)
        {
            // Create game state JSON
            var gameState = new
            {
                player1Score = player1Score_,
                player2Score = player2Score_,
                gameover = true,
                timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            string stateJson = JsonUtility.ToJson(gameState);
            IFrameBridge.Instance.PostGameState(stateJson);
        }
    }

    public void TogglePause()
    {
        if (gameState == GameState.OVER) return;
        
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
        //if (uiManager != null)
        //{
        //    uiManager.UpdateScore(score);
        //    uiManager.UpdateLives(lives);
        //    uiManager.UpdateWave(wave);
        //    uiManager.UpdateTime(gameTime);
        //}
    }
    
    //public int GetScore()
    //{
    //    return score;
    //}
    
    //public int GetHighScore()
    //{
    //    return highScore;
    //}
    
    //public int GetLives()
    //{
    //    return lives;
    //}
    
    public int GetWave()
    {
        return wave;
    }
    
    public float GetGameTime()
    {
        return gameTime;
    }

    public void HandleSpaceShip(GameObject go, Vector3 pos)
    {
        StartCoroutine(IEHandleSpaceShip(go, pos));
    }

    public IEnumerator IEHandleSpaceShip(GameObject go, Vector3 pos)
    {
        go.SetActive(false);
        Vector3 offset = new Vector3(Random.Range(-2.5f, 2.5f), 0, 0);
        go.transform.position = pos + offset;
        yield return new WaitForSeconds(1.0f);
        go.GetComponent<SpaceshipController>().isActive = true;
        go.SetActive(true);
    }
}
