using SSpot.Ambient.ComputerCode;
using UnityEngine;

public class AddRemoveIterations : MonoBehaviour
{
    public bool isAdding;
    public LoopController loopController;

    /// <summary>
    /// When player clicks on this object, it adds one iteraction at LoopController.
    /// </summary>
    public void OnPointerClick()
    {
        if(isAdding)
        {
            loopController.IncreaseIterations();
        }
        else
        {
            loopController.DecreaseIterations();
        }
    }
}
