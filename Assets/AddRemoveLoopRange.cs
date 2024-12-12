using SSpot.Ambient.ComputerCode;
using UnityEngine;

public class AddRemoveLoopRange : MonoBehaviour
{
    public bool isAdding;
    public LoopController loopController;

    /// <summary>
    /// When player clicks on this object, it adds one block at the range of LoopController.
    /// </summary>
    public void OnPointerClick()
    {
        if (isAdding)
        {
            loopController.IncreaseRange();
        }
        else
        {
            loopController.DecreaseRange();
        }
    }
}
