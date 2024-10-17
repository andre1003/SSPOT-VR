using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Collections.Generic;
using NaughtyAttributes;
using static Cube;

public class AttachingCube : MonoBehaviourPun
{
    public int cubeIndex;
    public bool isLeftCell;

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
    private CubeClass curCube;
    private int playerId;

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
        photonView.RPC("AttachingRpc", RpcTarget.AllBuffered, playerId);
    }

    [PunRPC]
    private void AttachingRpc(int id)
    {
        // Get hand
        GameObject hand = PhotonView.Find(id).gameObject.GetComponent<PlayerSetup>().playerHand;

        // Check if player hand has a child
        if(hand.transform.childCount == 1)
        {
            // Set selected cube
            selectedCube = hand.transform.GetChild(0).gameObject;
            curCube = selectedCube.GetComponent<CloningCube>().Cube;


            // If is a loop cube (This gives an object reference not set because playerID is null when calling the left cell)
            if(curCube.IsLoop && !isLeftCell)
            {
                ComputerCellsController.instance.GetLoopPanelAtIndex(cubeIndex).SetActive(true);
                ComputerCellsController.instance.GetLoopPanelAtIndex(cubeIndex).GetComponent<AttachingCube>().SetPlayerID(playerId);
                PlayerSetup.instance.DestroyCubeOnHand();
                //ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).GetComponent<AttachingCube>().Attaching();
                
                return;
            }
            else if(selectedCube.name.StartsWith("EndRepeat") && !isLeftCell)
            {
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).SetActive(true);
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).GetComponent<AttachingCube>().SetPlayerID(playerId);
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).GetComponent<AttachingCube>().Attaching();
                return;
            }
            //TODO: change loop code to new serializable class logic
            if (curCube.IsLoop)
            {

            }

            // If cube holder has no child
            if(cubeHolder.transform.childCount == 0)
            {
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
        }

        // If cube holder has a child and player hand has no child 
        else if(cubeHolder.transform.childCount == 1 && hand.transform.childCount == 0)
        {
            // Destroy cube from cube holder
            Destroy(cubeHolder.transform.GetChild(0).gameObject);

            // Play release sound
            audioSource.clip = releasingCube;
            audioSource.Play();

            // If is left cell, clear it and the correponding right cell
            if(isLeftCell)
            {
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).SetActive(false);
                ComputerCellsController.instance.GetRightCellAtIndex(cubeIndex).SetActive(false);
            }
        }
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

        if(isLeftCell)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Get local player ID.
    /// </summary>
    /// <returns>PhotonView local player ID.</returns>
    private int GetPlayerID()
    {
        // Loop player hands list
        foreach(GameObject hand in playerHands)
        {
            // Get PhotonView component of player
            PhotonView playerHandView = hand.GetComponentInParent<PhotonView>();

            // If is local player, return ViewID
            if(playerHandView.IsMine)
            {
                return playerHandView.ViewID;
            }
        }

        // If something went wrong, return -1
        return -1;
    }


    public void SetPlayerID(int id)
    {
        playerId = id;
    }
}
