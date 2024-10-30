using UnityEngine;
using Photon.Pun;

public class GoingUpAndDownController : MonoBehaviourPun
{
    private static GameObject Player => AmbientSetup.Instance.LocalPlayer.gameObject;
    
    // GameObjects
    public GameObject playerCodingPlatform;         // Elevator platform GameObject
    public GameObject instructionsCodingPlatform;   // Elevator instructions blackboard GameObject
    public GameObject location1Initial;             // Initial location GameObject
    public GameObject instructionsInitial;          // Initial instructions blackboard GameObject
    public GameObject instructionsProgramming;      // Programming instructions blackboard GameObject

    // Audio source
    public AudioSource audioSource;                 // Ambient audio source


    // Boolean variables for controlling
    private bool isUp = false;                      // Is up boolean
    private bool isDown = true;                     // Is down boolean


    /// <summary>
    /// Elevator controller. If the platform is on the floor, the goes up. If not, goes down.
    /// </summary>
    public void OnPointerClick()
    {
        // If player is on the floor and player is on the platform, go up
        if(isDown)
        {
            GoUp();
        }
        // Else, if player is up, go down
        else if(isUp)
        {
            GoDown();
        }

        // Play audio source
        audioSource.Play();
    }

    /// <summary>
    /// Go up the elevator.
    /// </summary>
    private void GoUp()
    {
        photonView.RPC("GoUpRpc", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void GoUpRpc()
    {
        // Enable GoingUp
        Player.GetComponent<GoingUp>().enabled = true;
        playerCodingPlatform.GetComponent<GoingUp>().enabled = true;
        GetComponent<GoingUp>().enabled = true;

        // Setup GameObjects
        instructionsCodingPlatform.SetActive(false);
        instructionsInitial.SetActive(false);
        instructionsProgramming.SetActive(true);

        // Rotate this object
        transform.Rotate(transform.rotation.x + 180, transform.rotation.y, transform.rotation.z, Space.Self);

        // Setup isDown and isUp
        isDown = false;
        isUp = true;
    }

    /// <summary>
    /// Go down the elevator.
    /// </summary>
    private void GoDown()
    {
        photonView.RPC("GoDownRpc", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void GoDownRpc()
    {
        // Enable GoingDown
        Player.GetComponent<GoingDown>().enabled = true;
        playerCodingPlatform.GetComponent<GoingDown>().enabled = true;
        GetComponent<GoingDown>().enabled = true;

        // Player cannot attach any cube when going down
        Player.GetComponent<InstantiateAttachAndDestroyBlocks>().enabled = false;

        // Setup GameObjects
        instructionsCodingPlatform.SetActive(true);
        instructionsProgramming.SetActive(false);
        instructionsInitial.SetActive(true);

        // Rotate this object
        transform.Rotate(transform.rotation.x - 180, transform.rotation.y, transform.rotation.z, Space.Self);

        // Set isUp and isDown
        isUp = false;
        isDown = true;
    }
}
