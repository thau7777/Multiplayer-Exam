using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine;

public enum InputButton
{
    Jump,
}

public struct NetworkInputData : INetworkInput
{
    public Vector2 moveInput;
    public NetworkButtons buttons;
    public float cameraYaw; // <-- add yaw
}

public class InputHandler : SimulationBehaviour, INetworkRunnerCallbacks, IBeforeUpdate
{
    private NetworkInputData _accumulatedInput;
    private bool _resetInput;

    private void Awake()
    {
        var runner = GetComponent<NetworkRunner>();
        if (runner != null)
        {
            runner.AddCallbacks(this);
        }
        else
        {
            Debug.LogError("No NetworkRunner found on this GameObject!");
        }
    }

    public void BeforeUpdate()
    {
        if (_resetInput)
        {
            _accumulatedInput = default;
            _resetInput = false;
        }

        // Movement
        _accumulatedInput.moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Buttons
        NetworkButtons buttons = default;
        buttons.Set(InputButton.Jump, Input.GetKeyDown(KeyCode.Space));
        _accumulatedInput.buttons = new NetworkButtons(_accumulatedInput.buttons.Bits | buttons.Bits);

        // Camera yaw (local player only)
        if (Camera.main != null)
        {
            _accumulatedInput.cameraYaw = Camera.main.transform.eulerAngles.y;
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(_accumulatedInput);
        _resetInput = true;
    }

    // ---------- Required stubs ----------
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
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
}
