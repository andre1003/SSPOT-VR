using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google;
//using GoogleVR;

public class DestroyingCubeOfMyHands : MonoBehaviour
{

    public GameObject playerHands;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(/*!GvrPointerInputModule.CurrentRaycastResult.isValid &&*/ playerHands.transform.childCount == 1 && Input.GetButton("Fire1"))
        {
            Destroy(playerHands.transform.GetChild(0).gameObject);
            source.Play();
        }
    }

}
