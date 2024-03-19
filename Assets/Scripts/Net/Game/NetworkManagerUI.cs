using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button serverBtn, hostBtn, clientBtn;

    private void Awake()
    {
        serverBtn.onClick.AddListener((() => NetworkManager.Singleton.StartServer()));
        clientBtn.onClick.AddListener((() => NetworkManager.Singleton.StartClient()));
        hostBtn.onClick.AddListener((() => NetworkManager.Singleton.StartHost()));
    }
}
