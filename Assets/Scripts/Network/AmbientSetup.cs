using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSetup : MonoBehaviour
{
    // Run and reset cubes
    public RunCubes runCubes;
    public ResetCubes resetCubes;

    // Teleport
    public List<TeleportToObject> teleports;

    // Elevator
    public GoingUpAndDownController elevator;

    // Clone and attaching cube lists
    public List<CloningCube> cloningCubesList;
    public List<AttachingCube> attachingCubesList;


    /// <summary>
    /// Configure ambient with local player settings.
    /// </summary>
    /// <param name="playerId">Local player Photon View ID.</param>
    public void ConfigureAmbient(int playerId)
    {
        // Get player hand
        GameObject player = PhotonView.Find(playerId).gameObject;
        GameObject playerHand = player.GetComponent<PlayerSetup>().playerHand;

        // Setup computer
        if(runCubes && resetCubes)
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
        // Get player hand
        GameObject player = PhotonView.Find(playerId).gameObject;
        GameObject playerHand = player.GetComponent<PlayerSetup>().playerHand;

        // Setup clone cubes
        if(cloningCubesList.Count > 0)
        {
            ConfigureCloningCubes(playerId);
        }

        // Setup attaching cubes (NOT WORKING YET!)
        if(attachingCubesList.Count > 0)
        {
            ConfigureAttachingCubes(playerHand);
        }
    }


    /// <summary>
    /// Configure scene computer.
    /// </summary>
    /// <param name="playerHand">Player hand reference.</param>
    private void ConfigureComputer(GameObject playerHand)
    {
        runCubes.playerHand = playerHand;
        resetCubes.playerHand = playerHand;
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

    private void ConfigureAttachingCubes(GameObject playerHand)
    {
        foreach(AttachingCube attachingCube in attachingCubesList)
        {
            attachingCube.playerHands = playerHand;
        }
    }
}
