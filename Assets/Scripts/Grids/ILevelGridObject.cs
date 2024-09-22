using UnityEngine;

namespace SSpot.Grids
{
    public interface ILevelGridObject
    {
        GameObject GameObject { get; }
        LevelGrid Grid { get; set; }
        Vector2Int GridPosition { get; set; }
    }
}