using Photon.Pun;
using SSPot;
using UnityEngine;

public class TeleportToObject : MonoBehaviourPun
{
    // Locations
    public MeshRenderer teleportLocationMesh;               // Teleport location mesh

    public bool isForPlayer1 = false;
    private bool firstTime = true;

	[SerializeField] AudioObject[] clips;


	private void Awake()
    {
        if(isForPlayer1 != PhotonNetwork.IsMasterClient)
            Destroy(gameObject);
    }

    /// <summary>
    /// When player clicks on this object, it teleports the player to current GameObject position.
    /// </summary>
    public void OnPointerClick()
    {
        if(firstTime)
        {
			Voice.instance.Speak(clips);
		}

        firstTime = false;

        // Disable teleport mesh
        photonView.RPC(nameof(DisableTeleportMesh), RpcTarget.AllBuffered);

        // Teleport player
        PlayerSetup.Local.transform.position = transform.position;

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
