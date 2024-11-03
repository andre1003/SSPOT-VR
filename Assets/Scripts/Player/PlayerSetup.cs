using System.Collections.Generic;
using Photon.Pun;
using SSpot.Utilities;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    #region Global
    
    private static readonly List<PlayerSetup> _players = new();
    public static IReadOnlyList<PlayerSetup> Players => _players;
    
    public static PlayerSetup Local { get; private set; }
    
    public static PlayerSetup GetPlayer(int id) => _players.Find(p => p.photonView.ViewID == id);
    
    #endregion

    public int PlayerIndex => Players.IndexOf(this);
    
    public GameObject Hand => playerHand;

    public bool IsLocal => photonView.IsMine;

    // Player hand
    [SerializeField] private GameObject playerHand;

    // Cameras (UI and player's)
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject uiCamera;
    
    private void Awake()
    {
        //Register global
        _players.Add(this);
        if (photonView.IsMine)
            Local = this;
        
        // If it is not local player disable Camera, CameraPointer, AudioListener, PlayerSetup and destroy UI Camera
        if (photonView.IsMine) return;
        
        playerCamera.GetComponent<Camera>().enabled = false;
        playerCamera.GetComponent<CameraPointer>().enabled = false;
        playerCamera.GetComponent<AudioListener>().enabled = false;
        Destroy(uiCamera);
        enabled = false;
    }
    
    private void OnDestroy()
    {
        _players.Remove(this);

        if (photonView.IsMine)
            Local = null;
    }

    /// <summary>
    /// Call RPC destroy cube on hand method.
    /// </summary>
    public void DestroyCubeOnHand()
    {
        if(playerHand.transform.childCount > 0)
        {
            // Call DestroyCubeOnHandRpc using this View ID
            photonView.RPC(nameof(DestroyCubeOnHandRpc), RpcTarget.AllBuffered);
        }
    }

    /// <summary>
    /// Destroy the cube this player is holding via RPC. It is important to use RPC
    /// because it needs to be synced.
    /// </summary>
    [PunRPC]
    private void DestroyCubeOnHandRpc()
    {
        // If there is a cube on player hand, destroy it
        if(playerHand.transform.childCount > 0)
        {
            Destroy(playerHand.transform.GetChild(0).gameObject);
        }
    }
}
