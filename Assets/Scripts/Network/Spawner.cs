using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviourPun
{
    // Ambient setup reference
    public AmbientSetup ambientSetup;

    // Player prefab
    public GameObject playerPrefab;

    // UI elements
    public Canvas hudCanvas;
    public Text pingText;

    // Spawn setup
    public List<Transform> spawnPoints;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    // Player reference
    GameObject player;
    int viewId;


    // Start is called before the first frame update
    void Start()
    {
        // Declare spawn position and get the last player index
        Vector3 spawnPosition;
        int index = 0;

        // Find current player's index on network
        for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if(PhotonNetwork.PlayerList[i].IsLocal)
            {
                index = i;
                break;
            }
        }

        // If there are no spawn point or index is bigger equal to spawn points, get a random spawn position
        if(spawnPoints.Count == 0 || index >= spawnPoints.Count)
        {
            spawnPosition = new Vector3(Random.Range(minX, maxX), 1f, Random.Range(minZ, maxZ));
        }

        // If there is a valid spawn point, get it
        else
        {
            spawnPosition = spawnPoints[index].position;
        }

        // Spawn player on network
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        viewId = player.GetComponent<PhotonView>().ViewID;

        // Setup player crosshair
        hudCanvas.worldCamera = player.GetComponentInChildren<CameraPointer>().uiCamera;

        // Ambient setup
        if(ambientSetup != null)
        {
            ambientSetup.ConfigureAmbient(viewId);
            photonView.RPC("AmbientSetup", RpcTarget.AllBuffered, viewId);
        }
    }

    /// <summary>
    /// Perform RPC ambient setup.
    /// </summary>
    /// <param name="id">Player Photon View ID.</param>
    [PunRPC]
    private void AmbientSetup(int id)
    {
        ambientSetup.AddHands(id);
    }
}
