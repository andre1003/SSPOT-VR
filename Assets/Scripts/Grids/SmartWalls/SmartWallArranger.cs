using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using SSpot.Grids;
using UnityEngine;

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

        private bool IsInValidState() => this &&
                                         straight && cornerUpRight && tripleCornerUpRightDown && quadCorner;
        
        private void OnEnable()
        {
            if (!IsInValidState()) return;
            
            _grid = GetComponentInParent<LevelGrid>();
            if (!_grid) return;
            
            //Clear neighbors
            //ValidNeighbors.ForEach(n => n.NotifyNeighborDestroyed(this));
            _neighbors.Clear();
            
            //Get neighbors
            foreach (var obj in _grid.GetComponentsInChildren<SmartWallArranger>())
            {
                if (obj == this) continue;
                
                var dir = DirectionToNeighbor(obj);
                if (dir == Direction.None) continue;
                
                _neighbors.Add(obj);
                obj._neighbors.Add(this);
                obj.UpdateMesh();
            }
            
            UpdateMesh();
        }

        private readonly List<SmartWallArranger> _removedNeighbors = new();
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
            
            _removedNeighbors.AddRange(_neighbors.Where(n => !n || !n.gameObject.activeSelf));
            if (_removedNeighbors.Count > 0)
            {
                _removedNeighbors.ForEach(n => _neighbors.Remove(n));
                _removedNeighbors.Clear();
                UpdateMesh();
            }
        }

        private void OnValidate() => _fieldsDirty = true;

        private void UpdateMesh()
        {
            if (!IsInValidState()) return;
            
            SetMeshActive(straight, false);
            SetMeshActive(cornerUpRight, false);
            SetMeshActive(tripleCornerUpRightDown, false);
            SetMeshActive(quadCorner, false);
            
            Direction flags = _neighbors
                .Where(n => n)
                .Aggregate(Direction.None, (a, n) => a | DirectionToNeighbor(n));
            
            var (mesh, rotation) = GetMesh(flags);
            mesh.transform.forward = Quaternion.AngleAxis(rotation, Vector3.up) * Vector3.forward;
            SetMeshActive(mesh, true);
        }

        private static void SetMeshActive(GameObject mesh, bool active)
        {
            mesh.SetActive(active);
            mesh.tag = active ? "Untagged" : "EditorOnly";
        }

        private Direction DirectionToNeighbor(SmartWallArranger neighbor)
        {
            Vector2Int dir = neighbor.GridPosition - GridPosition;
            if (dir == Vector2Int.right) return Direction.Right;
            if (dir == Vector2Int.up) return Direction.Up;
            if (dir == Vector2Int.left) return Direction.Left;
            if (dir == Vector2Int.down) return Direction.Down;
            
            //if (!avoidParallelLineConnection) return Direction.None;
            
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