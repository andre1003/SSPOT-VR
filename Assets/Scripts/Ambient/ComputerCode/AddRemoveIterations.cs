using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRemoveIterations : MonoBehaviour
{
    public bool isAdding;
    public LoopController loopController;

    /// <summary>
    /// When player clicks on this object, it teleports the player to current GameObject position.
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
