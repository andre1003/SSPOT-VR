using UnityEngine;

public class TeleportToObject : MonoBehaviour {
    // Player
    public GameObject player;                               // Player GameObject

    // Locations
    public GameObject initialLocation;                      // Initial location GameObject
    public MeshRenderer teleportLocationMesh;               // Teleport location mesh

    // Elevator
    public GoingUpAndDownController upAndDownController;    // Elevator controller

    // Audio source
    public AudioSource audioSource;                         // Audio source


    /// <summary>
    /// When player clicks on this object, it teleports the player to current GameObject position.
    /// </summary>
    public void OnPointerClick() {
        // Disable elevator teleport button mesh
        teleportLocationMesh.enabled = false;

        // Teleport player
        player.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // Activate initial location for teleport
        initialLocation.SetActive(true);

        // Enable elevator button
        upAndDownController.enabled = true;
    }
}
