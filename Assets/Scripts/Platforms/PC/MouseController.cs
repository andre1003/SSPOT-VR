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
	private float pitch = 0f;
    private float yaw = 0f;


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
		float mouseX_frame = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; 
		float mouseY_frame = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime; 

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

        yaw += mouseX_frame;

		playerBody.localRotation = Quaternion.Euler(pitch, yaw, 0f);
	}
}
