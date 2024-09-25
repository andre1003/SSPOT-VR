using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSpot.Grids
{
    public class Node
    {
        public Vector2Int Cell { get; }
        
        public List<ILevelGridObject> Objects { get; } = new();
        
        public GameObject NodeObject { get; set; }

        private bool _canWalk = true;
        public bool CanWalk
        {
            get => _canWalk && Objects.All(o => o.CanWalkThrough);
            set => _canWalk = value;
        }

        public Node(Vector2Int cell)
        {
            Cell = cell;
        }
    }
}