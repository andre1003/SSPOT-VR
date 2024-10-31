using NaughtyAttributes;
using Photon.Pun;
using SSpot.Ambient.ComputerCode;
using UnityEngine;

namespace SSpot.Ambient.Cubes
{
    [ExecuteAlways]
    public class CellLayout : MonoBehaviourPun
    {
        [SerializeField] private CodingCell cellPrefab;
        [SerializeField] private Transform cellHolder;
        [SerializeField, MinValue(0)] private int cellCount = 7;
        
#if UNITY_EDITOR
        
        private bool _dirty;
        
        private void OnValidate() => _dirty = true;

        private void Update()
        {
            if (!_dirty) return;
            _dirty = false;
            
            if (Application.isPlaying) return;
            if (IsInPrefabStage()) return;
            if (!cellHolder) return;
         
            if (!cellPrefab || cellCount == 0)
            {
                Clear();
                return;
            }
            
            ClearExcess();
            
            // Ensure the prefab and name is correct, and instantiate missing cells
            for (int i = 0; i < cellCount; i++)
            {
                GameObject child = null;
                if (i < cellHolder.childCount)
                {
                    child = cellHolder.GetChild(i).gameObject;
                    if (!IsRightPrefab(child))
                        DestroyImmediate(child);
                }

                if (!child)
                {
                    child = InstantiateCell();
                    child.transform.SetSiblingIndex(i);
                }
                
                child.name = cellPrefab.name + i;
            }
        }
        
        private void Clear()
        {
            for (int i = cellHolder.childCount - 1; i >= 0; i--)
                DestroyImmediate(cellHolder.GetChild(i).gameObject);
        }
        
        private void ClearExcess()
        {
            for (int i = cellHolder.childCount - 1; i >= cellCount; i--)
                DestroyImmediate(cellHolder.GetChild(i).gameObject);
        }

        private GameObject InstantiateCell()
        {
            var result = (CodingCell)UnityEditor.PrefabUtility.InstantiatePrefab(cellPrefab, cellHolder);
            return result.gameObject;
        }

        private static bool IsInPrefabStage()
        {
            return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
        }

        private bool IsRightPrefab(GameObject cell)
        {
            if (!cell.GetComponent<CodingCell>()) return false;
            return UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(cell) == cellPrefab.gameObject;
        }
#endif
    }
}