using Photon.Pun;
using System.Collections.Generic;
using SSpot.Robot;
using UnityEngine;

public class ResetCubes : MonoBehaviourPun
{
    [SerializeField] private bool clearBlocksOnReset = true;
    
    // Terminal stuff
    [SerializeField] private List<GameObject> codingCells = new();
    [SerializeField] private GameObject terminal;
    [SerializeField] private Material originalTerminalMaterial;
    [SerializeField] private RunCubes runCubes;

    //Environment
    [SerializeField] private RobotData robot;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerClick() => Reset();

    public void Reset()
    {
        if(PhotonNetwork.OfflineMode)
            ResetRpc();
        else
            photonView.RPC(nameof(ResetRpc), RpcTarget.AllBuffered);
    }
    
    [PunRPC]
    private void ResetRpc()
    {
        PlayerSetup.instance.DestroyCubeOnHand();
        
        if (clearBlocksOnReset)
        {
            ClearCubes();
        }
        
        // Reset robot
        robot.Reset();

        // Reset terminal materials
        var mats = terminal.GetComponent<MeshRenderer>().materials;
        mats[1] = originalTerminalMaterial;
        terminal.GetComponent<MeshRenderer>().materials = mats;

        // Play audio source
        _audioSource.Play();

        // Reset cubes list from RunCubes script
        //TODO ResetComputer was used in ResetCubesAlt
        //runCubes.ResetComputer();
        runCubes.mainInstructions.Clear();
    }

    /// <summary> Destroys all the code cubes. </summary>
    private void ClearCubes()
    {
        foreach (var cell in codingCells)
        {
            if(cell.transform.childCount > 0)
            {
                Destroy(cell.transform.GetChild(0).gameObject);
            }
        }
    }
}
