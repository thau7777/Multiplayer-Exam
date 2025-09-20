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
}
public class InputHandler : SimulationBehaviour, INetworkRunnerCallbacks, IBeforeUpdate
{
    // Accumulated input each frame
    private Vector2 _moveInput;
    private NetworkButtons _buttons;
    private void Awake()
    {
        // The InputHandler is on the same GameObject as the NetworkRunner
        var runner = GetComponent<NetworkRunner>();

        if (runner != null)
        {
            // Register as callback target
            runner.AddCallbacks(this);
        }
        else
        {
            Debug.LogError("No NetworkRunner found on this GameObject!");
        }
    }


    // Called every frame *before* Fusion’s simulation tick

    public void BeforeUpdate()
    {
        Debug.Log("BeforeUpdate called");
        // Movement input
        _moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Reset buttons each frame
        _buttons = default;

        // Accumulate single-frame button presses
        _buttons.Set(InputButton.Jump, Input.GetKeyDown(KeyCode.Space));
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log("OnInput called");
        // Package into NetworkInputData
        NetworkInputData data = new NetworkInputData
        {
            moveInput = _moveInput,
            buttons = _buttons
        };

        input.Set(data);

        // Clear one-frame buttons after sending
        _buttons = default;
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
