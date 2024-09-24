using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public float totalTime = 30f;

    public TextMeshProUGUI timerText;

    private bool isTimerRunning = true;

    [SerializeField] private GameObject gameOver;

    [SerializeField] private GameObject gameUI;

    [SerializeField] private GameManager gameManager;

    private void Update()
    {
        if (isTimerRunning)
        {
            totalTime -= Time.deltaTime;

            if (totalTime <= 0)
            {
                totalTime = 0;
                isTimerRunning = false;

                TimerEnded();
            }

            UpdateTimerDisplay();
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(totalTime / 60);
        int seconds = Mathf.FloorToInt(totalTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void TimerEnded()
    {
        gameOver.SetActive(true);
        gameUI.SetActive(false);
        Time.timeScale = 0f;
        gameManager.gameOver = true;
    }
}
