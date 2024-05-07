using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackHome : MonoBehaviour
{
    [SerializeField] private Button homeBtn;

    private void OnEnable()
    {
        homeBtn.onClick.AddListener(DisconnectGame);
    }

    void DisconnectGame()
    {
        NetworkManager.Singleton.Shutdown();
        SceneController.Instance.BackToMainMenu();
    }

    private void OnDisable()
    {
        homeBtn.onClick.RemoveAllListeners();
    }
}
