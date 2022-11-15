using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public static bool isPaused = false;
    public GameObject pauseMenuUI;
    // public GameObject pauseButton;

    public void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPressed();
        }
    }
    public void IsPressed()
    {
        if (isPaused)
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
        Time.timeScale = 0;
        isPaused = true;
        pauseMenuUI.SetActive(true);
        // pauseButton.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        // pauseButton.SetActive(true);
    }

    public void Quit()
    {
        // SceneManager.LoadScene("Main Menu Scene");
        Debug.Log("Quit Game");
    }
}
