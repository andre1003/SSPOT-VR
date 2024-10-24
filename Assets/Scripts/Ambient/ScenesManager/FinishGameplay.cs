using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class FinishGameplay : MonoBehaviour
{
    // UI Objects
    public GameObject loadSceneCanvas;  // Loading scene canvas GameObject
    public Slider progressBar;          // Progress bar slider
    public string mainMenuLevelName = "MainMenu";

    /// <summary>
    /// When player clicks on this object, the next level is loaded
    /// </summary>
    public void OnPointerClick()
    {
        loadSceneCanvas.SetActive(true);
        PhotonNetwork.LoadLevel(mainMenuLevelName);
    }
}
