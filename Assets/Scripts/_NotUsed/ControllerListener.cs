using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerListener : MonoBehaviour
{
    public Text canvasMsg;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if(Input.GetKeyDown(kcode))
            {
                Debug.Log("Key Code down: " + kcode);
                canvasMsg.text = "Key Code down: " + kcode.ToString();
            }
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {

        }

    }




}
