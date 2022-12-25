﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class ChangeScene : MonoBehaviour
{
    // UI Objects
    public GameObject loadSceneCanvas;  // Loading scene canvas GameObject
    public Slider progressBar;          // Progress bar slider

    // Controller
    public bool goForward = false;      // Go to next or previous level controller


    /// <summary>
    /// When player clicks on this object, the next level is loaded
    /// </summary>
    public void OnPointerClick()
    {
        // If is not master client, exit
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // Load next level
        if(goForward)
        {
            
            LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }

        // Load previous level
        else
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

    /// <summary>
    /// Starts coroutine for load a level async.
    /// </summary>
    /// <param name="level">Level index to be loaded</param>
    private void LoadLevel(int level)
    {
        // If level to load is out of bounds, exit
        if(level >= SceneManager.sceneCountInBuildSettings || level < 0)
        {
            Debug.LogError("Cena inválida!");
            return;
        }

        // Activate loading scene canvas
        loadSceneCanvas.SetActive(true);

        // Start loading next level
        PhotonNetwork.LoadLevel(level);
    }
}
