// LobbyUI.cs
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : Singleton<LobbyUI>, INetworkRunnerCallbacks
{
    public Transform playerListParent;
    public GameObject playerRowPrefab;
    public TMP_Text roomNameText;
    public Button readyButton;
    public Button quitButton;
    public TMP_Text readyButtonText;

    private NetworkRunner _runner;
    private bool _isReady = false;

    protected override void Awake()
    {
        base.Awake();
        if (readyButton != null) readyButton.onClick.AddListener(ToggleReady);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuit);

        _runner = FindAnyObjectByType<NetworkRunner>();
        _runner.AddCallbacks(this); // Subscribe to Fusion callbacks
        roomNameText.text = _runner.SessionInfo.Name;
        
    }
    private void Start()
    {
        SetCursorVisible(true);
    }
    private void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        RefreshPlayerList();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        RefreshPlayerList();
    }


    public void RefreshPlayerList()
    {
        Debug.Log("Refreshing player list...");
        if (playerListParent == null || playerRowPrefab == null) return;

        // clear old rows
        for (int i = playerListParent.childCount - 1; i >= 0; i--)
            Destroy(playerListParent.GetChild(i).gameObject);

        // find networked players in scene
        var players = FindObjectsByType<PlayerNetwork>(FindObjectsSortMode.None);
        if (players == null || players.Length == 0) return;

        // sort players by their InputAuthority (PlayerRef.RawEncoded is numeric)
        //var sortedPlayers = players
        //    .OrderBy(p => p.Object.InputAuthority.RawEncoded)
        //    .ToList();

        foreach (var p in players)
        {
            if (!p.Object || !p.Object.IsValid) continue;

            var go = Instantiate(playerRowPrefab, playerListParent);
            var row = go.GetComponent<PlayerRow>();

            string displayName = p.PlayerName.ToString();
            if (string.IsNullOrEmpty(displayName))
                displayName = $"Player {p.Object.InputAuthority.PlayerId}";

            row.Setup(displayName, p.IsReady);
        }
    }


    void ToggleReady()
    {
        var local = FindLocalPlayerNetwork();
        if (local == null) return;

        _isReady = !_isReady;
        readyButtonText.text = _isReady ? "Unready" : "Ready";

        // Only tell the host; replication will sync IsReady to everyone
        local.RPC_SetReady(_isReady);
    }


    PlayerNetwork FindLocalPlayerNetwork()
    {
        var players = FindObjectsByType<PlayerNetwork>(FindObjectsSortMode.None);
        foreach (var p in players)
        {
            if (p.Object.HasInputAuthority)
                return p;
        }
        return null;
    }

    async void OnQuit()
    {
        if (_runner != null)
        {
            await _runner.Shutdown(true); // ✅ graceful shutdown, sends leave message
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    void OnDestroy()
    {
        if (readyButton != null) readyButton.onClick.RemoveListener(ToggleReady);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuit);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}
