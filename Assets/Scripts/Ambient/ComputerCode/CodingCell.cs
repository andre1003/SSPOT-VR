using Photon.Pun;
using SSpot.Level;
using UnityEngine;

namespace SSpot.Ambient.ComputerCode
{
    public class CodingCell : MonoBehaviourPun
    {
        public CubeClass CurrentCube => AttachingCube.CurrentCube;
        
        public bool HasLoop => LoopController != null && LoopController.gameObject.activeSelf;
        
        [field: SerializeField]
        public AttachingCube AttachingCube { get; private set; }
        
        [field: SerializeField]
        public LoopController LoopController { get; private set; }
        
        public CubeComputer Computer { get; private set; }
        public int Index { get; private set; }
        
        public void Init(int index, CubeComputer computer)
        {
            Index = index;
            Computer = computer;
        }

        private void Awake()
        {
            AttachingCube.ParentCell = this;

            if (LoopController)
                LoopController.ParentCell = this;
        }

        public void Clear()
        {
            if (CurrentCube != null)
                AttachingCube.ClearCellRPC();
            
            //if (LoopController)
            //    LoopController.Reset();
        }
        
        public void SetLoop(bool loopActive)
        {
            if (LoopController == null)
            {
                Debug.LogError($"Null LoopController but called {nameof(SetLoop)}(true)", gameObject);
                return;
            }
            
            photonView.RPC(nameof(SetLoopRpc), RpcTarget.AllBuffered, loopActive);
        }

        [PunRPC]
        private void SetLoopRpc(bool loopActive)
        {
            LoopController.gameObject.SetActive(loopActive);
        }
    }
}