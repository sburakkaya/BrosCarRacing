using Unity.Netcode;
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
        SceneManager.LoadScene("LobbyTutorial_Done");
    }

    private void OnDisable()
    {
        homeBtn.onClick.RemoveAllListeners();
    }
}
