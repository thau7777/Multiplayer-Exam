using Fusion;
using Unity.Cinemachine;
using UnityEngine;
// Make sure your prefab is registered in the NetworkRunner prefab list and assigned as a NetworkPrefabRef.

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;
    [Header("Spawn")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float randomRadius = 5f;

    // Called by Fusion when any player joins (on every client). 
    // We only spawn the local player's avatar on the local client.
    public void PlayerJoined(PlayerRef player)
    {
        // spawn only for the local client
        if (player != Runner.LocalPlayer)
            return;

        Vector3 spawnPos = Vector3.zero;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            var t = spawnPoints[Random.Range(0, spawnPoints.Length)];
            spawnPos = t.position;
        }
        else
        {
            spawnPos = new Vector3(
                Random.Range(-randomRadius, randomRadius),
                1f,
                Random.Range(-randomRadius, randomRadius)
            );
        }

        // Spawn the player object and assign ownership to this player
        var spawned = Runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);

        // setup Cinemachine for local player only
        if (virtualCamera == null)
            virtualCamera = FindFirstObjectByType<CinemachineCamera>();

        if (virtualCamera != null && spawned != null)
        {
            virtualCamera.Follow = spawned.transform;
            virtualCamera.LookAt = spawned.transform;
        }
    }

    // Called when any player leaves. Despawn their object if present.
    public void PlayerLeft(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var obj) && obj != null)
        {
            // It's fine to call Despawn from any peer here; Fusion will remove the object across the session.
            Runner.Despawn(obj);
        }
    }
}
