using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyObjectController : MonoBehaviour {
    public GameObject description;
    public AudioSource audioSource;
    public Image reticle;

    private Vector3 newScale;


    private void Awake() {
        newScale = reticle.rectTransform.localScale;
    }

    private void Update() {
        if(reticle.rectTransform.localScale != newScale) {
            reticle.rectTransform.localScale = Vector3.Lerp(reticle.rectTransform.localScale, newScale, 3f * Time.deltaTime);
        }
    }

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter() {
        description.SetActive(true);
        audioSource.Play();
        newScale = new Vector3(2.5f, 2.75f, 2.5f);
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit() {
        description.SetActive(false);
        newScale = new Vector3(0.5f, 0.55f, 0.5f);
    }

    public void OnPointerClick() {

    }
}
