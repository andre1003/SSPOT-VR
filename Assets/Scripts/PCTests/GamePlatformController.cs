using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlatformController : MonoBehaviour
{
    public bool IsOnPC = true;

    // Start is called before the first frame update
    void Awake()
    {
        if(IsOnPC)
        {
            GetComponentInChildren<GyroController>().enabled = false;
            GetComponentInChildren<MouseController>().enabled = true;
        }
        else
        {
            GetComponentInChildren<GyroController>().enabled = true;
            GetComponentInChildren<MouseController>().enabled = false;
        }
    }
}
