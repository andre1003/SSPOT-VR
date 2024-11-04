using System;
using UnityEngine;

namespace SSPot.Utilities.Attributes
{
    /// <summary>
    /// Draws the first child of the property instead of a nested object. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class InlineAttribute : PropertyAttribute
    {
    }
}