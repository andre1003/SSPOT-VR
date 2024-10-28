using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSetup : MonoBehaviour
{
    #region Singleton
    public static AmbientSetup instance;

    void Awake()
    {
        if(!instance)
        {
            instance = this;
        }

        if(PhotonNetwork.OfflineMode && teleports.Count > 0)
        {
            teleports[2].enabled = false;
            teleports[2].gameObject.SetActive(false);
        }
    }
    #endregion


    // Run and reset cubes
    public RunCubes runCubes;
    public ResetCubes resetCubes;

    // Teleport
    public List<TeleportToObject> teleports = new List<TeleportToObject>();

    // Elevator
    public GoingUpAndDownController elevator;

    // Clone and attaching cube lists
    public List<CloningCube> cloningCubesList;
    public List<AttachingCube> attachingCubesList;


    // Players list
    private List<GameObject> players = new List<GameObject>();

    // Hide computer
    [SerializeField] private GameObject computerToHide;


    /// <summary>
    /// Configure ambient with local player settings.
    /// </summary>
    /// <param name="playerId">Local player Photon View ID.</param>
    public void ConfigureAmbient(int playerId)
    {
        // Get player hand
        GameObject player = PhotonView.Find(playerId).gameObject;
        GameObject playerHand = player.GetComponent<PlayerSetup>().playerHand;

        // Add player game object
        players.Add(player);

        // Setup computer
        if(runCubes)
        {
            ConfigureComputer(playerHand);
        }

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
        // Setup clone cubes
        if(cloningCubesList.Count > 0)
        {
            ConfigureCloningCubes(playerId);
        }

        // Setup attaching cubes
        if(attachingCubesList.Count > 0)
        {
            ConfigureAttachingCubes(playerId);
        }
    }

    /// <summary>
    /// Get all players GameObjects.
    /// </summary>
    /// <returns>Players GameObject.</returns>
    public List<GameObject> GetPlayers()
    {
        return players;
    }


    /// <summary>
    /// Configure scene computer.
    /// </summary>
    /// <param name="playerHand">Player hand reference.</param>
    private void ConfigureComputer(GameObject playerHand)
    {
        //runCubes.playerHand = playerHand;
    }

    /// <summary>
    /// Configure scene teleports.
    /// </summary>
    /// <param name="player">Player reference.</param>
    private void ConfigureTeleport(GameObject player)
    {
        foreach(TeleportToObject teleport in teleports)
        {
            teleport.player = player;
        }
    }

    /// <summary>
    /// Configure elevator.
    /// </summary>
    /// <param name="player">Player reference.</param>
    private void ConfigureElevator(GameObject player)
    {
        elevator.player = player;
    }

    /// <summary>
    /// Configure all cloning cubes scripts.
    /// </summary>
    /// <param name="playerViewId">Local player Photon View ID.</param>
    private void ConfigureCloningCubes(int playerViewId)
    {
        // Loop cloning cubes scripts list, adding the player Photon View ID
        foreach(CloningCube cloningCube in cloningCubesList)
        {
            cloningCube.AddPlayerHand(playerViewId);
        }
    }

    /// <summary>
    /// Configure all attaching cubes scripts.
    /// </summary>
    /// <param name="playerViewId">Local player Photon View ID.</param>
    private void ConfigureAttachingCubes(int playerViewId)
    {
        // Loop attaching cubes scripts list, adding the player Photon View ID
        foreach(AttachingCube attachingCube in attachingCubesList)
        {
            attachingCube.AddPlayerHand(playerViewId);
        }
        if(computerToHide != null) computerToHide.SetActive(false);
    }
}
