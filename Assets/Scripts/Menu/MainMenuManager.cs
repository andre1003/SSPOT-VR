using Photon.Pun;
using SSPot.Scenes;
using UnityEngine;

namespace SSPot.Menu
{
    public class MainMenuManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject settingsButton;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        
            if(PhotonNetwork.InLobby)
                PhotonNetwork.LeaveLobby();
            if(PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();

            // If platform is mobile, disable settings button
            bool isMobile = Application.isMobilePlatform;
            if (isMobile) settingsButton.SetActive(false);
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
}