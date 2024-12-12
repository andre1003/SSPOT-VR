using Photon.Pun;
using System.Collections.Generic;
using SSPot.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviourPun
{
    // Player prefab
    public GameObject playerPrefab;

    // UI elements
    public Canvas hudCanvas;

    // Spawn setup
    public List<Transform> spawnPoints;
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;
    
    private void Awake()
    {
        // Set the automatic scene sync to true
        PhotonNetwork.AutomaticallySyncScene = true;
        
        // Get index of local player
        int index = PhotonNetwork.PlayerList.FindIndex(p => p.IsLocal);
        
        // If there is a valid index, spawn the player there. Else, spawn at a random position
        Vector3 spawnPosition = index < spawnPoints.Count 
            ? spawnPoints[index].position 
            : new Vector3(Random.Range(minX, maxX), 1f, Random.Range(minZ, maxZ));

        // Spawn player on network
        var player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

        // Setup player crosshair
        hudCanvas.worldCamera = player.GetComponentInChildren<CameraPointer>().uiCamera;
    }
}
