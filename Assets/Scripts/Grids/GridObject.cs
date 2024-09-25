using Photon.Pun;
using UnityEngine;

namespace SSpot.Grids
{
    public class GridObject : MonoBehaviourPun, ILevelGridObject
    {
        public LevelGrid Grid { get; set; }
        
        public Vector2Int GridPosition { get; set; }

        [field: SerializeField] 
        public bool CanWalkThrough { get; private set; } = true;

        public virtual void OnSteppedOn()
        {
        }
    }
}