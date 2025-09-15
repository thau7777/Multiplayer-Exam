using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;

public class InputHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;

    // These are for “one-frame” inputs like jump; track state so we don't miss the press
    private bool _jumpPressed;

    void Awake()
    {
        // maybe ensure there's a NetworkRunner in scene
        _runner = FindFirstObjectByType<NetworkRunner>();
        if (_runner == null)
        {
            Debug.LogError("No NetworkRunner found in scene for InputHandler");
        }
        else
        {
            _runner.AddCallbacks(this);
        }
    }

    void Update()
    {
        // capture jump down in Unity's Update, so we don't miss the frame
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpPressed = true;
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!runner.IsRunning) return;

        NetworkInputData data = new NetworkInputData
        {
            moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ),
            jumpPressed = _jumpPressed
        };

        input.Set(data);

        // reset the one-frame input
        _jumpPressed = false;
    }

    // Required empty implementations
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, ref NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
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
