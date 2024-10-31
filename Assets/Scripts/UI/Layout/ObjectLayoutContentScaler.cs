using UnityEngine;

namespace SSpot.UI.Layout
{
    [ExecuteAlways]
    public class ObjectLayoutContentScaler : MonoBehaviour
    {
        public ObjectLayout objectLayout;
        public MeshRenderer meshToScale;
        public Transform transformToScale;
        public float paddingStart;
        public float paddingEnd;

        private void OnEnable() => RegisterEvents(objectLayout);

        private void OnDisable() => DeregisterEvents(_oldLayout);
        
        private void Update()
        {
            if (!objectLayout) return;
            if (!meshToScale) return;
            if (!transformToScale) return;
            
            float finalLen = paddingStart + paddingEnd + objectLayout.LengthAlongAxis;
            
            // Move the center of the mesh to the center of the layout
            int axis = objectLayout.direction.IsHorizontal() ? 0 : 1;
            Vector3 center = objectLayout.Center;
            center[axis] += paddingStart / 2;
            center[axis] -= paddingEnd / 2;
            transformToScale.position = center;
            
            
            // Get the current size of the mesh (with scale applied). If it is not the right size, recalculate
            float scaledMeshLen = meshToScale.bounds.size[axis];
            if (Mathf.Approximately(scaledMeshLen, finalLen)) return;
            
            // Calculate the size of the mesh before scaling
            float baseMeshLen = scaledMeshLen / transformToScale.localScale[axis];

            // Calculate the scale needed to change the mesh size to the desired size
            Vector3 finalScale = transformToScale.localScale;
            finalScale[axis] = finalLen / baseMeshLen;
            transformToScale.localScale = finalScale;
        }
        
        private void RegisterEvents(ObjectLayout layout)
        {
            if (!layout) return;
            
            layout.OnLayoutChanged -= Update;
            layout.OnLayoutChanged += Update;
        }
        
        private void DeregisterEvents(ObjectLayout layout)
        {
            if (!layout) return;
            
            layout.OnLayoutChanged -= Update;
        }

        private ObjectLayout _oldLayout;
        private void OnValidate()
        {
            if (_oldLayout == objectLayout) return;
            
            DeregisterEvents(_oldLayout);
            RegisterEvents(objectLayout);
                        
            _oldLayout = objectLayout;
        } 
    }
}