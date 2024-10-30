using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;

public class AttachingCube : MonoBehaviourPun
{
    public CubeClass CurrentCube { get; private set; }

    public int cubeIndex;

    // Coding cell
    public GameObject cubeHolder;       // Coding cell cube holder
    [BoxGroup("CubeInfo")]
    public CubeClass cube;

    // Audio
    public AudioClip selectingCube;     // Cube select audio
    public AudioClip releasingCube;     // Cube release audio


    // Selected cube
    private GameObject selectedCube;    // Selected cube GameObject

    // Audio source
    [SerializeField] private AudioSource audioSource;    // Audio source

    /// <summary>
    /// When player click on this object, it attaches a cube in the coding cell
    /// </summary>
    public void OnPointerClick()
    {
        int localPlayerId = AmbientSetup.Instance.LocalPlayer.ViewID;
        AttachingHandler(localPlayerId);
    }

    /// <summary>
    /// Attach a cube to a coding cell.
    /// 
    /// <para>If there is a cube in player hand and the coding cell cube holder is free, then attach it to the clicked cube holder.</para>
    /// <para>If there is a cube in the coding cell but not in the player hand, remove the cube from coding cell.</para>
    /// </summary>
    private void AttachingHandler(int playerId)
    {
        //TODO dude I think I can get rid of AmbientSetup and just use PhotonView.Find
        // Get hand
        GameObject hand = PhotonView.Find(playerId).gameObject.GetComponent<PlayerSetup>().playerHand;

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
                PlayerSetup.instance.DestroyCubeOnHand();
                
                return;
            }

            // If cube holder has no child
            if(cubeHolder.transform.childCount == 0)
            {
                int selectedCubeID = selectedCube.GetComponent<PhotonView>().ViewID;
                photonView.RPC(nameof(AttachCubeRPC), RpcTarget.AllBuffered, selectedCubeID);
            }
        }

        // If cube holder has a child and player hand has no child 
        else if(cubeHolder.transform.childCount == 1 && hand.transform.childCount == 0)
        {
            photonView.RPC(nameof(ClearCellRPC), RpcTarget.AllBuffered);
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
}
