using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    public GameManager gameManager;

    //Loading level
    public GameObject loadingPannel;
    public Slider slider;

    //Music Pannel
    public GameObject AudioControls;

    public GameObject GameUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;

        gameManager.pauseActive = true;
    }

    private void ResumeGame()
    {
        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;

        gameManager.pauseActive = false;
    }

    public void RestartLevel()
    {
        GameUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadSceneAsyn(currentScene));
        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        StartCoroutine(LoadSceneAsyn(0));
        Time.timeScale = 1f;
    }

    public void MusicControls()
    {
        AudioControls.SetActive(true);
        pauseMenuUI.SetActive(false);

    }

    public void BackToPause()
    {
        AudioControls.SetActive(false);
        pauseMenuUI.SetActive(true);

    }

    IEnumerator LoadSceneAsyn(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingPannel.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 9f);

            slider.value = progress;

            yield return null;
        }
    }
}


