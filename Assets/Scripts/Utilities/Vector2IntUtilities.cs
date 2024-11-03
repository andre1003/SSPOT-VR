using System;
using UnityEngine;

namespace SSpot.Utilities
{
    public static class Vector2IntUtilities
    {
        public static Vector2Int RotateClockwise(this Vector2Int dir)
        {
            if (dir == Vector2Int.right) return Vector2Int.down;
            if (dir == Vector2Int.down) return Vector2Int.left;
            if (dir == Vector2Int.left) return Vector2Int.up;
            if (dir == Vector2Int.up) return Vector2Int.right;
            throw new ArgumentException();
        }
        
        public static Vector2Int RotateCounterClockwise(this Vector2Int dir)
        {
            if (dir == Vector2Int.right) return Vector2Int.up;
            if (dir == Vector2Int.up) return Vector2Int.left;
            if (dir == Vector2Int.left) return Vector2Int.down;
            if (dir == Vector2Int.down) return Vector2Int.right;
            throw new ArgumentException();
        }
    }
}