using Photon.Pun;
using UnityEngine;

namespace SSpot.ComputerCode
{
    public class CodingCell : MonoBehaviourPun
    {
        public CubeClass CurrentCube => AttachingCube.CurrentCube;
        
        public bool HasLoop => LoopController != null && LoopController.gameObject.activeSelf;
        
        [field: SerializeField]
        public AttachingCube AttachingCube { get; private set; }
        
        [field: SerializeField]
        public LoopController LoopController { get; private set; }

        public void Clear()
        {
            if (CurrentCube != null)
                AttachingCube.ClearCellRPC();
            
            //if (LoopController)
            //    LoopController.Reset();
        }

        public void AddPlayerHand(int playerViewId)
        {
            AttachingCube.AddPlayerHand(playerViewId);
        }
    }
}