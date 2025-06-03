using System;
using System.Collections.Generic;
using SSPot.Utilities;
using UnityEngine;

namespace SSpot.UI.Layout
{
    /// <summary>
    /// Combines multiple <see cref="ObjectLayout"/>s into one.
    /// </summary>
    [ExecuteAlways]
    public class MultiObjectLayout : BaseObjectLayout
    {
        [SerializeField] private ObjectLayout[] layouts = Array.Empty<ObjectLayout>();

        public override LayoutDirection Direction => layouts.Length == 0 ? default : layouts[0].Direction;

        private float _lengthAlongAxis;
        public override float LengthAlongAxis => _lengthAlongAxis;
        
        private Vector3 _worldCenter;
        public override Vector3 WorldCenter => _worldCenter;

        private Vector3 _worldOrigin;
        public override Vector3 WorldOrigin => _worldOrigin;
        
        private void OnEnable() => layouts.ForEach(l => l.OnLayoutChanged += UpdatePositions);
        private void OnDisable() => layouts.ForEach(l => l.OnLayoutChanged -= UpdatePositions);
        
        private void UpdatePositions()
        {
            MakeAllSameDirection();
            UpdateValues();
            InvokeLayoutChanged();
        }
        
        private void MakeAllSameDirection()
        {
            for (int i = 1; i < layouts.Length; i++)
            {
                if (!layouts[i]) continue;
                layouts[i].direction = Direction;
                layouts[i].SetDirty();
            }
        }

        private void UpdateValues()
        {
            if (layouts.Length == 0)
            {
                _worldCenter = Vector3.zero;
                _lengthAlongAxis = 0;
                return;
            }
            
            _lengthAlongAxis = CalculateLength(layouts, Direction.GetDirection());
            _worldCenter = CalculateCenter(layouts);
            _worldOrigin = CalculateWorldOrigin(layouts);
        }
        
        private static float CalculateLength(IEnumerable<ObjectLayout> layouts, Vector3 direction)
        {
            float minProjection = float.MaxValue;
            float maxProjection = float.MinValue;
            foreach (var layout in layouts)
            {
                if (!layout) continue;
                float projFrom = Vector3.Dot(layout.WorldOrigin, direction);
                float projTo = Vector3.Dot(layout.WorldEnd, direction);
                minProjection = Mathf.Min(minProjection, projFrom, projTo);
                maxProjection = Mathf.Max(maxProjection, projFrom, projTo);
            }
            return maxProjection - minProjection;
        }
        
        private static Vector3 CalculateCenter(IEnumerable<ObjectLayout> layouts)
        {
            Vector3 worldMin = Vector3.positiveInfinity;
            Vector3 worldMax = Vector3.negativeInfinity;
            foreach (var layout in layouts)
            {
                if (!layout) continue;
                worldMin = Vector3.Min(worldMin, layout.WorldOrigin);
                worldMax = Vector3.Max(worldMax, layout.WorldEnd);
            }

            return (worldMin + worldMax) * .5f;
        }
        
        private static Vector3 CalculateWorldOrigin(IEnumerable<ObjectLayout> layouts)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var layout in layouts)
            {
                if (!layout) continue;
                sum += layout.WorldOrigin;
                count++;
            }
            return count > 0 ? sum / count : Vector3.zero;
        }
    }
}