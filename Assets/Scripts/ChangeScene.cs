using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour {
    // UI Objects
    public GameObject loadSceneCanvas;  // Loading scene canvas GameObject
    public Slider progressBar;          // Progress bar slider


    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter() {

    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit() {

    }

    /// <summary>
    /// When player clicks on this object, the next level is loaded
    /// </summary>
    public void OnPointerClick() {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1); // Load next level
    }

    /// <summary>
    /// Starts coroutine for load a level async.
    /// </summary>
    /// <param name="level">Level index to be loaded</param>
    private void LoadLevel(int level) {
        loadSceneCanvas.SetActive(true);            // Activate loading scene canvas
        StartCoroutine(LoadAsynchronously(level));  // Start loading next level
    }

    /// <summary>
    /// Load level async. It also update progress bar.
    /// </summary>
    /// <param name="level">Level index to be loaded</param>
    /// <returns></returns>
    private IEnumerator LoadAsynchronously(int level) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);  // Get the async operation from LoadSceneAsync

        // While the loading is not done
        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);   // Calculate the loading progress percent
            progressBar.value = progress;                               // Set slider value acording to progress

            yield return null;
        }
    }
}
