using UnityEngine;

public class GamePlatformController : MonoBehaviour
{
    private GyroController _gyroController;
    private MouseController _mouseController;
    
    private void Awake()
    {
        _gyroController = GetComponentInChildren<GyroController>();
        _mouseController = GetComponentInChildren<MouseController>();

        bool isMobile = Application.isMobilePlatform;
        SetPlatform(isMobile);
    }

    private void SetPlatform(bool isMobile)
    {
        _gyroController.enabled = isMobile;
        _mouseController.enabled = !isMobile;
    }
}
