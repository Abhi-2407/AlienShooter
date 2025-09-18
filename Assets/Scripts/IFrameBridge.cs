using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class IFrameBridge : MonoBehaviour
{
    public static IFrameBridge Instance { get; private set; }

    public bool IsSinglePlayer;
    public bool IsEasyMod;

    // Bot difficulty from platform
    private AIMode botLevel;
    private GameType gameType;

    // Specific references
    public GameManager gameManager;
    public GameTimer gameTimer;

    private bool isInitialized = false;
    private bool gameModeInitialized = false;

    // Score submission tracking
    private bool scoreSubmitted = false;
    private float submitTimeout = 5f;
    private float submitTimer = 0f;

    // Match information
    public static string MatchId { get; private set; } = string.Empty;
    public static string PlayerId { get; private set; } = string.Empty;
    public static string OpponentId { get; private set; } = string.Empty;

    // WebGL external methods (DllImport) matching our platform bridge
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void SendMatchResult(string matchId, string playerId, string opponentId, string outcome, int score, int opponentScore);
    [DllImport("__Internal")] private static extern void SendMatchAbort(string message, string error, string errorCode);     
    [DllImport("__Internal")] private static extern void SendBuildVersion(string version);
    [DllImport("__Internal")] private static extern void SendGameReady();
    [DllImport("__Internal")] private static extern int GetDeviceType();
    [DllImport("__Internal")] private static extern string GetURLParameters();
#endif

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameObject.name = "IFrameBridge";
            DontDestroyOnLoad(gameObject);
            Debug.unityLogger.logEnabled = true;
            isInitialized = true;
            gameModeInitialized = false; // Reset game mode initialization
            Debug.Log("[IFrameBridge] Instance initialized");
        }
        else
        {
            Debug.LogWarning("[IFrameBridge] Multiple instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!isInitialized)
        {
            Debug.LogError("[IFrameBridge] Start called before initialization!");
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR

        SendGameReady();
        SendBuildVersion(Application.version);

        Debug.Log("[IFrameBridge] WebGL: Game ready signal sent successfully");
#endif

        ExtractParametersFromURL();
    }


    private void ExtractParametersFromURL()
    {
        string json = "";

#if UNITY_WEBGL && !UNITY_EDITOR
            // Get parameters from URL in WebGL build
            json = GetURLParameters();
#else
        // Use test data in editor - CHOOSE MODE HERE:

        if (IsSinglePlayer)
        {
            if (IsEasyMod)
            {
                json = "{\"matchId\":\"test_match\",\"playerId\":\"human_player\",\"opponentId\":\"a912345678\"}";
            }
            else
            {
                json = "{\"matchId\":\"test_match\",\"playerId\":\"human_player\",\"opponentId\":\"b912345678\"}";
            }
        }
        else
        {
            json = "{\"matchId\":\"test_match\",\"playerId\":\"player1\",\"opponentId\":\"player2\"}";
        }

        Debug.Log($"[IFrameBridge] Editor/Build: Using test parameters: {json}");

#endif

        InitParamsFromJS(json);
    }

    public void InitParamsFromJS(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("[IFrameBridge] Received null or empty JSON!");
            AbortInitializationError("Empty JSON received");
            return;
        }

        // Prevent multiple initializations
        if (gameModeInitialized)
        {
            Debug.LogWarning("[IFrameBridge] Game mode already initialized, ignoring duplicate InitParamsFromJS call");
            return;
        }

        Debug.Log($"[IFrameBridge] Parsing match parameters from JSON: {json}");

        var data = JsonUtility.FromJson<MatchParams>(json);
        if (data == null)
        {
            throw new ArgumentException("Failed to parse JSON data");
        }

        // Validate and assign the data
        if (
            string.IsNullOrEmpty(data.matchId)
            || string.IsNullOrEmpty(data.playerId)
            || string.IsNullOrEmpty(data.opponentId)
        )
        {
            throw new ArgumentException("Required match parameters are missing or empty");
        }

        MatchId = data.matchId;
        PlayerId = data.playerId;
        OpponentId = data.opponentId;

        if (IsBot(OpponentId))
        {
            // AI MODE
            botLevel = GetBotLevel(OpponentId);

            gameType = GameType.Singleplayer;
            gameModeInitialized = true;

            Debug.Log($"[IFrameBridge] AI MODE INITIALIZED - Bot difficulty: {botLevel}, gameType: {gameType}");

            // Initialize AI game completely separate from multiplayer
            InitializeAIMode();
        }
        else
        {
            // MULTIPLAYER MODE
            gameType = GameType.Multiplayer;
            gameModeInitialized = true;

            Debug.Log($"[IFrameBridge] MULTIPLAYER MODE INITIALIZED - gameType: {gameType}");

            // Initialize multiplayer game completely separate from AI
            InitializeMultiplayerMode();
        }
    }

    private void InitializeAIMode()
    {
        Debug.Log("[IFrameBridge] Initializing AI Mode...");

        // Set bot difficulty based on opponent ID
        if (botLevel == AIMode.Easy)
            gameManager.SetBotDifficulty(0);
        else if (botLevel == AIMode.Hard)
            gameManager.SetBotDifficulty(1);

        // Initialize singleplayer game systems
        gameManager.InitializeSinglePlayerGame();

        Debug.Log("[IFrameBridge] AI Mode initialized successfully");
    }

    private void InitializeMultiplayerMode()
    {
        Debug.Log("[IFrameBridge] Initializing Multiplayer Mode...");

        // Show waiting UI
        gameManager.waitingForPlayersUI.SetActive(true);

        FusionConnector.instance.ConnectToServer(MatchId);

        // For multiplayer mode, disable bot AI (set difficulty to 0 for human vs human)
        gameManager.SetBotDifficulty(0);
        gameManager.IsSinglePlayerMode = false;

        Debug.Log("[IFrameBridge] Multiplayer Mode initialized successfully");
    }

    public bool IsBot(string playerId)
    {
        if (string.IsNullOrEmpty(playerId))
            return false;
        // According to documentation, bot IDs start with "a9" or "b9"
        return playerId.StartsWith("a9") || playerId.StartsWith("b9");
    }

    public AIMode GetBotLevel(string playerId)
    {
        if (playerId.StartsWith("a9"))
            return AIMode.Easy;
        else if (playerId.StartsWith("b9"))
            return AIMode.Hard;
        return AIMode.Easy;
    }

    public void PostMatchResult(string outcome, int score, int opponentScore)
    {
        Debug.Log(
            "[IFrameBridge] Match Result - Outcome: "
                + outcome
                + ", Score: "
                + score.ToString()
                + ", OpponentScore: "
                + opponentScore.ToString()
        );

        // Add 3-second delay before handshake
        StartCoroutine(PostMatchResultWithDelay(outcome, score, opponentScore));
    }

    private System.Collections.IEnumerator PostMatchResultWithDelay(string outcome, int score, int opponentScore)
    {
        Debug.Log("[IFrameBridge] Waiting 3 seconds before sending match result...");
        yield return new WaitForSeconds(3f);

#if UNITY_WEBGL && !UNITY_EDITOR
        SendMatchResult(MatchId, PlayerId, OpponentId, outcome, score, opponentScore);
#else
        Debug.Log($"[Editor] match_result: {{ matchId: {MatchId}, playerId: {PlayerId}, opponentId: {OpponentId}, outcome: {outcome}, score: {score}, score2: {opponentScore} }}");
#endif
    }

    public void PostMatchAbort(string message, string error = "", string errorCode = "")
    {
        Debug.Log($"[Bridge] Sending match_abort: message={message}, error={error}, errorCode={errorCode}");

#if UNITY_WEBGL && !UNITY_EDITOR
        SendMatchAbort(message, error, errorCode);
#else
        Debug.Log($"[Editor] match_abort: {{ message: {message}, error: {error}, errorCode: {errorCode} }}");
#endif
    }

    internal bool IsMobile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetDeviceType() == 1;
#else
        return false;
#endif
    }

    public static class PlayerUtils
    {
        public static bool IsBot(string playerId)
        {
            return !string.IsNullOrEmpty(playerId) && (playerId.StartsWith("a9") || playerId.StartsWith("b9"));
        }
    }

    public void AbortInitializationError(string error)
    {
        PostMatchAbort("Failed to initialize game", error, "INIT_ERROR");
    }   

    // Handle opponent leaves (should trigger player win)
    public void PostOpponentForfeit()
    {
        Debug.Log($"[IFrameBridge] Opponent {OpponentId} forfeited the match");

#if UNITY_WEBGL && !UNITY_EDITOR
        // Use simple approach
        PostMatchAbort("Opponent left the game.", "", "");

        // Send match result with "won" for local player (this will now send two payloads)
        int myScore = gameManager != null ? gameManager.player1Score : 0;
        int opponentScore = gameManager != null ? gameManager.player2Score : 0;
        SendMatchResult(MatchId, PlayerId, OpponentId, "won", myScore, opponentScore);
#endif
    }

    [Serializable]
    private class MatchParams
    {
        public string matchId = string.Empty;
        public string playerId = string.Empty;
        public string opponentId = string.Empty;
    }

    [Serializable]
    private class MatchAbortMessage
    {
        public string type;
        public MatchAbortPayload payload;
    }

    [Serializable]
    private class MatchAbortPayload
    {
        public string message; // reason for the abort to show to user
        public string error; // error if error occurs
        public string errorCode; // for debugging purpose
    }
}
public enum GameType
{
    Singleplayer = 0,
    Multiplayer = 1,
    AIvsAI = 2,
    Practice = 3
}

public enum AIMode
{
    Easy,
    Hard
}
