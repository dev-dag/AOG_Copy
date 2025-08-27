using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingleTon<GameManager>
{
    public GameSceneControl GaeSceneControl { get; private set; }

    private void Start()
    {
        
    }

    public async Awaitable LoadScene(SceneEnum scene)
    {
        await SceneManager.LoadSceneAsync((int)SceneEnum.Empty, LoadSceneMode.Single);

        await SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Single);

        OnSceneLoaded();
    }

    private void OnSceneLoaded()
    {
        GaeSceneControl = GameObject.FindGameObjectWithTag("SceneControl").GetComponent<GameSceneControl>(); // 씬 컨트롤 캐싱
        GaeSceneControl.Initialize(60f);
    }
}
