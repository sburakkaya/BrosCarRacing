using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CarRaceControl : NetworkBehaviour
{
    private int totalLaps; 
    public Transform[] checkpoints; 
    private int currentCheckpointIndex = 0; 
    private int currentLap = 1; 
    private bool raceFinished = false;
    
    public NetworkVariable<int> totalCollectedCheckpoints = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    
    private void Start()
    {
        checkpoints = RaceManager.Instance.checkpoints;
        totalLaps = RaceManager.Instance.totalLaps;
    }

    public override void OnNetworkSpawn()
    {
        totalCollectedCheckpoints.OnValueChanged += (int previousValue, int newValue) =>
        {
            Debug.Log(OwnerClientId + " Checkpoints : " + totalCollectedCheckpoints.Value);
        };
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            other.GetComponent<CheckPointOnOff>().CloseCP();
            CheckpointReached(other.transform);
        }
    }

    void CheckpointReached(Transform checkpoint)
    {
        if (!raceFinished)
        {
            if (checkpoint == checkpoints[currentCheckpointIndex])
            {
                currentCheckpointIndex++;

                if (IsOwner)
                {
                    totalCollectedCheckpoints.Value = totalCollectedCheckpoints.Value + 1;
                }

                float progress = (float)currentCheckpointIndex / (float)checkpoints.Length;

                Debug.Log("Yarışın %" + (progress * 100).ToString("F2") + " tamamlandı.");

                if (currentCheckpointIndex >= checkpoints.Length)
                {
                    currentCheckpointIndex = 0; 
                    currentLap++; 
                    
                    Debug.Log("Tur " + currentLap + " tamamlandı!");
                    
                    if (currentLap > totalLaps)
                    {
                        Debug.Log("Yarış bitti!");
                        raceFinished = true;
                        RaceManager.Instance.WhoFinishedRace(GetComponent<PlayerController>());
                    }
                }
            }
            else
            {
                Debug.LogWarning("Yanlış sıradaki checkpoint'e ulaşıldı!");
            }
        }
    }
}
