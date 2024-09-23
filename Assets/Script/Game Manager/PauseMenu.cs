using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    public GameManager gameManager;

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
}


