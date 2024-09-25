using System.Collections.Generic;
using UnityEngine;

namespace SSpot.Grids
{
    public class Node
    {
        public Vector2Int Cell { get; }
        
        public List<ILevelGridObject> Objects { get; } = new();

        public bool CanWalk { get; set; } = true;

        public Node(Vector2Int cell)
        {
            Cell = cell;
        }
    }
}