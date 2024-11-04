using System.Collections;
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

    public string nextSceneOffline;
    public string nextSceneOnline;

    public GameObject buttonTextCanvas;

    void Awake()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Destroy(buttonTextCanvas);
            Destroy(gameObject);
        }
    }


    /// <summary>
    /// When player clicks on this object, the next level is loaded
    /// </summary>
    public void OnPointerClick()
    {
        // Load next level
        if(goForward)
        {
            LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }

        else if(nextSceneOffline != "" && nextSceneOnline != "")
        {
            if(PhotonNetwork.OfflineMode)
                LoadLevel(nextSceneOffline);
            else
                LoadLevel(nextSceneOnline);
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

    private void LoadLevel(string level)
    {
        loadSceneCanvas.SetActive(true);
        PhotonNetwork.LoadLevel(level);
    }
}
