using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PauseManager2 : MonoBehaviour
{
    public bool ispaused = false;
    public GameObject pauseMenu;
    public void PresionaEscape()
    {
        if (ispaused)
            Resume();
        else
            Pause();
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        ispaused = true;

        pauseMenu.SetActive(true);
    }
    public void Resume()
    {
        Time.timeScale = 1f;
        ispaused = false;

        pauseMenu.SetActive(false);
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("MainMenu");
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}