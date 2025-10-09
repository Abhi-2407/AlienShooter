  using UnityEngine;
using Fusion;
using System.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Player Settings")]
    public string playerName = "Player";

    public bool isLocalPlayer;

    [Header("Network Properties")]
    [Networked] public int playerID { get; set; }
    [Networked] public string PlayerName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public bool IsMyTurn { get; set; }
    
    [Header("Game State")]
    public bool isGameStarted = false;
    public int score = 0;

    // Game state tracking
    private bool isInitialized = false;
    public int localPlayerID = 0;
    public bool isMine;
    public bool im1stPlayer;

    // Local components
    private GameManager localGameManager;
    private GameTimer localGameTimer;

    public override void Spawned()
    {
        base.Spawned();

        Debug.Log($"[NetworkPlayer] Spawned() called - Object.IsValid: {Object.IsValid}");

        // Find local components
        localGameManager = FindObjectOfType<GameManager>();
        localGameTimer = FindObjectOfType<GameTimer>();

        // Set local player ID based on ownership
        if (Object.HasStateAuthority)
        {
            localPlayerID = 1; // Host is player 1

            isMine = true;

            isLocalPlayer = true;
            playerID = Runner.LocalPlayer.PlayerId;
            PlayerName = $"Player_{playerID}";

            Debug.Log($"[NetworkPlayer] Spawned as local player {playerID}");

            localGameManager.localPlayer = this;

            if (playerID == 1)
            {
                im1stPlayer = true;
                localGameManager.isPlayer1Turn = false;

                ButtonManager.Instance.blueButton.gameObject.SetActive(false);

                localGameManager.player1Txt.text = "You";
                localGameManager.player2Txt.text = "Opponent";
                //localGameManager.Timer1Txt.text = "YOUR\nTURN";
                //localGameManager.Timer2Txt.text = "OPPONENT\nTURN";

                Debug.Log("I'm 1st Player");
            }
            else
            {
                localGameManager.isPlayer1Turn = true;

                ButtonManager.Instance.redButton.gameObject.SetActive(false);

                localGameManager.player1Txt.text = "Opponent";
                localGameManager.player2Txt.text = "You";
                //localGameManager.Timer1Txt.text = "OPPONENT\nTURN";
                //localGameManager.Timer2Txt.text = "YOUR\nTURN";

                Debug.Log("I'm 2nd Player");
            }
        }
        else
        {
            localPlayerID = 2; // Client is player 2

            isLocalPlayer = false;
            playerID = Object.InputAuthority.PlayerId;
            PlayerName = $"Player_{playerID}";

            Debug.Log($"[NetworkPlayer] Spawned as remote player {playerID}");
        }

        Debug.Log($"[NetworkGridManager] Spawned as Player {localPlayerID}");

        // Initialize grid arrays
        if (Object.HasStateAuthority)
        {
            InitializePlayer();
        }
    }

    private void InitializePlayer()
    {
        SetReady(true);

        Debug.Log($"[NetworkPlayer] Player {playerID} initialized and ready");
    }

    public void SetReady(bool ready)
    {
        IsReady = ready;
        RPC_SetReady(ready);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetReady(bool ready)
    {
        IsReady = ready;
        Debug.Log($"[NetworkPlayer] Player {playerID} ready status: {ready}");  
        
        // Check if both players are ready
        if (IsReady)
        {
            CheckGameStart();
        }
    }
    
    private void CheckGameStart()
    {
        // Find all network players
        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
        bool allReady = true;

        //Debug.Log("CheckGameStart2");

        foreach (var player in players)
        {
            if (!player.IsReady)
            {
                allReady = false;
                break;
            }
        }

        if (players.Length >= 2)
        {
            // Both players are ready, start the game
            if (Object.HasStateAuthority)
            {
                RPC_StartGame();
            }
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartGame()
    {
        isGameStarted = true;
        Debug.Log("[NetworkPlayer] Game started!");
        
        localGameManager.InitializeMultiplayerGame();
    }

    // RPC methods for player actions
    //[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    //[Rpc(RpcSources.All, RpcTargets.All)]
    //[Rpc(RpcSources.All, RpcTargets.AllExceptInputAuthority)]
    [Rpc(sources: RpcSources.All, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_OnRedButtonClicked(Vector2 pos)
    {
        ButtonManager.Instance.OnRedButtonClicked_(pos);

        //Debug.Log("RPC_RedButtonClick");
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_OnBlueButtonClicked(Vector2 pos)
    {
        ButtonManager.Instance.OnBlueButtonClicked_(pos);

        //Debug.Log("RPC_BlueButtonClick");
    }

    [Rpc(sources: RpcSources.All, targets: RpcTargets.All, InvokeLocal = false)]
    public void RPC_AddSpaceshipScore(SpaceshipController.SpaceshipType spaceshipType, int scoreValue)
    {
        GameManager.Instance.AddSpaceshipScore(spaceshipType, scoreValue);
    }

    //[Rpc(sources: RpcSources.All, targets: RpcTargets.All, InvokeLocal = false)]
    //public void RPC_RedEnemyCreate(Vector3 pos)
    //{
    //    EnemySpawner.Instance.SpawnRedEnemy_(pos);

    //    //Debug.Log("RPC_RedEnemyCreate");
    //}

    //[Rpc(sources: RpcSources.All, targets: RpcTargets.All, InvokeLocal = false)]
    //public void RPC_BlueEnemyCreate(Vector3 pos)
    //{
    //    EnemySpawner.Instance.SpawnBlueEnemy_(pos);

    //    //Debug.Log("RPC_BlueEnemyCreate");
    //}

    private IEnumerator DisconnectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (FusionConnector.instance != null && 
            FusionConnector.instance.NetworkRunner != null)
        {
            FusionConnector.instance.NetworkRunner.Shutdown();
        }
    }

    // Public methods for external access
    public bool IsGameStarted => isGameStarted;
    public int Score => score;
    public bool IsPlayerReady => IsReady;
}
