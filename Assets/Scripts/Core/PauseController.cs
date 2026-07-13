using UnityEngine;

public class PauseController : SingletonMonoBehaviour<PauseController>
{
    public bool isPaused = false;
    public GameObject pauseMenu;

    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         TogglePause();
    //     }
    // }

    // public void TogglePause()
    // {
    //     isPaused = !isPaused;
    //     Time.timeScale = isPaused ? 0f : 1f;
    //     pauseMenu.SetActive(isPaused);
    // }

    // public void ExitGame()
    // {
    //     Time.timeScale = 1f; // Asegura que el tiempo se reanude antes de salir
    //     GoToScene.LoadScene("MainMenu");
    // }

}
