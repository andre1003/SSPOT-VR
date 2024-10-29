using UnityEngine;

namespace SSpot.UI
{
    [ExecuteAlways]
    public class ObjectLayoutContentScaler : MonoBehaviour
    {
        public ObjectLayout objectLayout;
        public MeshRenderer meshToScale;
        public Transform transformToScale;
        public float lenPerObject = 1f;
        public float padding;

        private void Update()
        {
            if (!objectLayout) return;
            if (!meshToScale) return;
            if (!transformToScale) return;
            
            float finalLen = 2 * padding;
            for (int i = 0; i < objectLayout.transform.childCount; i++)
            {
                if (!objectLayout.transform.GetChild(i).gameObject.activeSelf) continue;
                
                finalLen += lenPerObject;
            }

            int axis = IsVertical() ? 1 : 0;
            
            float scaledMeshLen = meshToScale.bounds.size[axis];
            if (Mathf.Approximately(scaledMeshLen, finalLen)) return;
            
            Vector3 originalScale = transformToScale.localScale;
            
            // Calculate the size of the mesh before scaling
            float baseMeshLen = scaledMeshLen / originalScale[axis];
            
            // Calculate the scale needed to scale the mesh to the desired size
            Vector3 finalScale = originalScale;
            finalScale[axis] = finalLen / baseMeshLen;
            
            transformToScale.localScale = finalScale;
        }
        
        
        private bool IsVertical() => objectLayout.direction is ObjectLayout.Direction.Up 
            or ObjectLayout.Direction.Down
            or ObjectLayout.Direction.VerticalCentered;
    }
}