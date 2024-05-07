using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneController : SingletonNetwork<SceneController>
{
    public void OnEnable()
    {
        DontDestroyOnLoad(this);
    }

    public void GameStarter()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(LobbyManager.KEY_GAME_MAP,LoadSceneMode.Single);
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SpawnCars;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnCars;
        }
    }

    private void SpawnCars(ulong clientid, string scenename, LoadSceneMode loadscenemode)
    {
        if (IsServer)
        {
            GameplayManager.Instance.PlayerCarSpawnerServerRpc(clientid);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
