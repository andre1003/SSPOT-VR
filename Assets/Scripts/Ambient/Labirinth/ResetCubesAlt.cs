using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCubesAlt : MonoBehaviourPun
{
    // Terminal coding cells
    public List<GameObject> codingCell = new List<GameObject>();

    [SerializeField] private RunCubesAlt runCubes;

    // Audio source
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Awake()
    {
        // Set audio source
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// When player clicks on this object, it reset all coding cells.
    /// </summary>
    public void OnPointerClick()
    {
        ResetCallback();
    }

    // Reset callback
    public void ResetCallback()
    {
        if(PhotonNetwork.OfflineMode)
            ExecuteReset();
        else
            photonView.RPC("ExecuteReset", RpcTarget.AllBuffered);
    }

    // Reset method
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

        // Stop robot movment and reset its position
        runCubes.robotMovement.StopMovement();
        ResetLabirinth.instance?.ResetRobotPosition();

        // Play audio source
        audioSource.Play();

        // Reset cubes list from RunCubes script
        runCubes.ResetComputer();
    }
}
