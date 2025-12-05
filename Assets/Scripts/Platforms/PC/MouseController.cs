using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Mouse sensitivity
    public float sensitivity = 150f;

    // Player body transform
    public Transform playerBody;


    // Photon View reference
    private PhotonView photonView;

	// Mouse position
	private float mouseX;
	private float mouseY;


	// Start is called before the first frame update
	void Start()
    {
        // Get Photon View component
        photonView = GetComponentInParent<PhotonView>();

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

	void Update()
	{
		// Is this Photon View is not mine, exit
		if (!photonView.IsMine)
		{
			return;
		}

		// Get new mouse position

        mouseX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;


        // Limit mouseY to -90 and 90 (looking up and down)
        if(mouseY > 90f)
		{
			mouseY = 90f;
		}
		else if (mouseY < -90f)
		{
			mouseY = -90f;
		}


		// Rotate player body
		playerBody.rotation = Quaternion.Euler(-mouseY, mouseX, 0f); 
		//transform.localRotation = Quaternion.Euler(-mouseY, 0f, 0f);
	}
}
