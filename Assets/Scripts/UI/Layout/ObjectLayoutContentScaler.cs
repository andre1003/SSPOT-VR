using UnityEngine;

namespace SSpot.UI.Layout
{
    /// <summary>
    /// Scales a mesh object to fit one or more <see cref="BaseObjectLayout"/>s.
    /// </summary>
    [ExecuteAlways]
    public class ObjectLayoutContentScaler : MonoBehaviour
    {
        [Tooltip("The scaled mesh will never be smaller than this.")]
        public float minLength;
        [Tooltip("The target layout that we want to fit inside the mesh.")]
        public BaseObjectLayout objectLayout;
        [Tooltip("The mesh to scale, ideally a child of 'transformToScale'. Will provide raw mesh size.")]
        public MeshRenderer meshToScale;
        [Tooltip("The transform to scale, ideally a parent of 'meshToScale'. Will be actually manipulated.")]
        public Transform transformToScale;
        [Tooltip("Padding applied to the start of the layout.")]
        public float paddingStart;
        [Tooltip("Padding applied to the end of the layout.")]
        public float paddingEnd;
        
        private void Update()
        {
            if (!objectLayout) return;
            if (!meshToScale) return;
            if (!transformToScale) return;

            int axis = objectLayout.Direction.IsHorizontal() ? 0 : 1;
            
            var (center, finalLength) = CalculateCenterAndLength(objectLayout, axis);
            if (finalLength < minLength)
                (center, finalLength) = CalculateOverrideCenterAndLength(objectLayout);
            
            AdjustMesh(center, finalLength, axis);
        }

        private void AdjustMesh(Vector3 center, float length, int axis)
        {
            transformToScale.position = center;
            
            // Get the current size of the mesh (with scale applied). If it is not the right size, recalculate
            float scaledMeshLen = meshToScale.bounds.size[axis];
            if (Mathf.Approximately(scaledMeshLen, length)) return;
            
            // Calculate the size of the mesh (without scale applied)
            float baseMeshLen = scaledMeshLen / transformToScale.localScale[axis];

            // Calculate the scale needed to change the mesh size to the desired size
            Vector3 finalScale = transformToScale.localScale;
            finalScale[axis] = length / baseMeshLen;
            transformToScale.localScale = finalScale;
        }
        private (Vector3 center, float length) CalculateCenterAndLength(IObjectLayout layout, int axis)
        {
            float finalLen = paddingStart + paddingEnd + layout.LengthAlongAxis;
            
            // Move the center of the mesh to the center of the layout
            Vector3 center = layout.WorldCenter;
            center[axis] += paddingStart / 2;
            center[axis] -= paddingEnd / 2;
            
            return (center, finalLen);
        }

        private (Vector3 center, float length) CalculateOverrideCenterAndLength(IObjectLayout layout)
        {
            float length = minLength;
            Vector3 center = layout.WorldOrigin + layout.Direction.GetDirection() * (length / 2);
            
            return (center, length);
        }
        
        #region EVENTS
        
        private BaseObjectLayout _oldLayout;
        private void OnValidate()
        {
            if (_oldLayout == objectLayout) return;
            
            DeregisterEvents(_oldLayout);
            RegisterEvents(objectLayout);
                        
            _oldLayout = objectLayout;
        }
        private void OnEnable() => RegisterEvents(objectLayout);
        private void OnDisable() => DeregisterEvents(objectLayout);
        
        private void RegisterEvents(BaseObjectLayout layout)
        {
            if (!layout) return;
            
            layout.OnLayoutChanged -= Update;
            layout.OnLayoutChanged += Update;
        }
        
        private void DeregisterEvents(BaseObjectLayout layout)
        {
            if (!layout) return;
            
            layout.OnLayoutChanged -= Update;
        }
        
        #endregion
    }
}