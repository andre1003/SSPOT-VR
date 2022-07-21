using UnityEngine;
using UnityEngine.UI;

public class VRCObject : MonoBehaviour {
    // Audio source
    public AudioSource audioSource; // Audio source


    /// <summary>
    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    /// </summary>
    public void OnPointerEnter() {
        audioSource.Play();                         // Play audio source
    }

    /// <summary>
    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    /// </summary>
    public void OnPointerExit() {

    }
}
