using UnityEngine;
using Photon.Pun;
using SSpot.Ambient.ComputerCode;

public class AttachingCube : MonoBehaviourPun
{
    public CodingCell ParentCell { get; set; }
    
    public CubeClass CurrentCube { get; private set; }

    // Coding cell
    public GameObject cubeHolder;       // Coding cell cube holder

    // Audio
    public AudioClip selectingCube;     // Cube select audio
    public AudioClip releasingCube;     // Cube release audio

    // Audio source
    [SerializeField] private AudioSource audioSource;    // Audio source

    /// <summary>
    /// When player click on this object, it attaches a cube in the coding cell
    /// </summary>
    public void OnPointerClick()
    {
        GameObject hand = PlayerSetup.Local.Hand;
        AttachingHandler(hand);
    }

    /// <summary>
    /// Attach a cube to a coding cell.
    /// 
    /// <para>If there is a cube in player hand and the coding cell cube holder is free, then attach it to the clicked cube holder.</para>
    /// <para>If there is a cube in the coding cell but not in the player hand, remove the cube from coding cell.</para>
    /// </summary>
    private void AttachingHandler(GameObject hand)
    {
        if (hand.transform.childCount == 1)
        {
            var cube = hand.transform.GetChild(0).GetComponent<CloningCube>();
            AttachCube(cube);
        }
        else if (cubeHolder.transform.childCount == 1)
        {
            ClearCellRPC();
        }
    }

    private void AttachCube(CloningCube selectedCube)
    {
        if(selectedCube.Cube.IsLoop)
        {
            ParentCell.SetLoop(true);
            PlayerSetup.Local.DestroyCubeOnHand();
        }
        else
        {
            photonView.RPC(nameof(ReplaceCubeRPC), RpcTarget.AllBuffered, selectedCube);
        }
    }

    [PunRPC]
    private void ReplaceCubeRPC(CloningCube newCube)
    {
        if (CurrentCube != null)
            ClearCellRPC();
        AttachCubeRPC(newCube);
    }


    [PunRPC]
    private void AttachCubeRPC(CloningCube selectedCube)
    {
        // Attach the selected cube to cubeHolder
        selectedCube.transform.SetParent(cubeHolder.transform);

        // Disable BoxCollider and EvenTrigger from selected cube
        selectedCube.GetComponent<BoxCollider>().enabled = false;

        // Set selected cube transform
        selectedCube.transform.localPosition = Vector3.zero;
        selectedCube.transform.rotation = Quaternion.identity;
        selectedCube.transform.localScale = Vector3.one;

        // Play select cube sound
        audioSource.clip = selectingCube;
        audioSource.Play();
        
        CurrentCube = selectedCube.GetComponent<CloningCube>().Cube;
    }

    [PunRPC]
    public void ClearCellRPC()
    {
        if (CurrentCube == null)
            return;
        
        // Destroy cube from cube holder
        Destroy(cubeHolder.transform.GetChild(0).gameObject);

        // Play release sound
        audioSource.clip = releasingCube;
        audioSource.Play();

        CurrentCube = null;
    }
}
