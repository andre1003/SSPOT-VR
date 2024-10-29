using System;
using System.Collections.Generic;
using System.Linq;
using SSpot.Utilities;
using UnityEngine;

namespace SSpot.UI
{
    [ExecuteAlways]
    public class ObjectLayout : MonoBehaviour
    {
        public enum Direction
        {
            Down = 0,
            Up = 1,
            Left = 2,
            Right = 3,
            VerticalCentered = 4,
            HorizontalCentered = 5
        }
            
        public float distanceBetweenObjects = 1f;
        public Direction direction = Direction.Down;
    
        private void Update() => UpdatePositions();
    
        private void UpdatePositions()
        {
            var activeChildren = transform.ActiveChildren().ToList();
            
            Vector3 pos = GetOffset(activeChildren.Count);
            foreach (var child in activeChildren)
            {
                child.localPosition = pos;
                pos += GetDirection(direction) * distanceBetweenObjects;
            }
        }
        
        private Vector3 GetOffset(int count)
        {
            float totalWidth = distanceBetweenObjects * (count - 1);
            return direction switch
            {
                Direction.VerticalCentered => Vector3.up * totalWidth / 2f,
                Direction.HorizontalCentered => Vector3.right * totalWidth / 2f,
                _ => Vector3.zero
            };
        }
            
        private static Vector3 GetDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Vector3.up,
                Direction.Down or Direction.VerticalCentered => Vector3.down,
                Direction.Left or Direction.HorizontalCentered => Vector3.left,
                Direction.Right => Vector3.right,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}