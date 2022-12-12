using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Localization.Settings;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public InputField roomNameInput;
    public Text statusText;


    private void Start()
    {
        statusText.text = "";
    }

    #region Create Room
    /// <summary>
    /// Create a room in Photon server.
    /// </summary>
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(roomNameInput.text);
    }

    /// <summary>
    /// On room creation failed handler.
    /// </summary>
    /// <param name="returnCode">- Error code.</param>
    /// <param name="message">- Error message.</param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Switch return code cases
        switch(returnCode)
        {
            // Room already exists
            case 32766:
                statusText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Lobby", "RoomAlredyExist");
                break;

            // Default error
            default:
                statusText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Lobby", "CreateRoomErrorDefault");
                break;
        }

        // Debug error message and error code
        Debug.Log("Error message: " + message + " - Error code: " + returnCode);
    }
    #endregion

    #region Join Room    
    /// <summary>
    /// Join a room in Photon server.
    /// </summary>
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomNameInput.text);
    }

    /// <summary>
    /// On join room failed handler.
    /// </summary>
    /// <param name="returnCode">- Error code.</param>
    /// <param name="message">- Error message.</param>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        // Switch return code cases
        switch(returnCode)
        {
            // Room is full
            case 32765:
                statusText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Lobby", "FullRoom");
                break;

            // Room does not exist
            case 32758:
                statusText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Lobby", "RoomDoesNotExist");
                break;

            // Default error
            default:
                statusText.text = LocalizationSettings.StringDatabase.GetLocalizedString("Lobby", "JoinRoomErrorDefault");
                break;
        }

        // Debug error message and error code
        Debug.Log("Error message: " + message + " - Error code: " + returnCode);
    }

    /// <summary>
    /// On player joined room handler.
    /// </summary>
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Tutorial");
    }
    #endregion

    #region Back to Main Menu
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}
