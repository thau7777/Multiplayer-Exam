using Fusion;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class RunnerManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [Header("Scenes")]
    public int lobbySceneIndex = 1;
    public int gameplaySceneIndex = 2;

    [Header("Gameplay")]
    public NetworkPrefabRef playerPrefab;
    public Transform[] spawnPoints;

    private NetworkRunner _runner;
    private NetworkSceneManagerDefault _sceneManager;
    private bool _isInLobby = false;

    async void Start()
    {
        MainMenuUI.Instance.ModifyMainMenuScreen(true);

        _runner = GetComponent<NetworkRunner>() ?? gameObject.AddComponent<NetworkRunner>();
        _sceneManager = GetComponent<NetworkSceneManagerDefault>() ?? gameObject.AddComponent<NetworkSceneManagerDefault>();

        _runner.AddCallbacks(this);

        var joinResult = await _runner.JoinSessionLobby(SessionLobby.ClientServer);
        if (!joinResult.Ok)
        {
            Debug.LogWarning($"JoinSessionLobby failed: {joinResult.ShutdownReason}");
        }
        else
        {
            _isInLobby = true;
            Debug.Log("Successfully joined lobby.");
            MainMenuUI.Instance.ModifyMainMenuScreen(false);
        }

    }


    // ----------------- Create (Host) -----------------
    public async void CreateRoom(string roomName)
    {
        if (!_isInLobby) return;

        if (_runner.IsRunning)
        {
            Debug.LogWarning("Runner already running");
            return;
        }

        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = roomName,
            Scene = SceneRef.FromIndex(lobbySceneIndex),
            SceneManager = _sceneManager,
            PlayerCount = 4,
            IsOpen = true,
            IsVisible = true
        };

        var result = await _runner.StartGame(startArgs);
        if (!result.Ok)
        {
            Debug.LogError($"Host StartGame failed: {result.ShutdownReason}");
            return;
        }

        Debug.Log($"Created and joined host room: {roomName}");
    }

    // ----------------- Join -----------------
    public async void JoinRoomByName(string sessionName)
    {
        if (!_isInLobby) return;

        if (_runner.IsRunning && _runner.IsInSession) return;

        var startArgs = new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sessionName,
            SceneManager = _sceneManager
        };

        var result = await _runner.StartGame(startArgs);
        if (!result.Ok)
        {
            Debug.LogError($"Join failed: {result.ShutdownReason}");
            return;
        }

        Debug.Log($"Joined room: {sessionName}");
    }
    public async void RefreshSessionList(UnityAction action)
    {
        if (_runner == null) return;
        UnityAction safeAction = action ?? (() => { });
        Debug.Log("Refreshing session list...");

        var result = await _runner.JoinSessionLobby(SessionLobby.ClientServer);
        if (!result.Ok)
        {
            Debug.LogWarning($"Refresh failed: {result.ShutdownReason}");
        }
        else
        {
            Debug.Log("Refresh successful.");
        }
        safeAction?.Invoke();
    }

    // ----------------- INetworkRunnerCallbacks -----------------
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"OnSessionListUpdated: {sessionList.Count} sessions found");
        MainMenuUI.Instance.UpdateSessionList(sessionList);
    }

    // ✅ Spawn players again after gameplay scene is loaded
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        if (runner.IsServer && SceneManager.GetActiveScene().buildIndex == 2)
        {
            foreach (var player in runner.ActivePlayers)
            {
                SpawnPlayer(player);
            }

            var doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
            if (doors.Length > 0)
            {
                var randomDoor = doors[UnityEngine.Random.Range(0, doors.Length)].GetComponent<NetworkObject>();

                GameManager.Instance.Rpc_ChooseExitDoor(randomDoor);
            }
        }
    }
    

    // in lobby
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            SpawnPlayer(player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer && runner.TryGetPlayerObject(player, out var obj) && obj != null)
        {
            runner.Despawn(obj);
        }
    }

    // ----------------- Player Spawning -----------------
    public void SpawnPlayer(PlayerRef player)
    {
        var oldObject = _runner.GetPlayerObject(player);
        if (oldObject != null)
        {
            _runner.Despawn(oldObject);
        }
        Vector3 pos = Vector3.zero;
        if (spawnPoints != null && spawnPoints.Length > 0)
            pos = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;
        else
            pos = new Vector3(UnityEngine.Random.Range(-3f, 3f), 1f, UnityEngine.Random.Range(-3f, 3f));

        var obj = _runner.Spawn(playerPrefab, pos, Quaternion.identity, player);
        _runner.SetPlayerObject(player, obj); // ✅ critical step
    }
    // ----------------- Other callbacks (minimal stubs) -----------------
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, ref NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) => SceneManager.LoadScene(0);
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) => SceneManager.LoadScene(0);
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }
}
