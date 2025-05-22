using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Press Esc to pause/unpause
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        //BroadcastMessage("RESUME");
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        //BroadcastMessage("PAUSE");
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Reset time before switching scenes
        SceneManager.LoadScene("MainMenu");
    }
    public void ExitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    
}