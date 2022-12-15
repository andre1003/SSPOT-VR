using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    // Player hand
    public GameObject playerHand;

    // Cameras (UI and player's)
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject uiCamera;

    // Photon view reference
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if(!photonView.IsMine)
        {
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
            Destroy(uiCamera);
        }
    }
}
