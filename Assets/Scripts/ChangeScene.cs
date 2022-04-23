using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour {
    // UI Objects
    public Image reticle;               // Crosshair reticle image
    public GameObject loadSceneCanvas;  // Loading scene canvas GameObject
    public Slider progressBar;          // Progress bar slider


    // Scale
    private Vector3 newScale;           // Crosshair target scale


    // Called before everything
    private void Awake() {
        newScale = transform.localScale; // Set initial target scale to current image local scale
    }

    // Update is called once per frame.
    private void Update() {
        // If the current crosshair local scale is different from target scale, then smoothly scale the image to newScale
        if (reticle.rectTransform.localScale != newScale) {
            reticle.rectTransform.localScale = Vector3.Lerp(reticle.rectTransform.localScale, newScale, 3f * Time.deltaTime);
        }
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter() {
        newScale = new Vector3(2.5f, 2.75f, 2.5f);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit() {
        newScale = new Vector3(0.5f, 0.55f, 0.5f);
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
