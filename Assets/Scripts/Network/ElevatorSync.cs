using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ElevatorSync : MonoBehaviourPun
{
    #region Singleton
    public static ElevatorSync instance;

    void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }
    #endregion


    public int playersOnElevator = 0;
    public GameObject elevatorButton;


    public void AddPlayerOnElevator()
    {
        photonView.RPC("AddPlayerOnElevatorRpc", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void AddPlayerOnElevatorRpc()
    {
        // Increase number of players on elevator
        playersOnElevator++;

        // If this number is equal to number of players, enable elevator button
        if(playersOnElevator == PhotonNetwork.PlayerList.Length)
        {
            elevatorButton.SetActive(true);
            elevatorButton.GetComponent<GoingUpAndDownController>().enabled = true;
        }
    }
}
