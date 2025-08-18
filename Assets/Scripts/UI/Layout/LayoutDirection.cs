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
        /// <summary>
        /// Returns true if the direction is one of the following:
        /// <list type="bullet">
        /// <item><see cref="LayoutDirection.Down"/></item>
        /// <item><see cref="LayoutDirection.Up"/></item>
        /// <item><see cref="LayoutDirection.CenteredTopToBottom"/></item>
        /// <item><see cref="LayoutDirection.CenteredBottomToTop"/></item>
        /// </list>
        /// </summary>
        public static bool IsVertical(this LayoutDirection direction) =>
            direction is LayoutDirection.Down
                or LayoutDirection.Up
                or LayoutDirection.CenteredTopToBottom
                or LayoutDirection.CenteredBottomToTop;

        /// <summary>
        /// Returns true if the direction is one of the following:
        /// <list type="bullet">
        /// <item><see cref="LayoutDirection.Left"/></item>
        /// <item><see cref="LayoutDirection.Right"/></item>
        /// <item><see cref="LayoutDirection.CenteredLeftToRight"/></item>
        /// <item><see cref="LayoutDirection.CenteredRightToLeft"/></item>
        /// </list>
        public static bool IsHorizontal(this LayoutDirection direction) =>
            direction is LayoutDirection.Left
                or LayoutDirection.Right
                or LayoutDirection.CenteredLeftToRight
                or LayoutDirection.CenteredRightToLeft;

        /// <summary>
        /// Returns true if the direction is one of the following:
        /// <list type="bullet">
        /// <item><see cref="LayoutDirection.CenteredTopToBottom"/></item>
        /// <item><see cref="LayoutDirection.CenteredBottomToTop"/></item>
        /// <item><see cref="LayoutDirection.CenteredLeftToRight"/></item>
        /// <item><see cref="LayoutDirection.CenteredRightToLeft"/></item>
        /// </list>
        /// </summary>
        public static bool IsCentered(this LayoutDirection direction) =>
            direction is LayoutDirection.CenteredTopToBottom
                or LayoutDirection.CenteredBottomToTop
                or LayoutDirection.CenteredLeftToRight
                or LayoutDirection.CenteredRightToLeft;
        
        /// <summary>
        /// Returns the Vector3 representation of the direction enum.
        /// </summary>
        public static Vector3 GetDirection(this LayoutDirection direction) =>
            direction switch
            {
                LayoutDirection.Down or LayoutDirection.CenteredTopToBottom => Vector3.down,
                LayoutDirection.Up or LayoutDirection.CenteredBottomToTop => Vector3.up,
                LayoutDirection.Left or LayoutDirection.CenteredRightToLeft => Vector3.left,
                LayoutDirection.Right or LayoutDirection.CenteredLeftToRight => Vector3.right,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
    }
}