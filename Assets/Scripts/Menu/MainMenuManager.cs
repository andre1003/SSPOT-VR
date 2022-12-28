using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    // Settings button reference
    public GameObject settingsButton;


    // Platform controller
    private bool isOnPc = false;


    public void SetGamePlatform(bool isOnPc)
    {
        // Set isOnPc
        this.isOnPc = isOnPc;

        // If platform is not PC, disable settings button
        if(!isOnPc)
        {
            settingsButton.SetActive(false);
        }
    }

    #region Offline
    public void PlayOffline()
    {
        PhotonNetwork.OfflineMode = true;
    }

    /// <summary>
    /// On player joined room handler.
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Tutorial");
    }
    #endregion

    #region Online
    public void PlayOnline()
    {
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
    #endregion

    #region Master Connection
    public override void OnConnectedToMaster()
    {
        if(PhotonNetwork.OfflineMode)
        {
            Debug.Log("Starting game offline");
            PhotonNetwork.CreateRoom("singleplayer");
        }
        else
        {
            Debug.Log("Starting game online");
            PhotonNetwork.JoinLobby();
        }

    }
    #endregion

    #region Quit
    public void Quit()
    {
        Application.Quit();
    }
    #endregion
}
