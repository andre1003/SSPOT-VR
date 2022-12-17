using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class AttachingCube : MonoBehaviourPun
{
    public int cubeIndex;
    public bool isLeftCell;

    // Player
    public GameObject playerHands;      // Player hand

    // Coding cell
    public GameObject cubeHolder;       // Coding cell cube holder

    // Audio
    public AudioClip selectingCube;     // Cube select audio
    public AudioClip releasingCube;     // Cube release audio


    // Selected cube
    private GameObject selectedCube;    // Selected cube GameObject

    // Audio source
    [SerializeField] private AudioSource audioSource;    // Audio source

    /// <summary>
    /// Called before Start.
    /// </summary>
    private void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// When player click on this object, it attaches a cube in the coding cell
    /// </summary>
    public void OnPointerClick()
    {
        // Attach a cube to a coding cell.
        //this.photonView.RPC("Attaching", RpcTarget.AllBuffered);
        Attaching();
    }

    /// <summary>
    /// Attach a cube to a coding cell.
    /// 
    /// <para>If there is a cube in player hand and the coding cell cube holder is free, then attach it to the clicked cube holder.</para>
    /// <para>If there is a cube in the coding cell but not in the player hand, remove the cube from coding cell.</para>
    /// </summary>
    //[PunRPC]
    private void Attaching()
    {
        // Check if player hand has a child
        if(playerHands.transform.childCount == 1)
        {
            // Set selected cube
            selectedCube = playerHands.transform.GetChild(0).gameObject;


            // If is a loop cube (this gives an index out of bounds error!)
            if(selectedCube.name.StartsWith("Repeat") && !isLeftCell)
            {
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).SetActive(true);
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).GetComponent<AttachingCube>().Attaching();
                ComputerCellsController.instance.GetRightCellAtIndex(cubeIndex).SetActive(true);
                return;
            }
            else if(selectedCube.name.StartsWith("EndRepeat") && !isLeftCell)
            {
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).SetActive(true);
                ComputerCellsController.instance.GetLeftCellAtIndex(cubeIndex).GetComponent<AttachingCube>().Attaching();
                return;
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
        else if(cubeHolder.transform.childCount == 1 && playerHands.transform.childCount == 0)
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
}
