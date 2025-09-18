using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
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

    public NetworkRunner NetworkRunner => networkRunner;

    public GameManager gameManager;

    private void Awake()
    {
        instance = this;
    }

    internal async void ConnectToServer(string sessionName)
    {
        if (networkRunner == null)
            gameObject.AddComponent<NetworkRunner>();

        networkRunner.ProvideInput = true;

        var result = await networkRunner.StartGame(
            new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
                PlayerCount = 2,
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

            // Spawn NetworkGridManager if it doesn't exist
            if (UnityEngine.Object.FindObjectOfType<NetworkPlayer>() == null)
            {
                Debug.Log("[FusionConnector] Spawning NetworkPlayer...");
                if (networkPlayerPrefab != null)
                {
                    runner.Spawn(networkPlayerPrefab);
                }
                else
                {
                    Debug.LogError("[FusionConnector] NetworkPlayer prefab not assigned!");
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
}
