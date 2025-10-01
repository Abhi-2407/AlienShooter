using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening.Core.Easing;

public class FusionConnector : MonoBehaviour, INetworkRunnerCallbacks
{
    public static FusionConnector instance;

    [SerializeField]
    private NetworkRunner networkRunner;

    [Header("Network Prefabs")]
    [SerializeField]
    private NetworkObject networkPlayerPrefab;

    [Header("Local Player Objects")]
    [SerializeField]
    private NetworkObject blueObjectPrefab; // Blue object for local player

    [Header("Remote Player Objects")]
    [SerializeField]
    private NetworkObject redObjectPrefab; // Red object for remote player

    public NetworkRunner NetworkRunner => networkRunner;

    public GameManager gameManager;

    private void Awake()
    {
        instance = this;
    }

    internal async void ConnectToServer(string sessionName, string region = "in")
    {
        if (networkRunner == null)
            gameObject.AddComponent<NetworkRunner>();

        networkRunner.ProvideInput = true;

        // Configure Photon app settings with region
        var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
        appSettings.UseNameServer = true;
        appSettings.AppVersion = "1.0.0";
        appSettings.FixedRegion = region.ToLower();
        Debug.Log($"[FusionConnector] Connecting to region: {region}");

        var result = await networkRunner.StartGame(
            new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
                PlayerCount = 2,
                CustomPhotonAppSettings = appSettings
            }
        );

        if (result.Ok)
        {
            // Register callbacks
            networkRunner.AddCallbacks(this);

            //Initialize multiplayer match setup

            // Spawn players (your original working implementation)
            //GameManager.Instance.SpawnPlayer(networkRunner);

            Debug.Log("PhotonFusion: Game started successfully.");
        }
        else
        {
            Debug.LogError($"PhotonFusion: Failed to start game. Reason: {result.ShutdownReason}");

            // Report connection failure to platform
            if (IFrameBridge.Instance != null)
            {
                IFrameBridge.Instance.PostMatchAbort(
                    "Failed to connect to multiplayer server",
                    result.ShutdownReason.ToString(),
                    "CONNECTION_FAILED"
                );
            }
        }
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log($"Photon Callback - Connected to server");
    }

    public void OnConnectFailed(
        NetworkRunner runner,
        NetAddress remoteAddress,
        NetConnectFailedReason reason
    )
    {
        Debug.Log($"Photon Callback - Connect failed: {reason}");
    }

    public void OnConnectRequest(
        NetworkRunner runner,
        NetworkRunnerCallbackArgs.ConnectRequest request,
        byte[] token
    )
    {
        Debug.Log($"Photon Callback - Connect request: {request}");
    }

    public void OnCustomAuthenticationResponse(
        NetworkRunner runner,
        Dictionary<string, object> data
    )
    {
        Debug.Log($"Photon Callback - Custom authentication response: {data}");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"Photon Callback - Disconnected from server: {reason}");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log($"Photon Callback - Host migration: {hostMigrationToken}");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Debug.Log($"Photon Callback - Input received: {input}");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log($"Photon Callback - Input missing for player {player}: {input}");
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // Debug.Log($"Photon Callback - Object entered AOI: {obj} for player {player}");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // Debug.Log($"Photon Callback - Object exited AOI: {obj} for player {player}");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"[FusionConnector] Player {player} joined the game");

        // Check if we have 2 players and start the game
        if (runner.ActivePlayers.Count() == 2)
        {
            Debug.Log("[FusionConnector] 2 players joined - starting multiplayer game!");


            if (runner.IsSharedModeMasterClient)
            {
                Debug.Log("I am the Master/Host in Shared Mode");
                GameObject go1 = runner.Spawn(blueObjectPrefab).gameObject;
                go1.transform.position = gameManager.spawnPoints[0].position;
            }
            else
            {
                if (player == runner.LocalPlayer)
                {
                    Debug.Log("I am just a Client");
                    GameObject go2 = runner.Spawn(redObjectPrefab).gameObject;
                    go2.transform.position = gameManager.spawnPoints[1].position;
                }
            }
            gameManager.InitializeMultiplayerGame();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (gameManager != null && gameManager.gameState != GameState.OVER)
        {
            var remainingPlayers = runner.ActivePlayers.ToList();

            if (remainingPlayers.Count() == 1)
            {
                // Determine if this is the opponent (not the local player)
                bool isOpponent = player != runner.LocalPlayer;

                if (isOpponent)
                {
                    Debug.Log($"[FusionConnector] Opponent {player} left the game - triggering forfeit");

                    // Stop the game
                    gameManager.gameState = GameState.OVER;

                    // Use the same simple approach as MotorKick with delay
                    IFrameBridge.Instance.PostOpponentForfeit();
                }
                else
                {
                    Debug.Log($"[FusionConnector] Local player {player} left the game");

                    // Send player forfeit message to platform
                    HandleLocalPlayerLeftWithDelay();
                }
            }
        }
    }

    // Coroutine to handle local player left with delay
    private void HandleLocalPlayerLeftWithDelay()
    {
        Debug.Log("[FusionConnector] Sending local player left handshake...");

        IFrameBridge.Instance.PostMatchAbort("You left the game.", "", "");
    }

    public void OnReliableDataProgress(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        float progress
    )
    {
        Debug.Log($"Photon Callback - Reliable data progress: {progress} for player {player}");
    }

    public void OnReliableDataReceived(
        NetworkRunner runner,
        PlayerRef player,
        ReliableKey key,
        ArraySegment<byte> data
    )
    {
        Debug.Log($"Photon Callback - Reliable data received for player {player}: {data}");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log($"Photon Callback - Scene load done");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log($"Photon Callback - Scene load start");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Photon Callback - Session list updated");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Photon Callback - Shutdown: {shutdownReason}");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log($"Photon Callback - User simulation message: {message}");
    }

    // Method to handle connection failures
    public void OnConnectionFailed(string reason)
    {
        Debug.LogError($"[FusionConnector] Connection failed: {reason}");

        if (IFrameBridge.Instance != null)
        {
            IFrameBridge.Instance.PostMatchAbort(
                "Failed to connect to multiplayer server",
                reason,
                "CONNECTION_FAILED"
            );
        }
    }

    /// <summary>
    /// Spawns player-specific objects based on local/remote player status
    /// </summary>
    private void SpawnPlayerObjects(NetworkRunner runner)
    {
        Debug.Log("[FusionConnector] Spawning player-specific objects...");

        foreach (PlayerRef player in runner.ActivePlayers)
        {
            if (player == runner.LocalPlayer)
            {
                // Local player - spawn blue objects
                SpawnBlueObjects(player);
            }
            else
            {
                // Remote player - spawn red objects
                SpawnRedObjects(player);
            }
        }
    }

    /// <summary>
    /// Spawns blue objects for the local player
    /// </summary>
    private void SpawnBlueObjects(PlayerRef player)
    {
        Debug.Log($"[FusionConnector] Spawning blue objects for local player {player}");

        //if (blueObjectPrefab != null)
        //{
        //    // Find a suitable spawn position for blue objects
        //    Vector3 spawnPosition = GetSpawnPosition(true); // true for blue (left side)

        //    GameObject blueObject = Instantiate(blueObjectPrefab, spawnPosition, Quaternion.identity);

        //    // Configure blue object properties
        //    ConfigureBlueObject(blueObject, player);

        //    Debug.Log($"[FusionConnector] Blue object spawned at position: {spawnPosition}");
        //}
        //else
        //{
        //    Debug.LogError("[FusionConnector] Blue object prefab not assigned!");
        //}
    }

    /// <summary>
    /// Spawns red objects for remote players
    /// </summary>
    private void SpawnRedObjects(PlayerRef player)
    {
        Debug.Log($"[FusionConnector] Spawning red objects for remote player {player}");

        //if (redObjectPrefab != null)
        //{
        //    // Find a suitable spawn position for red objects
        //    Vector3 spawnPosition = GetSpawnPosition(false); // false for red (right side)

        //    GameObject redObject = Instantiate(redObjectPrefab, spawnPosition, Quaternion.identity);

        //    // Configure red object properties
        //    ConfigureRedObject(redObject, player);

        //    Debug.Log($"[FusionConnector] Red object spawned at position: {spawnPosition}");
        //}
        //else
        //{
        //    Debug.LogError("[FusionConnector] Red object prefab not assigned!");
        //}
    }

    /// <summary>
    /// Gets spawn position based on object type
    /// </summary>
    private Vector3 GetSpawnPosition(bool isBlue)
    {
        // Get camera bounds for spawning
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }

        if (mainCamera != null)
        {
            Vector2 screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

            // Blue objects spawn on the left side, red objects on the right side
            float xPosition = isBlue ? -screenBounds.x + 1f : screenBounds.x - 1f;
            float yPosition = UnityEngine.Random.Range(-screenBounds.y + 1f, screenBounds.y - 1f);

            return new Vector3(xPosition, yPosition, 0f);
        }

        // Fallback position
        return isBlue ? new Vector3(-8f, 0f, 0f) : new Vector3(8f, 0f, 0f);
    }

    /// <summary>
    /// Configures blue object with player-specific properties
    /// </summary>
    private void ConfigureBlueObject(GameObject blueObject, PlayerRef player)
    {
        // Set tag for identification
        blueObject.tag = "BluePlayer";

        // Add any blue-specific components or properties
        if (blueObject.GetComponent<BlueEnemy>() != null)
        {
            // If it's a BlueEnemy, configure it for the local player
            BlueEnemy blueEnemy = blueObject.GetComponent<BlueEnemy>();
            // Add any specific configuration here
        }

        // You can add more configuration logic here
        Debug.Log($"[FusionConnector] Blue object configured for player {player}");
    }

    /// <summary>
    /// Configures red object with player-specific properties
    /// </summary>
    private void ConfigureRedObject(GameObject redObject, PlayerRef player)
    {
        // Set tag for identification
        redObject.tag = "RedPlayer";

        // Add any red-specific components or properties
        if (redObject.GetComponent<RedEnemy>() != null)
        {
            // If it's a RedEnemy, configure it for the remote player
            RedEnemy redEnemy = redObject.GetComponent<RedEnemy>();
            // Add any specific configuration here
        }

        // You can add more configuration logic here
        Debug.Log($"[FusionConnector] Red object configured for player {player}");
    }

    /// <summary>
    /// Public method to manually spawn objects for a specific player
    /// </summary>
    public void SpawnObjectsForPlayer(PlayerRef player, bool isLocalPlayer)
    {
        if (isLocalPlayer)
        {
            SpawnBlueObjects(player);
        }
        else
        {
            SpawnRedObjects(player);
        }
    }
}
