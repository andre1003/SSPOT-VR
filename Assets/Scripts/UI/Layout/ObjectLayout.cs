using System;
using System.Collections.Generic;
using System.Linq;
using SSpot.Utilities;
using UnityEngine;

namespace SSpot.UI.Layout
{
    [ExecuteAlways]
    public class ObjectLayout : MonoBehaviour
    {
        public event Action OnLayoutChanged;
        
        public float distanceBetweenObjects = 1f;
        public LayoutDirection direction = LayoutDirection.Down;

        private readonly List<Transform> _activeChildren = new();
        public IReadOnlyList<Transform> ActiveChildren => _activeChildren;

        public float LengthAlongAxis => distanceBetweenObjects * (_activeChildren.Count - 1);
        
        public Vector3 LocalOrigin => direction.GetOffsetDirection() * LengthAlongAxis * .5f;
        
        public Vector3 Origin => transform.TransformPoint(LocalOrigin);
        
        public Vector3 LocalCenter => LocalOrigin + direction.GetDirection() * LengthAlongAxis * .5f;
        
        public Vector3 Center => transform.TransformPoint(LocalCenter);

        private bool _fieldsDirty;
        
        private void OnValidate() => _fieldsDirty = true;

        private void OnTransformChildrenChanged() => _fieldsDirty = true;

        private void Update()
        {
            bool childrenChanged = !transform.ActiveChildren().SequenceEqual(_activeChildren);
            if (!_fieldsDirty && !childrenChanged)
                return;

            if (childrenChanged)
            {
                _activeChildren.Clear();
                _activeChildren.AddRange(transform.ActiveChildren());
            }
            
            UpdatePositions();
        }
        
        private void UpdatePositions()
        {
            Vector3 pos = LocalOrigin;
            foreach (var child in _activeChildren)
            {
                child.localPosition = pos;
                pos += direction.GetDirection() * distanceBetweenObjects;
            }

            OnLayoutChanged?.Invoke();
        }
    }
}