using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class CloningCube : MonoBehaviourPun
{
    // Player
    public List<GameObject> playerHands;      // Player hands GameObject

    [BoxGroup("CubeInfo")]
    public CubeClass Cube;

    // Audio source
    public AudioSource audioSource;     // Audio source


    // Selected cube
    private GameObject selectedCube;    // Selected cube GameObject
    private int playerId;

    /// <summary>
    /// When player clicks on this object, it clones the selected cube to player's hand
    /// </summary>
    public void OnPointerClick()
    {
        // Get local player index
        playerId = GetPlayerID();

        // Destroy cube on hand, if there is any
        PlayerSetup.instance.DestroyCubeOnHand();

        // Attach the selected cube to the player's hand
        AttachCubeToHand();
    }

    private void AttachCubeToHand()
    {
        // Instanciate the selected object
        GameObject _selectedCube = PhotonNetwork.Instantiate(gameObject.name, Vector3.zero, Quaternion.identity);
        int cubeId = _selectedCube.GetComponent<PhotonView>().ViewID;

        // Cloning Cube Class
        _selectedCube.GetComponent<CloningCube>().Cube = Cube;

        // Setup selected cube via RPC
        photonView.RPC("SetupSelectedCube", RpcTarget.AllBuffered, cubeId, playerId);
    }

    /// <summary>
    /// Setup the selected cube via RPC.
    /// </summary>
    /// <param name="name">Cube name.</param>
    /// <param name="id">Player ID.</param>
    [PunRPC]
    private void SetupSelectedCube(int cubeId, int id)
    {
        // Find selected cube
        selectedCube = PhotonView.Find(cubeId).gameObject;

        // If there is no selected cube, exit
        if(!selectedCube)
        {
            Debug.LogError("No selected cubes!");
            return;
        }

        // Attach to player's hand
        selectedCube.transform.SetParent(PhotonView.Find(id).GetComponent<PlayerSetup>().playerHand.transform);

        // Set cube transform at player's hand
        selectedCube.transform.localPosition = new Vector3(0f, -0.5f, 0.75f);   // Position
        selectedCube.transform.rotation = Quaternion.Euler(72f, 0f, 0f);        // Rotation
        selectedCube.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);      // Scale



        /* IDEA FOR SYNC THE CUBE:
         * 
         * Currently, I'm able to instantiate the cube on network for all players and
         * setup the parent, with PunRPC. But, the problem is that is setting incorrect
         * parent! As implemented in AmbientSetup.cs, the playerHand is actually the
         * local player hand, and setting the parent to playerHand forces to every
         * player set the cube parent to it's hand!!
         * 
         * That's NOT what I want. I want to set the cube parent to the player that
         * actually click the cube. For this, I might have a solution, that consists on
         * redo what I was first trying to do:
         * 
         * -> Change playerHand to be a list of GameObject, and the index is players
         * index on network. The problem with that is that I have to figure out how to
         * get player's GameObject from PhotonNetwork. Once I can do that, I should be
         * to create that list and setup cube's parent correctly.
         * 
         * UPDATE:
         * 
         * Now this works perfectly fine!
         */
    }

    /// <summary>
    /// Get local player ID.
    /// </summary>
    /// <returns>PhotonView local player ID.</returns>
    private int GetPlayerID()
    {
        return AmbientSetup.Instance.Players
            .Select(p => p.GetComponentInParent<PhotonView>())
            .First(v => v.IsMine)
            .ViewID;
    }
}
