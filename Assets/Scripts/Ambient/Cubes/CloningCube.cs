using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class CloningCube : MonoBehaviourPun
{
    [BoxGroup("CubeInfo")]
    public CubeClass Cube;

    // Audio source
    public AudioSource audioSource;

    /// <summary>
    /// When player clicks on this object, it clones the selected cube to player's hand
    /// </summary>
    public void OnPointerClick()
    {
        // Destroy cube on hand, if there is any
        PlayerSetup.Local.DestroyCubeOnHand();

        // Attach the selected cube to the player's hand
        AttachCubeToHand();
    }

    private void AttachCubeToHand()
    {
        // Instantiate the selected object
        GameObject cubeInstance = PhotonNetwork.Instantiate(gameObject.name, Vector3.zero, Quaternion.identity);
        CloningCube cube = cubeInstance.GetComponent<CloningCube>();
        cube.Cube = Cube;

        // Setup selected cube via RPC
        photonView.RPC(nameof(SetupSelectedCube), RpcTarget.AllBuffered, cube, PlayerSetup.Local);
    }

    /// <summary>
    /// Setup the selected cube via RPC.
    /// </summary>
    [PunRPC]
    private void SetupSelectedCube(CloningCube cube, PlayerSetup player)
    {
        // If there is no selected cube, exit
        if(!cube)
        {
            Debug.LogError("No selected cubes!");
            return;
        }

        // Attach to player's hand
        cube.transform.SetParent(player.Hand.transform);

        // Set cube transform at player's hand
        //TODO avoid using hardcoded values
        cube.transform.localPosition = new Vector3(0f, -0.5f, 0.75f);   // Position
        cube.transform.rotation = Quaternion.Euler(72f, 0f, 0f);        // Rotation
        cube.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);      // Scale
    }
}
