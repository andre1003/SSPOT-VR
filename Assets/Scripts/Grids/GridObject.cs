using Photon.Pun;
using UnityEngine;

namespace SSpot.Grids
{
    public class GridObject : MonoBehaviourPun, ILevelGridObject
    {
        public LevelGrid Grid { get; set; }
        
        public Vector2Int GridPosition { get; set; }
        
        //TODO proper handling of Facing
        public Vector2Int Facing
        {
            get
            {
                Vector3 forward = transform.forward;
                return new((int)forward.x, (int)forward.z);
            }
        }

        [field: SerializeField] 
        public bool CanWalkThrough { get; private set; } = true;

        public virtual void OnSteppedOn()
        {
        }
    }
}