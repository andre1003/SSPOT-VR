using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GamePlatformController : MonoBehaviour
{
    // Is platform a PC?
    private bool IsOnPC;


    /// <summary>
    /// Set player according to the platform.
    /// </summary>
    /// <param name="isOnPc">Is platform a PC?</param>
    public void SetGamePlatform(bool isOnPc)
    {
        // Set IsOnPC variable
        IsOnPC = isOnPc;

        // If platform is PC, disable gyro controller and enable mouse controller
        if(IsOnPC)
        {
            GetComponentInChildren<GyroController>().enabled = false;
            GetComponentInChildren<MouseController>().enabled = true;
        }

        // If platform is not PC, enable gyro controller and disable mouse controller
        else
        {
            GetComponentInChildren<GyroController>().enabled = true;
            GetComponentInChildren<MouseController>().enabled = false;
        }
    }
}
