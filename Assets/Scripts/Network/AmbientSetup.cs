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


    public void ConfigureAmbient(GameObject player)
    {
        // Get player hand
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

        // Setup clone cubes
        if(cloningCubesList.Count > 0)
        {
            ConfigureCloningCubes(playerHand);
        }

        // Setup attaching cubes
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

    private void ConfigureCloningCubes(GameObject playerHand)
    {
        foreach(CloningCube cloningCube in cloningCubesList)
        {
            cloningCube.playerHands = playerHand;
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
