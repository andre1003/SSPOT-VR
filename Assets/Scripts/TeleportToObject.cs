using Photon.Pun;
using UnityEngine;

public class TeleportToObject : MonoBehaviourPun
{
    // Player
    public GameObject player;                               // Player GameObject

    // Locations
    public GameObject initialLocation;                      // Initial location GameObject
    public MeshRenderer teleportLocationMesh;               // Teleport location mesh


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

        // Activate initial location for teleport
        initialLocation.SetActive(true);
        initialLocation.GetComponent<MeshRenderer>().enabled = true;
    }
}
