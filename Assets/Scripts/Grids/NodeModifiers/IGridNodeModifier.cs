using UnityEngine;

namespace SSpot.Grids.NodeModifiers
{
    public interface IGridNodeModifier
    {
        GameObject GameObject { get; }
        
        void Modify(LevelGrid grid, Node node);
    }
}