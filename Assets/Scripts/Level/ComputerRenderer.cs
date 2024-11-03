using SSpot.Utilities;
using UnityEngine;

namespace SSpot.Level
{
    public class ComputerRenderer : MonoBehaviour
    {
        [SerializeField] private int materialIndex = 1;
        
        [SerializeField] private MeshRenderer terminalRenderer;
        
        [SerializeField] private Material successMaterial;
        [SerializeField] private Material errorMaterial;
        
        [SerializeField] private float materialResetDelay = 5f;

        private Material _originalMaterial;
        
        private Coroutine _materialResetCoroutine;

        private void Awake() => _originalMaterial = terminalRenderer.materials[materialIndex];

        private void SetMaterialInternal(Material material, bool reset)
        {
            if (_materialResetCoroutine != null)
            {
                StopCoroutine(_materialResetCoroutine);
                _materialResetCoroutine = null;
            }
            
            var mats = terminalRenderer.materials;
            mats[materialIndex] = material;
            terminalRenderer.materials = mats;
            
            if (reset)
            {
                _materialResetCoroutine = StartCoroutine(CoroutineUtilities.WaitThen(materialResetDelay, ResetMaterial));
            }
        }

        public void SetMaterial(bool success) =>
            SetMaterialInternal(success ? successMaterial : errorMaterial, reset: true);
        
        public void ResetMaterial() => SetMaterialInternal(_originalMaterial, reset: false);
    }
}