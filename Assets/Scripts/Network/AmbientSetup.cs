using Photon.Pun;
using System.Collections.Generic;
using SSpot.Level;
using SSpot.Utilities;
using UnityEngine;

public class AmbientSetup : Singleton<AmbientSetup>
{
    public GoingUpAndDownController elevator;
    public CubeComputer computer;
    public List<CloningCube> cloningCubesList = new();
    public List<TeleportToObject> teleports = new();
    
    private List<GameObject> players = new List<GameObject>();
    public IReadOnlyList<GameObject> Players => players;
    
    [SerializeField] private GameObject computerToHide;

    private void Start()
    {
        if(PhotonNetwork.OfflineMode && teleports.Count > 0)
        {
            teleports[2].enabled = false;
            teleports[2].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Configure ambient with local player settings.
    /// </summary>
    /// <param name="playerId">Local player Photon View ID.</param>
    public void ConfigureAmbient(int playerId)
    {
        GameObject player = PhotonView.Find(playerId).gameObject;
        players.Add(player);

        // Setup teleports
        if(teleports.Count > 0)
        {
            ConfigureTeleport(player);
        }

        // Setup elevator
        if(elevator)
        {
            ConfigureElevator(player);
        }
    }

    /// <summary>
    /// Add local player hand to hands lists from every needed script.
    /// </summary>
    /// <param name="playerId">Local player Photon View ID.</param>
    public void AddHands(int playerId)
    {
        ConfigureCloningCubes(playerId);
        ConfigureComputer(playerId);
    }
    
    private void ConfigureComputer(int playerId)
    {
        if (computerToHide)
            computerToHide.SetActive(false);
        
        if (computer)
            computer.AddPlayerHand(playerId);
    }

    private void ConfigureCloningCubes(int playerId)
    {
        /*foreach (CloningCube cloningCube in cloningCubesList)
        {
            cloningCube.AddPlayerHand(playerId);
        }*/
    }
    
    private void ConfigureTeleport(GameObject player)
    {
        foreach (TeleportToObject teleport in teleports)
        {
            teleport.player = player;
        }
    }
    
    private void ConfigureElevator(GameObject player)
    {
        elevator.player = player;
    }
}
