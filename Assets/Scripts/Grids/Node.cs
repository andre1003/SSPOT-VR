using System.Collections.Generic;

namespace SSpot.Grids
{
    public class Node
    {
        public List<ILevelGridObject> Objects { get; } = new();
    }
}