using UnityEngine;
using UnityEngine.UI;

public class VRCObject : MonoBehaviour {
    // Audio source
    public AudioSource audioSource; // Audio source

    // Crosshair
    public Image reticle;           // Crosshair reticle


    // New scale
    private Vector3 newScale;       // Target scale


    /// <summary>
    /// Awake is called before everthing.
    /// </summary>
    private void Awake() {
        newScale = reticle.rectTransform.localScale; // Set initial target scale to current image local scale
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
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
        audioSource.Play();                         // Play audio source
        newScale = new Vector3(2.5f, 2.75f, 2.5f);  // Set target scale
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit() {
        newScale = new Vector3(0.5f, 0.55f, 0.5f);  // Set target scale
    }
}
