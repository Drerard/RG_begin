using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseGameMenu;

    private bool pauseGame = false;


    private void Awake()
    {
        KeyInputEventManager.escapeEvent.AddListener(Pause);
    }
   

    private void PauseSwitch()
    {
        if (pauseGame)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    public void Pause()
    {
        pauseGame = true;
        Time.timeScale = 0;
        pauseGameMenu.SetActive(true);
    }
    public void Resume()
    {
        pauseGame = false;
        Time.timeScale = 1;
        pauseGameMenu.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
