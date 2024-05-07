using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName : byte
{
    MainMenuAndLobby,
    RaceTrack
}
public class PlayerSelections : SingletonNetwork<PlayerSelections>
{
    public int selectedCarIndex;

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    
}
