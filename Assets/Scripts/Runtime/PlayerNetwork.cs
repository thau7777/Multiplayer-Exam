using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnPlayerNameChanged))]
    public NetworkString<_32> PlayerName { get; set; }

    [Networked, OnChangedRender(nameof(UpdateLobbyUI))]
    public bool IsReady { get; set; }

    // Called automatically when PlayerName changes
    public void OnPlayerNameChanged()
    {
        SetDisplayName();
    }

    void UpdateLobbyUI()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            return;

        LobbyUI.Instance.RefreshPlayerList();
        LobbyManager.Instance.UpdateLobbyReady();
    }

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasStateAuthority)
            IsReady = false;

        if (Object.HasInputAuthority)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", $"Player{Random.Range(1000, 9999)}");
            RPC_SetPlayerName(savedName);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
            LobbyUI.Instance.RefreshPlayerList();

        // Ensure display name is set on local spawn too
        SetDisplayName();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            return;

        LobbyUI.Instance.RefreshPlayerList();
        LobbyManager.Instance.UpdateLobbyReady();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetPlayerName(string name, RpcInfo info = default)
    {
        PlayerName = name;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetReady(bool ready, RpcInfo info = default)
    {
        IsReady = ready;
    }

    public void SetDisplayName()
    {
        if (SceneManager.GetActiveScene().buildIndex != 2) return; // only gameplay scene
        var display = GetComponentInChildren<PlayerNameDisplay>();
        if (display != null)
            display.SetName(PlayerName.ToString());
    }
}
