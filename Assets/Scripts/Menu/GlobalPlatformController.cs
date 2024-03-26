using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalPlatformController : MonoBehaviour
{
    // Is platform a PC?
    public bool isOnPc = true;


    void Awake()
    {
        // Delegate OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Don't destroy this on scene load
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Called when a new scene is loaded.
    /// </summary>
    /// <param name="scene">Scene loaded.</param>
    /// <param name="mode">Load scene mode.</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wait 0.25 seconds to setup platform. This is needed, because player spawn have a delay
        StartCoroutine(Wait(0.25f));
    }

    /// <summary>
    /// Wait for seconds and setup platform.
    /// </summary>
    /// <param name="seconds">Seconds to wait.</param>
    private IEnumerator Wait(float seconds)
    {
        // Wait for seconds
        yield return new WaitForSeconds(seconds);

        // Setup platform
        SetupPlatform();
    }

    /// <summary>
    /// Setup game platform for all players.
    /// </summary>
    private void SetupPlatform()
    {
        // Get main menu
        GameObject mainMenu = GameObject.Find("MainMenuManager");

        // If there is a main menu, set game platform
        if (mainMenu)
        {
            mainMenu.GetComponent<MainMenuManager>().SetGamePlatform(isOnPc);
        }

        // Get all players GameObject
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        // Loop players, setting the proper game platform
        foreach(GameObject player in players)
        {
            player.GetComponent<GamePlatformController>().SetGamePlatform(isOnPc);
        }
    }
}
