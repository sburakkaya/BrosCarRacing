using Unity.Netcode;
using UnityEngine;

public class GameplayManager : SingletonNetwork<GameplayManager>
{
    [SerializeField] private GameObject car;
    
    [ServerRpc(RequireOwnership = false)]
    public void PlayerCarSpawnerServerRpc(ulong clientid)
    {
        GameObject playerCar =
            NetworkObjectSpawner.SpawnNewNetworkObjectAsPlayerObject(
                car,
                transform.position,
                clientid);
    }
}
