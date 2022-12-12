using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public PhotonView photonView;

    public Canvas hudCanvas;
    public Text pingText;

    public List<Transform> spawnPoints;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;


    // Start is called before the first frame update
    void Start()
    {
        // Declare spawn position and get the last player index
        Vector3 spawnPosition;
        int index = PhotonNetwork.PlayerList.Length - 1;

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
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        // Setup player crosshair
        player.GetComponentInChildren<CameraPointer>().crosshairController = GameObject.Find("CrosshairController").GetComponent<CrosshairController>();
        hudCanvas.worldCamera = player.GetComponentInChildren<CameraPointer>().uiCamera;
    }
}
