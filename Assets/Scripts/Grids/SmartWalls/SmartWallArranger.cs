using System;
using System.Collections.Generic;
using System.Linq;
using SSpot.Grids;
using SSpot.Utilities;
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
            Down = 1<<4
        }

        //Consider using meshes instead, with optimization to merge meshes
        [SerializeField] private GameObject straight, cornerUpRight, tripleCornerUpRightDown, quadCorner;

        private Direction _currentFlags = Direction.None;
        
        private LevelGrid _grid;

        private Vector2Int GridPosition => _grid && _grid.TryGetComponent(out Grid internalGrid)
            ? (Vector2Int) internalGrid.WorldToCell(transform.position)
            : Vector2Int.zero;

        private readonly List<SmartWallArranger> _neighbors = new();
        private IEnumerable<SmartWallArranger> ValidNeighbors => _neighbors.Where(n => n);

        private void OnEnable()
        {
            if (!this) return;
            
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
                Debug.Log($"From {this} to {obj} is {dir}");
                if (dir == Direction.None) continue;
                
                _neighbors.Add(obj);
                obj.NotifyNeighborCreated(this);
            }
            
            UpdateMesh();
        }

        private void Update()
        {
            if (transform.hasChanged)
            {

                transform.hasChanged = false;
                OnEnable();
            }
        }
        
        private void OnDisable()
        {
            ValidNeighbors.ForEach(n => n.NotifyNeighborDestroyed(this));
        }

        private void UpdateMesh()
        {
            if (!this) return;
            
            Direction flags = ValidNeighbors.Aggregate(Direction.None, (a, n) => a | DirectionToNeighbor(n));
            
            var (oldMesh, _) = GetMesh(_currentFlags);
            if (oldMesh) oldMesh.SetActive(false);
            
            var (mesh, rotation) = GetMesh(flags);
            mesh.SetActive(true);
            mesh.transform.forward = Quaternion.AngleAxis(rotation, Vector3.up) * Vector3.forward;

            _currentFlags = flags;
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
            return Direction.None;
        }
        
        private (GameObject mesh, float rotation) GetMesh(Direction flags)
        {
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