using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabirinthEnd : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Robot") return;
        LabirinthManager.instance?.LabirinthSuccess();
    }
}
