using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // Mouse sensitivity
    public float sensitivity = 200f;

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

    // Update is called once per frame
    void Update()
    {
        // Is this Photon View is not mine, exit
        if(!photonView.IsMine)
        {
            return;
        }

		// Get new mouse position
		float mouseX_frame = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime; 
		float mouseY_frame = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime; 

		pitch -= mouseY_frame;
		pitch = Mathf.Clamp(pitch, -90f, 90f);

        yaw += mouseX_frame;

		playerBody.localRotation = Quaternion.Euler(pitch, yaw, 0f);
	}
}
