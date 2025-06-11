using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip onClickClip;
    public void ClickAndLoad(String functionName)
    {
        audioSource.PlayOneShot(onClickClip);
        Invoke(functionName, 0.3f);
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("TutorialLevel");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}