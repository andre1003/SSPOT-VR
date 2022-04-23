using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour {
    public Image reticle;
    public GameObject loadSceneCanvas;
    public Slider progressBar;

    private Vector3 newScale;

    private void Awake() { 
        newScale = transform.localScale;
    }

    private void Update() {
        if (reticle.rectTransform.localScale != newScale) {
            reticle.rectTransform.localScale = Vector3.Lerp(reticle.rectTransform.localScale, newScale, 3f * Time.deltaTime);
        }
    }

    public void OnPointerEnter() {
        newScale = new Vector3(2.5f, 2.75f, 2.5f);
    }

    public void OnPointerExit() {
        newScale = new Vector3(0.5f, 0.55f, 0.5f);
    }

    // When player clicks on this object, the FirstLevel is loaded
    // Hint: It's better use an async load, for older devices. It is a good pratice anyway
    public void OnPointerClick() {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Starts coroutine for load a level async.
    /// </summary>
    /// <param name="level">Level index to be loaded</param>
    private void LoadLevel(int level) {
        loadSceneCanvas.SetActive(true);
        StartCoroutine(LoadAsynchronously(level));
    }

    /// <summary>
    /// Load level async. It also update progress bar.
    /// </summary>
    /// <param name="level">Level index to be loaded</param>
    /// <returns></returns>
    IEnumerator LoadAsynchronously(int level) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progressBar.value = progress;

            yield return null;
        }
    }
}
