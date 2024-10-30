using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;

public class AttachingCube : MonoBehaviourPun
{
    public CubeClass CurrentCube { get; private set; }

    public int cubeIndex;

    // Player
    public List<GameObject> playerHands;      // Player hands GameObject

    // Coding cell
    public GameObject cubeHolder;       // Coding cell cube holder
    [BoxGroup("CubeInfo")]
    public CubeClass cube;

    // Audio
    public AudioClip selectingCube;     // Cube select audio
    public AudioClip releasingCube;     // Cube release audio


    // Selected cube
    private GameObject selectedCube;    // Selected cube GameObject
    [SerializeField] private int playerId;

    // Audio source
    [SerializeField] private AudioSource audioSource;    // Audio source

    /// <summary>
    /// When player click on this object, it attaches a cube in the coding cell
    /// </summary>
    public void OnPointerClick()
    {
        // Get local player Photon View ID
        playerId = GetPlayerID();

        // Attach a cube to a coding cell.
        Attaching();
    }

    /// <summary>
    /// Attach a cube to a coding cell.
    /// 
    /// <para>If there is a cube in player hand and the coding cell cube holder is free, then attach it to the clicked cube holder.</para>
    /// <para>If there is a cube in the coding cell but not in the player hand, remove the cube from coding cell.</para>
    /// </summary>
    private void Attaching()
    {
        // Attach cube via RPC
        AttachingHandler(playerId);
    }

    private void AttachingHandler(int id)
    {
        // Get hand
        GameObject hand = PhotonView.Find(id).gameObject.GetComponent<PlayerSetup>().playerHand;

        // Check if player hand has a child
        if(hand.transform.childCount == 1)
        {
            // Set selected cube
            selectedCube = hand.transform.GetChild(0).gameObject;
            CurrentCube = selectedCube.GetComponent<CloningCube>().Cube;

            // If is a loop cube (This gives an object reference not set because playerID is null when calling the left cell)
            if(CurrentCube.IsLoop)
            {
                ComputerCellsController.instance.GetLoopPanelAtIndex(cubeIndex).SetActive(true);
                ComputerCellsController.instance.GetLoopPanelAtIndex(cubeIndex).GetComponent<AttachingCube>().SetPlayerID(playerId);
                PlayerSetup.instance.DestroyCubeOnHand();
                
                return;
            }

            // If cube holder has no child
            if(cubeHolder.transform.childCount == 0)
            {
                int selectedCubeID = selectedCube.GetComponent<PhotonView>().ViewID;
                photonView.RPC("AttachCubeRPC", RpcTarget.AllBuffered, selectedCubeID);
            }
        }

        // If cube holder has a child and player hand has no child 
        else if(cubeHolder.transform.childCount == 1 && hand.transform.childCount == 0)
        {
            photonView.RPC("ClearCellRPC", RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    private void AttachCubeRPC(int selectedCubeID)
    {
        GameObject selectedCube = PhotonView.Find(selectedCubeID).gameObject;

        // Attach the selected cube to cubeHolder
        selectedCube.transform.SetParent(cubeHolder.transform);

        // Disable BoxCollider and EvenTrigger from selected cube
        selectedCube.GetComponent<BoxCollider>().enabled = false;

        // Set selected cube transform
        selectedCube.transform.localPosition = new Vector3(0f, 0f, 0f);     // Position
        selectedCube.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);   // Rotation
        selectedCube.transform.localScale = new Vector3(1f, 1f, 1f);        // Scale

        // Set selected cube to null
        selectedCube = null;

        // Play select cube sound
        audioSource.clip = selectingCube;
        audioSource.Play();
    }

    [PunRPC]
    public void ClearCellRPC()
    {
        // Destroy cube from cube holder
        Destroy(cubeHolder.transform.GetChild(0).gameObject);

        // Play release sound
        audioSource.clip = releasingCube;
        audioSource.Play();
    }

    /// <summary>
    /// Call RPC add player hand.
    /// </summary>
    /// <param name="playerViewId"></param>
    public void AddPlayerHand(int playerViewId)
    {
        // If this player is master, than add the hand to the list
        if(PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("AddPlayerHandRPC", RpcTarget.AllBuffered, playerViewId);
        }
    }

    /// <summary>
    /// Add player hand or playerHands list via RPC.
    /// </summary>
    /// <param name="playerViewId">Player ID.</param>
    [PunRPC]
    private void AddPlayerHandRPC(int playerViewId)
    {
        GameObject hand = PhotonView.Find(playerViewId).gameObject.GetComponent<PlayerSetup>().playerHand;
        playerHands.Add(hand);
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


    public void SetPlayerID(int id)
    {
        playerId = id;
    }
}
