using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerNetwork : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(UpdateLobbyUI))] public NetworkString<_32> PlayerName { get; set; }

    [Networked, OnChangedRender(nameof(UpdateLobbyUI))]
    public bool IsReady { get; set; }


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
            IsReady = false; // default for new player

        if (Object.HasInputAuthority)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", $"Player{Random.Range(1000, 9999)}") ?? $"Player{Random.Range(1000, 9999)}";
            RPC_SetPlayerName(savedName);
        }
        if (SceneManager.GetActiveScene().buildIndex == 1) // in lobby
            LobbyUI.Instance.RefreshPlayerList();
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
            return;// in lobby
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
}
