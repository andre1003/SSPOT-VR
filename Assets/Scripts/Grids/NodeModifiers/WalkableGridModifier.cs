using Photon.Pun;
using UnityEngine;

namespace SSpot.Grids.NodeModifiers
{
    public class WalkableGridModifier : MonoBehaviourPun, IGridNodeModifier
    {
        [SerializeField] private bool canWalk;
        
        public void Modify(LevelGrid grid, Node node)
        {
            node.CanWalk = canWalk;
        }
        
        GameObject IGridNodeModifier.GameObject => gameObject;
    }
}