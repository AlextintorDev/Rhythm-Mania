using UnityEngine;

//Singleton al inicio del juego persistente
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public override bool Persistant => true;

    public override void Awake()
    {
        base.Awake();
        if (Instance != this) return;
        AnalyticsRecorder.Initialize();
    }

    public void GoToNextLevel()
    {
        GameSettings.Instance.SongIndex++;
        GoToScene("Gameplay");
    }

    public void GoToScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    [ContextMenu("Test Analytics")]
    public void TestAnalytics()
    {
        Debug.Log("Session JSON: " + AnalyticsRecorder.ToJson());
    }
}