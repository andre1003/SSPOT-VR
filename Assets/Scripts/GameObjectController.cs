using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectController : MonoBehaviour {
    public GameObject description;
    public AudioSource audioSource;

    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter() {
        if(description && audioSource) {
            description.SetActive(true);
            audioSource.Play();
        }
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit() {
        if(description)
            description.SetActive(false);
    }

    public void OnPointerClick() {

    }
}
