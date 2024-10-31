using System;
using UnityEngine;

namespace SSpot.Utilities.Attributes
{
    /// <summary>
    /// Draws a property with a toggle which links the values of the vector to be equal to each other.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LinkedVectorAttribute : PropertyAttribute
    {
    }
}