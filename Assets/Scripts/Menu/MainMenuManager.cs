using UnityEngine;
using Photon.Pun;
using SSPot.Scenes;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    // Settings button reference
    public GameObject settingsButton;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if(PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }


    public void SetGamePlatform(bool isOnPc)
    {
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
    
    public override void OnJoinedRoom()
    {
        SceneLoader.Instance.LoadTutorial();
    }
    #endregion

    #region Online
    public void PlayOnline()
    {
        SceneLoader.Instance.OpenLoadingScene(showConnectionMessage: true);
        
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnJoinedLobby()
    {
        SceneLoader.Instance.LoadLobby();
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
