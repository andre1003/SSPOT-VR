using System;
using UnityEngine;

namespace SSpot.UI.Layout
{
    public interface IObjectLayout
    {
        event Action OnLayoutChanged;
        LayoutDirection Direction { get; }
        float LengthAlongAxis { get; }
        Vector3 WorldCenter { get; }
        Vector3 WorldOrigin { get; }
    }
    
    /// <summary>
    /// Base class for objects that control the position of their children.
    /// </summary>
    public abstract class BaseObjectLayout : MonoBehaviour, IObjectLayout
    {
        public event Action OnLayoutChanged;
        protected void InvokeLayoutChanged() => OnLayoutChanged?.Invoke();

        public abstract LayoutDirection Direction { get; }
        public abstract float LengthAlongAxis { get; }
        public abstract Vector3 WorldCenter { get; }
        public abstract Vector3 WorldOrigin { get; }
    }
}