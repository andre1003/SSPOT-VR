using System;
using UnityEngine;

namespace SSpot.UI.Layout
{
    public enum LayoutDirection
    {
        Down = 0,
        Up = 1,
        Left = 2,
        Right = 3,
        CenteredTopToBottom = 4,
        CenteredBottomToTop = 5,
        CenteredLeftToRight = 6,
        CenteredRightToLeft = 7
    }
    
    public static class LayoutDirectionExtensions
    {
        public static bool IsVertical(this LayoutDirection direction) => direction is LayoutDirection.Down
            or LayoutDirection.Up 
            or LayoutDirection.CenteredTopToBottom 
            or LayoutDirection.CenteredBottomToTop;
        
        public static bool IsHorizontal(this LayoutDirection direction) => direction is LayoutDirection.Left
            or LayoutDirection.Right
            or LayoutDirection.CenteredLeftToRight
            or LayoutDirection.CenteredRightToLeft;
        
        public static Vector3 GetDirection(this LayoutDirection direction)
        {
            return direction switch
            {
                LayoutDirection.Down or LayoutDirection.CenteredTopToBottom => Vector3.down,
                LayoutDirection.Up or LayoutDirection.CenteredBottomToTop => Vector3.up,
                LayoutDirection.Left or LayoutDirection.CenteredRightToLeft => Vector3.left,
                LayoutDirection.Right or LayoutDirection.CenteredLeftToRight => Vector3.right,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
        
        public static Vector3 GetOffsetDirection(this LayoutDirection direction)
        {
            if (direction <= LayoutDirection.Right)
                return Vector3.zero;

            return -direction.GetDirection();
        }
    }
}