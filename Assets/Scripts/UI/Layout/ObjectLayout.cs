using System.Collections.Generic;
using System.Linq;
using SSPot.Utilities;
using UnityEngine;

namespace SSpot.UI.Layout
{
    /// <summary>
    /// Arranges children along an axis.
    /// </summary>
    [ExecuteAlways]
    public class ObjectLayout : BaseObjectLayout
    {
        public float distanceBetweenObjects = 1f;
        
        [Tooltip("If true, will position the first child at the end of the axis. For example," +
                 "with direction=Up, will build the axis from transform.position + up * distanceBetweenObjects," +
                 "but will have the first child in the hierarchy all the way at the top.")]
        public bool reverseOrder = false;
        
        [SerializeField]
        public LayoutDirection direction;
        public override LayoutDirection Direction => direction; 
     
        private readonly List<Transform> _activeChildren = new();
        public override float LengthAlongAxis => distanceBetweenObjects * Mathf.Max(_activeChildren.Count - 1, 0);

        public Vector3 LocalOrigin => direction.IsCentered()
            ? -direction.GetDirection() * LengthAlongAxis * .5f
            : Vector3.zero;
        
        public override Vector3 WorldOrigin => transform.TransformPoint(LocalOrigin);
        
        public Vector3 LocalEnd => LocalOrigin + direction.GetDirection() * LengthAlongAxis;
        
        public Vector3 WorldEnd => transform.TransformPoint(LocalEnd);
        
        public Vector3 LocalCenter => LocalOrigin + direction.GetDirection() * LengthAlongAxis * .5f;
        
        public override Vector3 WorldCenter => transform.TransformPoint(LocalCenter);
        
        private void Update()
        {
            bool childrenChanged = !transform.ActiveChildren().SequenceEqual(_activeChildren);
            if (childrenChanged)
            {
                _activeChildren.Clear();
                _activeChildren.AddRange(transform.ActiveChildren());
            }

            if (childrenChanged || _fieldsDirty)
                UpdatePositions();
            
            _fieldsDirty = false;
        }
        
        private void UpdatePositions()
        {
            IEnumerable<Transform> children = _activeChildren;
            if (reverseOrder) children = children.Reverse();
            
            Vector3 pos = LocalOrigin;
            foreach (var child in children)
            {
                child.localPosition = pos;
                pos += direction.GetDirection() * distanceBetweenObjects;
            }

            InvokeLayoutChanged();
        }
        
        private bool _fieldsDirty;
        private void OnValidate() => _fieldsDirty = true;
        private void OnTransformChildrenChanged() => _fieldsDirty = true;
        public void SetDirty() => _fieldsDirty = true;
    }
}