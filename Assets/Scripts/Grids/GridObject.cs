using NaughtyAttributes;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace SSpot.Grids
{
    public class GridObject : MonoBehaviourPun, ILevelGridObject
    {
        [field: SerializeField] 
        public bool CanWalkThrough { get; private set; } = true;
        
        [field: SerializeField, BoxGroup("Events")]
        public UnityEvent SteppedOnEvent { get; private set; } = new();
        
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

        public virtual void OnSteppedOn()
        {
            SteppedOnEvent.Invoke();
        }
    }
}