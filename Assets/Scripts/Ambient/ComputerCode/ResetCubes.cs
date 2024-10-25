using Photon.Pun;
using System.Collections.Generic;
using SSpot.Robot;
using UnityEngine;

public class ResetCubes : MonoBehaviourPun
{
    // Terminal stuff
    public List<GameObject> codingCell = new();    // List of all coding cells
    public GameObject terminal;                                     // Terminal GameObject
    public Material originalTerminalMaterial;                       // Original terminal material

    // Robot
    public Robot robot;                                        // Robot GameObject

    // Player
    public GameObject playerHand;                                   // Player hand


    // Material
    private Material[] mats;                                        // Material vector

    // Audio source
    private AudioSource audioSource;                                // Audio source

    // Start is called before the first frame update
    void Start()
    {
        // Set audio source
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// When player clicks on this object, it reset all coding cells.
    /// </summary>
    public void OnPointerClick()
    {
        if(PhotonNetwork.OfflineMode)
            ExecuteReset();
        else
            photonView.RPC("ExecuteReset", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ExecuteReset()
    {
        // Call ResetBlocks method
        ResetBlocks();

        // If there is a cube in player's hand
        PlayerSetup.instance.DestroyCubeOnHand();
    }

    /// <summary>
    /// Reset all coding cells of terminal.
    /// </summary>
    private void ResetBlocks()
    {
        // Check every coding cell
        for(int i = 0; i < codingCell.Count; i++)
        {
            // If the coding cell has a child
            if(codingCell[i].transform.childCount > 0)
            {
                // Clear this cell
                Destroy(codingCell[i].transform.GetChild(0).gameObject);
            }
        }

        // Reset robot
        robot.Reset();

        // Reset terminal materials
        mats = terminal.GetComponent<MeshRenderer>().materials;
        mats[1] = originalTerminalMaterial;
        terminal.GetComponent<MeshRenderer>().materials = mats;

        // Play audio source
        audioSource.Play();

        // Reset cubes list from RunCubes script
        RunCubes.mainInstructions = new List<Cube>();
        RunCubes.loopCommands = new List<string>();
    }
}
