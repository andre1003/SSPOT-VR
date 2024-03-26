using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Attach this controller to the main camera, or an appropriate ancestor thereof, such as the "player" game object.
/// </summary>
public class GyroController : MonoBehaviour
{
    // Rotation
    private const float DRAG_RATE = .2f;    // Drag left/right to rotate the world.
    private float dragYawDegrees;

    // Start is called before the first frame update
    private void Start()
    {
        // Make sure orientation sensor is enabled.
        Input.gyro.enabled = true;

        Input.compensateSensors = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if(XRSettings.enabled)
        {
            // Unity takes care of updating camera transform in VR.
            return;
        }

        // For more information:
        // https://android-developers.blogspot.com/2010/09/one-screen-turn-deserves-another.html
        // https://developer.android.com/guide/topics/sensors/sensors_overview.html#sensors-coords
        //
        //     y                                       x
        //     |  Gyro upright phone                   |  Gyro landscape left phone
        //     |                                       |
        //     |______ x                      y  ______|
        //     /                                       \
        //    /                                         \
        //   z                                           z
        //
        //
        //     y
        //     |  z   Unity
        //     | /
        //     |/_____ x
        //

        // Update dragYawDegrees based on user touch.
        CheckDrag();

        // Set local rotation
        transform.localRotation =
          // Allow user to drag left/right to adjust direction they're facing.
          Quaternion.Euler(0f, -dragYawDegrees, 0f) *

          // Neutral position is phone held upright, not flat on a table.
          Quaternion.Euler(90f, 0f, 0f) *

          // Sensor reading, assuming default `Input.compensateSensors == true`.
          Input.gyro.attitude *

          // So image is not upside down.
          Quaternion.Euler(0f, 0f, 180f);
    }

    /// <summary>
    /// Check if player is dragging a finger on screen.
    /// </summary>
    private void CheckDrag()
    {
        // If player is not touching the screen
        if(Input.touchCount != 1)
        {
            // Exit method
            return;
        }

        // Get player touch
        Touch touch = Input.GetTouch(0);

        // Check if player is not moving the finger across the screen
        if(touch.phase != TouchPhase.Moved)
        {
            // Exit method
            return;
        }

        // Set Yaw degrees to rotate
        dragYawDegrees += touch.deltaPosition.x * DRAG_RATE;
    }
}