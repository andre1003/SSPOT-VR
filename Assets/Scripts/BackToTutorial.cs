using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToTutorial : MonoBehaviour {
    // UI
    public Image reticle;               // Crosshair reticle
    public Slider progressBar;          // Progress bar slider
    public GameObject loadLevelCanvas;  // Load level screen canvas


    // Scale
    private Vector3 newScale;           // Target crosshair scale


    // Start is called before the first frame update
    private void Start() {
        newScale = reticle.transform.localScale;    // Set new scale to the reticle current scale when start the scene
    }

    // Update is called once per frame
    private void Update() {
        // If reticle current scale is different from newScale, smoothly change the crosshair the scale
        if(reticle.transform.localScale != newScale) {
            reticle.transform.localScale = Vector3.Lerp(reticle.transform.localScale, newScale, 5.0f * Time.deltaTime);
        }
    }

    /// <summary>
    /// When playes starts to gaze this object.
    /// </summary>
    public void OnPointerEnter() {
        newScale = new Vector3(2.5f, 2.75f, 2.5f);  // Set target scale
    }

    /// <summary>
    /// When player stops to gaze this object.
    /// </summary>
    public void OnPointerExit() {
        newScale = new Vector3(0.5f, 0.55f, 0.5f);  // Set target scale
    }

    /// <summary>
    /// When player click this object, load tutorial
    /// </summary>
    public void OnPointerClick() {
        StartCoroutine(LoadAsynchronously(0));
    }

    /// <summary>
    /// Load level async. It also update progress bar.
    /// </summary>
    /// <param name="level">Level index to be loaded</param>
    /// <returns></returns>
    private IEnumerator LoadAsynchronously(int level) {
        AsyncOperation loadLevel = SceneManager.LoadSceneAsync(level);  // Get the async operation from LoadSceneAsync

        // While the loading is not done
        while (!loadLevel.isDone) {
            float progress = Mathf.Clamp01(loadLevel.progress / 0.9f);  // Calculate the loading progress percent
            progressBar.value = progress;                               // Set slider value acording to progress

            yield return null;
        }
    }
}
