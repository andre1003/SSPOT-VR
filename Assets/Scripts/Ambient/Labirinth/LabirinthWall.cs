using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabirinthWall : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Robot") return;
        ResetLabirinth.instance?.WrongWay();
    }
}
