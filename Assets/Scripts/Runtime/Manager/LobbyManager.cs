
using Fusion;
using UnityEngine;
using System.Linq;

public class LobbyManager : Singleton<LobbyManager>
{
    [Header("Gameplay scene build index")]
    public int gameplaySceneIndex = 2;

    private bool _started = false;
    private NetworkRunner _runner;

    protected override void Awake()
    {
        base.Awake();
        _runner = FindAnyObjectByType<NetworkRunner>();
    }

    void Update()
    {
        //if (_runner == null || !_runner.IsServer || _started) return;
        //var players = FindObjectsByType<PlayerNetwork>(FindObjectsSortMode.None);
        //if (players.Length == 0)
        //{
        //    Debug.Log("No players in room yet.");
        //    return;
        //}

        //bool allReady = players.All(p => p.IsReady);
        //if (allReady)
        //{
        //    _started = true;
        //    Debug.Log("All players ready — host loading gameplay scene.");
        //    _runner.LoadScene(SceneRef.FromIndex(gameplaySceneIndex),
        //        UnityEngine.SceneManagement.LoadSceneMode.Single);
        //}
    }

    public void UpdateLobbyReady()
    {
        if (_runner == null || !_runner.IsServer || _started) return;
        var players = FindObjectsByType<PlayerNetwork>(FindObjectsSortMode.None);
        if (players.Length == 0)
        {
            Debug.Log("No players in room yet.");
            return;
        }

        bool allReady = players.All(p => p.IsReady);
        if (allReady)
        {
            _started = true;
            Debug.Log("All players ready — host loading gameplay scene.");
            _runner.LoadScene(SceneRef.FromIndex(gameplaySceneIndex),
                UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

}
