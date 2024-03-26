using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToTutorial : MonoBehaviour
{
    // UI Objects
    public Slider progressBar;          // Progress bar slider
    public GameObject loadLevelCanvas;  // Load level screen canvas


    /// <summary>
    /// When player click this object, load tutorial
    /// </summary>
    public void OnPointerClick()
    {
        StartCoroutine(LoadAsynchronously(0));
    }

    /// <summary>
    /// Load level async. It also update progress bar.
    /// </summary>
    /// <param name="level">Level index to be loaded</param>
    /// <returns></returns>
    private IEnumerator LoadAsynchronously(int level)
    {
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync(level);  // Get the async operation from LoadSceneAsync

        // While the loading is not done
        while(!loadLevel.isDone)
        {
            float progress = Mathf.Clamp01(loadLevel.progress / 0.9f);  // Calculate the loading progress percent
            progressBar.value = progress;                               // Set slider value acording to progress

            yield return null;
        }
    }
}
