using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Fusion;

public enum GameState
{
    DEFAULT,
    START,
    OVER
}

public class GameManager : MonoBehaviour
{
    [Header("Bot Settings")]
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
    
    [Header("References")]
    public UIManager uiManager;
    public EnemySpawner enemySpawner;
    public FishSpawner fishSpawner;
    public SpaceshipSpawner spaceshipSpawner;

    [Header("Spawn Settings")]
    public Transform[] shipPoints;
    public Transform[] missilePoints;

    public NetworkPlayer localPlayer;

    [Header("Local Player Objects")]
    public NetworkObject blueShipPrefab;
    public NetworkObject blueMissilePrefab;

    [Header("Remote Player Objects")]
    public NetworkObject redShipPrefab;
    public NetworkObject redMissilePrefab;

    NetworkRunner runner;

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
        runner = FusionConnector.instance.NetworkRunner;

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
    }
    
    void Update()
    {
        if (gameState == GameState.START)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void StartGame()
    {
        player1Score = 0;
        player2Score = 0;
        gameState = GameState.START;

        gameTime = 0f;

        fishSpawner.StartSpawning();

        if (IsSinglePlayerMode)
        {
            SpawnShipForSinglePlayer();
            SpawnEnemyForSinglePlayer();
        }
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

            if (i == 0)
            {
                txt = "GO!";
                AudioManager.Instance.GoMusic();
            }
            else
            {
                AudioManager.Instance.CountDownMusic();
            }
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
        StartGame();
    }

    public void PlayerSpawn(NetworkRunner runner)
    {
        SpawnShipsForMultiPlayer(runner);
        SpawnMissileForMultiPlayer(runner);
    }

    public void SpawnShipsForMultiPlayer(NetworkRunner runner)
    {
        if (runner.LocalPlayer.PlayerId == 1)
        {
            SpawnBlueShip(runner);
        }
        else
        {
            SpawnRedShip(runner);
        }
    }

    public void SpawnBlueShip(NetworkRunner runner)
    {
        runner.Spawn(blueShipPrefab, shipPoints[0].position, Quaternion.identity);
    }

    public void SpawnRedShip(NetworkRunner runner)
    {
        runner.Spawn(redShipPrefab, shipPoints[1].position, Quaternion.identity);
    }

    void SpawnShipForSinglePlayer()
    {
        SpawnBlueShip();
        SpawnRedShip();
    }

    public void SpawnBlueShip()
    {
        Instantiate(blueShipPrefab, shipPoints[0].position, Quaternion.identity);
    }

    public void SpawnRedShip()
    {
        Instantiate(redShipPrefab, shipPoints[1].position, Quaternion.identity);
    }

    public void SpawnMissileForMultiPlayer(NetworkRunner runner)
    {
        if (runner.LocalPlayer.PlayerId == 1)
        {
            SpawnBlueMissile(runner);
        }
        else
        {
            SpawnRedMissile(runner);
        }
    }

    public void SpawnBlueMissile(NetworkRunner runner)
    {
        runner.Spawn(blueMissilePrefab, missilePoints[0].position, Quaternion.identity);
    }

    public void SpawnRedMissile(NetworkRunner runner)
    {
        runner.Spawn(redMissilePrefab, missilePoints[1].position, Quaternion.identity);
    }

    void SpawnEnemyForSinglePlayer()
    {
        SpawnBlueMissile();
        SpawnRedMissile();
    }

    public void SpawnBlueMissile()
    {
        Instantiate(blueMissilePrefab, missilePoints[0].position, Quaternion.identity);
    }

    public void SpawnRedMissile()
    {
        Instantiate(redMissilePrefab, missilePoints[1].position, Quaternion.identity);
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
    public void SetBotDifficulty(int clickInterval)
    {
        ButtonManager.Instance.botClickInterval = clickInterval;
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
    }

    public void GameOver()
    {
        gameState = GameState.OVER;

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

            AudioManager.Instance.PlayDrawMusic();    
        }

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

    public void HandleSpaceShip(SpaceshipController.SpaceshipType spaceshipType, Vector3 pos)
    {
        StartCoroutine(IEHandleSpaceShip(spaceshipType, pos));
    }

    public IEnumerator IEHandleSpaceShip(SpaceshipController.SpaceshipType spaceshipType, Vector3 pos)
    {        
        yield return new WaitForSeconds(1.0f);

        if (!IsSinglePlayerMode)
        {
            if (spaceshipType == SpaceshipController.SpaceshipType.Blue && localPlayer.playerID == 1)
            {
                Vector3 offset = new Vector3(Random.Range(-2.5f, 2.5f), 0, 0);
                SpawnBlueShip(runner);
            }
            if (spaceshipType == SpaceshipController.SpaceshipType.Red && localPlayer.playerID != 1)
            {
                Vector3 offset = new Vector3(Random.Range(-2.5f, 2.5f), 0, 0);
                SpawnRedShip(runner);
            }
        }
        else
        {
            if (spaceshipType == SpaceshipController.SpaceshipType.Blue)
            {
                SpawnBlueShip();
            }

            if (spaceshipType == SpaceshipController.SpaceshipType.Red)
            {
                SpawnRedShip();
            }
        }
    }

    public void Rpc_HandleSpaceShip(SpaceshipController.SpaceshipType spaceshipType, Vector3 pos, Vector3 offset)
    {
        StartCoroutine(Rpc_IEHandleSpaceShip(spaceshipType, pos, offset));
    }

    public IEnumerator Rpc_IEHandleSpaceShip(SpaceshipController.SpaceshipType spaceshipType, Vector3 pos, Vector3 offset)
    {
        GameObject go;

        if(spaceshipType == SpaceshipController.SpaceshipType.Blue)
        {
            go = GameObject.Find("SpaceShip_Blue(Clone)");
        }
        else
        {
            go = GameObject.Find("SpaceShip_Red(Clone)");
        }

        //go.SetActive(false);                
        yield return new WaitForSeconds(1.0f);
        go.GetComponent<SpaceshipController>().isActive = true;
        go.transform.position = pos + offset;
        go.SetActive(true);
        go.transform.GetComponent<SpriteRenderer>().enabled = true;
        go.transform.GetComponent<Collider2D>().enabled = true;

        for (int i = 0; i < go.transform.childCount; i++)
        {
            go.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}