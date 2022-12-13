using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public float sensitivity = 200f;
    public Transform playerBody;


    private float xRotation = 0f;
    private PhotonView photonView;

    float mouseX;
    float mouseY;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponentInParent<PhotonView>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //playerBody.Rotate(Vector3.zero);
        //transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        mouseX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -45f, 45f);

        //transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //playerBody.Rotate(Vector3.up * mouseX);

        //transform.localRotation = Quaternion.Euler(-mouseY, mouseX, 0f);

        // I don't know why, but this works (with errors)
        playerBody.rotation = Quaternion.Euler(-mouseY, mouseX, 0f);
    }
}
