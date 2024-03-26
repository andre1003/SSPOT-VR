using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    #region Singleton
    // PlayerSetup instance
    public static PlayerSetup instance;

    void Awake()
    {
        if(!instance)
        {
            instance = this;
        }
    }
    #endregion

    // Player hand
    public GameObject playerHand;

    // Cameras (UI and player's)
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject uiCamera;

    // Photon view reference
    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        // Get PhotonView component
        view = GetComponent<PhotonView>();

        // If it is not local player disable Camera, CameraPointer, AudioListener, PlayerSetup and destroy UI Camera
        if(!view.IsMine)
        {
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<CameraPointer>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
            Destroy(uiCamera);
            enabled = false;
        }
    }

    /// <summary>
    /// Call RPC destroy cube on hand method.
    /// </summary>
    public void DestroyCubeOnHand()
    {
        if(ThereIsCubeOnHand())
        {
            // Call DestroyCubeOnHandRpc using this View ID
            view.RPC("DestroyCubeOnHandRpc", RpcTarget.AllBuffered, view.ViewID);
        }
    }

    /// <summary>
    /// Destroy the cube this player is holding via RPC. It is important to use RPC
    /// because it needs to be synced.
    /// </summary>
    /// <param name="id">Player's PhotonView ID to remove cube.</param>
    [PunRPC]
    private void DestroyCubeOnHandRpc(int id)
    {
        // Get player hand
        GameObject playerHand = PhotonView.Find(id).GetComponent<PlayerSetup>().playerHand;

        // If there is a cube on player hand, destroy it
        if(playerHand.transform.childCount > 0)
        {
            Destroy(playerHand.transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// Check if there is any cube on this player's hand.
    /// </summary>
    /// <returns>TRUE if there is a cube on player's. FALSE if not.</returns>
    public bool ThereIsCubeOnHand()
    {
        return PhotonView.Find(view.ViewID).GetComponent<PlayerSetup>().playerHand.transform.childCount > 0;
    }
}
