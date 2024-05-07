using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RaceManager : NetworkBehaviour
{
    public Transform[] checkpoints; 
    public Transform[] startPoints; 
    public int totalLaps;
    public List<PlayerController> players = new List<PlayerController>();
    public List<TextMeshProUGUI> scoreTextList = new List<TextMeshProUGUI>();
    public List<PlayerController> finishedPlayers = new List<PlayerController>();

    [SerializeField] private TextMeshProUGUI startCountdownTMP;
    [SerializeField] private Transform scorePanel;
    [SerializeField] private GameObject scoreTextPrefab;

    [SerializeField] private GameObject finishPanel;
    [SerializeField] private Transform finishedPlayersListTransform;
    
    public float countdownDuration = 5f;
    private bool isCounting = false;

    public static RaceManager Instance { get; private set; }
    
    public event Action<bool> OnRaceStartBool;
	    
    private void Awake() 
    {
        Instance = this;
    }
    
    public void RaceStart()
    {
        OnRaceStartBool?.Invoke(true);
    }

    public void RaceEnd()
    {
        OnRaceStartBool?.Invoke(false);
    }

    public void WhoFinishedRace(PlayerController playerController)
    {
        finishPanel.SetActive(true);
        finishedPlayers.Add(playerController);
        GameObject finishedPlayer = Instantiate(scoreTextPrefab, finishedPlayersListTransform);
        finishedPlayer.GetComponent<TextMeshProUGUI>().text = playerController.playerName.Value._playerName.Value;
    }

    public void AddPlayersToList(PlayerController playerController)
    {
        players.Add(playerController);
        GameObject scoreText = Instantiate(scoreTextPrefab,scorePanel);
        scoreTextList.Add(scoreText.GetComponent<TextMeshProUGUI>());
        playerController.playerNameTMP.text = playerController.playerName.Value._playerName.Value;
        players[players.Count-1].transform.position = startPoints[players.Count-1].transform.position;
        StartCountdown();
    }

    public void RemovePlayersToList(PlayerController playerController)
    {
        players.Remove(playerController);
    }

    void SetTransformsCars()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.position = startPoints[i].transform.position;
        }
    }

    private void LateUpdate()
    {
        List<PlayerController> playersSorted = players.OrderByDescending(p => p.GetComponent<CarRaceControl>().totalCollectedCheckpoints.Value).ToList();
        for (int i = 0; i < playersSorted.Count; i++)
        {
            scoreTextList[i].text = i+1 + " : " + playersSorted[i].playerName.Value._playerName.Value;
            playersSorted[i].playerNameTMP.text = playersSorted[i].playerName.Value._playerName.Value;
        }
    }

    private void Update()
    {
        if(!IsOwner)
            return;

        if (IsServer)
        {
            if (isCounting && players.Count > 1)
            {
                countdownDuration -= Time.deltaTime;
               
                StarterCountdownClientRPC(countdownDuration);
                
                if (0 >= countdownDuration)
                {
                    StopCountdownClientRPC();
                }
            }
        }
    }

    [ClientRpc]
    void StarterCountdownClientRPC(float time)
    {
        startCountdownTMP.text = time.ToString("F2");
    }
    
    void StartCountdown()
    {
        countdownDuration = 5;
        isCounting = true;
    }
    
    [ClientRpc]
    void StopCountdownClientRPC()
    {
        isCounting = false;
        startCountdownTMP.text = "";
        RaceStart();
    }
}
