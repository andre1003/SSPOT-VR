using System;
using UnityEngine;

namespace SSpot.Grids
{
    public interface ILevelGridObject : IGameObjectProvider
    {
        LevelGrid Grid { get; set; }
        
        Vector2Int GridPosition { get; set; }
        
        bool CanWalkThrough { get; }

        bool TriggerOnSteppedOn => false;
        void OnSteppedOn() {}
    }
}