using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public GameObject loadingPannel;

    public Slider slider;



    public void LoadLevel()
    {
        int NextScene = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadSceneAsyn(NextScene));
    }

    public void RestartLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadSceneAsyn(currentScene));
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
