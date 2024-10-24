using Photon.Pun;
using UnityEngine;

public class TeleportToObject : MonoBehaviourPun
{
    // Player
    public GameObject player;                               // Player GameObject

    // Locations
    public MeshRenderer teleportLocationMesh;               // Teleport location mesh

    public bool isForPlayer1 = false;


    void Awake()
    {
        if(isForPlayer1 != PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
    }

    /// <summary>
    /// When player clicks on this object, it teleports the player to current GameObject position.
    /// </summary>
    public void OnPointerClick()
    {
        // Disable teleport mesh
        photonView.RPC("DisableTeleportMesh", RpcTarget.AllBuffered);

        // Teleport player
        player.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Add player on elevator
        ElevatorSync.instance.AddPlayerOnElevator();
    }
    
    
    [PunRPC]    
    private void DisableTeleportMesh()
    {
        // Disable elevator teleport button mesh
        teleportLocationMesh.enabled = false;
    }
}
