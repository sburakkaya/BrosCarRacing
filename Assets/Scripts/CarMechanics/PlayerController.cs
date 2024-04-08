using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public Transform lookAtTarget, positionTarget;
    public TextMeshPro playerNameTMP;

    public NetworkVariable<PlayerStruct> playerName = new NetworkVariable<PlayerStruct>(new PlayerStruct(),NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public struct PlayerStruct : INetworkSerializable
    {
        public FixedString128Bytes _playerName;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _playerName);
        }
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerName.Value = new PlayerStruct{_playerName = LobbyManager.Instance.playerName};
            playerNameTMP.text = playerName.Value._playerName.Value;
            Camera.main.GetComponent<DriftCamera>().lookAtTarget = lookAtTarget;
            Camera.main.GetComponent<DriftCamera>().positionTarget = positionTarget;
        }
        RaceManager.Instance.AddPlayersToList(this);
    }

    public override void OnNetworkDespawn()
    {
        RaceManager.Instance.RemovePlayersToList(this);
    }
}
