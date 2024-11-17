using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using SSpot.Grids;
using SSPot.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSPot.Grids.SmartWalls
{
    [ExecuteAlways]
    public class SmartWallArranger : MonoBehaviour
    {
        [Flags]
        private enum Direction
        {
            None = 0,
            Right = 1<<0,
            Up = 1<<2,
            Left = 1<<3,
            Down = 1<<4,
            
            UpRight = 1<<5,
            UpLeft = 1<<6,
            DownRight = 1<<7,
            DownLeft = 1<<8
        }

        [Tooltip("If true, parallel lines of walls won't connect to each other.")]
        [SerializeField] private bool avoidParallelLineConnection = true;
        
        //Consider using meshes instead, with optimization to merge meshes
        [SerializeField, BoxGroup("Meshes")] 
        private GameObject straight, cornerUpRight, tripleCornerUpRightDown, quadCorner;

        private LevelGrid _grid;

        private Vector2Int GridPosition => _grid && _grid.TryGetComponent(out Grid internalGrid)
            ? (Vector2Int) internalGrid.WorldToCell(transform.position)
            : Vector2Int.zero;

        private readonly List<SmartWallArranger> _neighbors = new();
        private IEnumerable<SmartWallArranger> ValidNeighbors => _neighbors.Where(n => n);

        private bool IsInValidState() => this &&
                                         straight && cornerUpRight && tripleCornerUpRightDown && quadCorner;

        private bool _isUnloading;
        private void OnSceneUnloaded(Scene scene) => _isUnloading = scene == gameObject.scene;
        
        private void OnEnable()
        {
            if (!IsInValidState()) return;
            
            #if UNITY_EDITOR
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            #endif
            
            _grid = GetComponentInParent<LevelGrid>();
            if (!_grid) return;
            
            //Clear neighbors
            ValidNeighbors.ForEach(n => n.NotifyNeighborDestroyed(this));
            _neighbors.Clear();
            
            //Get neighbors
            foreach (var obj in _grid.GetComponentsInChildren<SmartWallArranger>())
            {
                if (obj == this) continue;
                
                var dir = DirectionToNeighbor(obj);
                if (dir == Direction.None) continue;
                
                _neighbors.Add(obj);
                obj.NotifyNeighborCreated(this);
            }
            
            UpdateMesh();
        }

        private bool _fieldsDirty;
        private void Update()
        {
            if (!IsInValidState()) return;
            
            if (transform.hasChanged || _fieldsDirty)
            {
                transform.hasChanged = false;
                _fieldsDirty = false;
                OnEnable();
            }
        }

        private void OnValidate() => _fieldsDirty = true;
      
        private void OnDisable()
        {
            #if UNITY_EDITOR
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            #endif
            
            // Prevent scene unloading from ruining wall arrangement
            if (_isUnloading)
            {
                _isUnloading = false;
                return;
            }
            
            if (!IsInValidState()) return;
            
            ValidNeighbors.ForEach(n => n.NotifyNeighborDestroyed(this));
        }

        private void UpdateMesh()
        {
            if (!IsInValidState()) return;
            
            SetMeshActive(straight, false);
            SetMeshActive(cornerUpRight, false);
            SetMeshActive(tripleCornerUpRightDown, false);
            SetMeshActive(quadCorner, false);
            
            Direction flags = ValidNeighbors.Aggregate(Direction.None, (a, n) => a | DirectionToNeighbor(n));
            
            var (mesh, rotation) = GetMesh(flags);
            mesh.transform.forward = Quaternion.AngleAxis(rotation, Vector3.up) * Vector3.forward;
            SetMeshActive(mesh, true);
        }

        private static void SetMeshActive(GameObject mesh, bool active)
        {
            mesh.SetActive(active);
            mesh.tag = active ? "Untagged" : "EditorOnly";
        }

        private void NotifyNeighborCreated(SmartWallArranger neighbor)
        {
            _neighbors.Add(neighbor);
            UpdateMesh();
        }
        
        private void NotifyNeighborDestroyed(SmartWallArranger neighbor)
        {
            _neighbors.Remove(neighbor);
            UpdateMesh();
        }

        private Direction DirectionToNeighbor(SmartWallArranger neighbor)
        {
            Vector2Int dir = neighbor.GridPosition - GridPosition;
            if (dir == Vector2Int.right) return Direction.Right;
            if (dir == Vector2Int.up) return Direction.Up;
            if (dir == Vector2Int.left) return Direction.Left;
            if (dir == Vector2Int.down) return Direction.Down;
            
            if (!avoidParallelLineConnection) return Direction.None;
            
            if (dir == Vector2Int.right + Vector2Int.up) return Direction.UpRight;
            if (dir == Vector2Int.right + Vector2Int.down) return Direction.DownRight;
            if (dir == Vector2Int.left + Vector2Int.up) return Direction.UpLeft;
            if (dir == Vector2Int.left + Vector2Int.down) return Direction.DownLeft;
            
            return Direction.None;
        }
        
        private (GameObject mesh, float rotation) GetMesh(Direction flags)
        {
           if (avoidParallelLineConnection)
           {
               // Remove parallel line neighbors
               if (flags.HasFlag(Direction.Down | Direction.Up | Direction.Right | Direction.UpRight | Direction.DownRight))
                   flags &= ~Direction.Right;
               else if (flags.HasFlag(Direction.Down | Direction.Up | Direction.Left | Direction.UpLeft | Direction.DownLeft))
                   flags &= ~Direction.Left;
               else if (flags.HasFlag(Direction.Right | Direction.Left | Direction.Up | Direction.UpRight | Direction.UpLeft))
                   flags &= ~Direction.Up;
               else if (flags.HasFlag(Direction.Right | Direction.Left | Direction.Down | Direction.DownRight | Direction.DownLeft))
                   flags &= ~Direction.Down;
           }
           
           // Don't consider corners when actually choosing the mesh
           flags &= ~(Direction.UpRight | Direction.DownRight | Direction.UpLeft | Direction.DownLeft);
            
           return flags switch
           {
               Direction.Up or Direction.Down or Direction.Up | Direction.Down => (straight, 0),
               Direction.Right or Direction.Left or Direction.Right | Direction.Left => (straight, 90),
                
               Direction.Up | Direction.Right => (cornerUpRight, 0),
               Direction.Right | Direction.Down => (cornerUpRight, 90),
               Direction.Down | Direction.Left => (cornerUpRight, 180),
               Direction.Left | Direction.Up => (cornerUpRight, 270),
                
               Direction.Up | Direction.Right | Direction.Down => (tripleCornerUpRightDown, 0),
               Direction.Right | Direction.Down | Direction.Left => (tripleCornerUpRightDown, 90),
               Direction.Down | Direction.Left | Direction.Up => (tripleCornerUpRightDown, 180),
               Direction.Left | Direction.Up | Direction.Right => (tripleCornerUpRightDown, 270),
                
               Direction.Up | Direction.Right | Direction.Down | Direction.Left => (quadCorner, 0),
                
               _ => (straight, 0)
           };
        }
    }
}