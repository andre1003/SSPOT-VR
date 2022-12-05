using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SwitchImmersiveNotImmersive : MonoBehaviour
{

    public float spinForce;
    bool autorization = true;
    bool ImmersiveVR = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, spinForce * Time.deltaTime, 0);
    }

    public void Autorization()
    {
        autorization = true;
    }

    public void ChangeVRMode()
    {

        Debug.Log("chamou a funcao");
        //Debug.Log(GvrIntent.IsLaunchedFromVr());

        if(autorization)
        {

            Debug.Log("autorizaton true");

            if(ImmersiveVR)
            {
                Debug.Log("immersive vr true");

                StartCoroutine(SwitchToNotImmersive());
                ImmersiveVR = false;
                spinForce = -spinForce;

            }
            else
            {

                Debug.Log("immersive vr false");

                StartCoroutine(SwitchToImmersive());
                ImmersiveVR = true;
                spinForce = -spinForce;

            }
        }

    }

    // Call via `StartCoroutine(SwitchToVR())` from your code. Or, use
    // `yield SwitchToVR()` if calling from inside another coroutine.
    IEnumerator SwitchToImmersive()
    {
        // Device names are lowercase, as returned by `XRSettings.supportedDevices`.
        string desiredDevice = "cardboard"; // Or "cardboard".

        // Some VR Devices do not support reloading when already active, see
        // https://docs.unity3d.com/ScriptReference/XR.XRSettings.LoadDeviceByName.html
        if(String.Compare(XRSettings.loadedDeviceName, desiredDevice, true) != 0)
        {
            XRSettings.LoadDeviceByName(desiredDevice);

            // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
            yield return null;
        }

        // Now it's ok to enable VR mode.
        XRSettings.enabled = true;
    }


    // Call via `StartCoroutine(SwitchTo2D())` from your code. Or, use
    // `yield SwitchTo2D()` if calling from inside another coroutine.
    IEnumerator SwitchToNotImmersive()
    {
        // Empty string loads the "None" device.
        XRSettings.LoadDeviceByName("");

        // Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
        yield return null;

        // Not needed, since loading the None (`""`) device takes care of this.
        XRSettings.enabled = false;

        // Restore 2D camera settings.
        ResetCameras();
    }

    // Resets camera transform and settings on all enabled eye cameras.
    void ResetCameras()
    {
        // Camera looping logic copied from GvrEditorEmulator.cs
        for(int i = 0; i < Camera.allCameras.Length; i++)
        {
            Camera cam = Camera.allCameras[i];
            if(cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None)
            {

                // Reset local position.
                // Only required if you change the camera's local position while in 2D mode.
                cam.transform.localPosition = Vector3.zero;

                // Reset local rotation.
                // Only required if you change the camera's local rotation while in 2D mode.
                cam.transform.localRotation = Quaternion.identity;

                // No longer needed, see issue github.com/googlevr/gvr-unity-sdk/issues/628.
                // cam.ResetAspect();

                // No need to reset `fieldOfView`, since it's reset automatically.
            }
        }
    }



}
