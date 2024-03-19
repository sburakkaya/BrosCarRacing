using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkGameManager : NetworkBehaviour
{
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData
        {
            _int = 55,
            _bool = true
        },NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }
    
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (previousValue, newValue) =>
        {
            Debug.Log(OwnerClientId + "random number: " + newValue._int + newValue._bool + newValue.message);
        };
    }
    
    private void Update()
    {
        if(!IsOwner)return;
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestClientRpc(new ClientRpcParams{Send = new ClientRpcSendParams{TargetClientIds = new List<ulong>{1}}});
            /*randomNumber.Value = new MyCustomData
            {
                _int = 10,
                _bool = false,
                message = "helloo"
            };*/
        }
    }

    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log("testrpc : " + OwnerClientId + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("testclientrpc");
    }
}
