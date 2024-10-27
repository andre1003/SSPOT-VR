using System.Diagnostics.CodeAnalysis;
using NaughtyAttributes;
using Photon.Pun;
using SSpot.Utilities;
using UnityEngine;

namespace SSpot.Grids
{
    [RequireComponent(typeof(Grid))]
    public class LevelGrid : MonoBehaviourPun
    {
        [LinkedVector] [MinValue(1)]
        [SerializeField] private Vector2Int gridSize = new(10, 10);

        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private bool prefabOnlyInWalkable = true;

        public Vector2Int GridSize => gridSize;

        public Grid InternalGrid { get; private set; }

        private Node[][] _nodes;

        public Node this[int x, int y] => _nodes[x][y];
        public Node this[Vector2Int xy] => this[xy.x, xy.y];
        public Node this[Vector3Int xyz] => this[xyz.x, xyz.y];

        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            var res = InternalGrid.WorldToCell(worldPos);
            return new(res.x, res.y);
        }

        public Vector3 CellToWorld(Vector2Int cell) => InternalGrid.CellToWorld(new(cell.x, cell.y, 0));

        public Vector3 GetCellCenterWorld(Vector2Int cell) => InternalGrid.GetCellCenterWorld(new(cell.x, cell.y, 0));

        public bool InGrid(Vector2Int cell) => cell.x >= 0 && cell.x < gridSize.x &&
                                               cell.y >= 0 && cell.y < gridSize.y;

        public bool IsWorldPosInGrid(Vector3 worldPos) => InGrid(WorldToCell(worldPos));

        public bool TryGetNode(Vector2Int cell, [NotNullWhen(true)] out Node node)
        {
            if (InGrid(cell))
            {
                node = this[cell];
                return true;
            }

            node = null;
            return false;
        }
        
        private void Awake()
        {
            InternalGrid = GetComponent<Grid>();
            
            _nodes = new Node[gridSize.x][];
            for (int x = 0; x < gridSize.x; x++)
            {
                _nodes[x] = new Node[gridSize.y];
                for (int y = 0; y < gridSize.y; y++)
                {
                    _nodes[x][y] = new(new(x, y));
                    if (nodePrefab != null)
                    {
                        var obj = Instantiate(nodePrefab, transform);
                        obj.transform.position = GetCellCenterWorld(new(x, y));
                        _nodes[x][y].NodeObject = obj;
                    }
                }
            }
            
            foreach (var obj in GetComponentsInChildren<ILevelGridObject>())
            {
                obj.Grid = this;
                var cell = WorldToCell(obj.gameObject.transform.position);
                obj.GridPosition = cell;
                this[cell].Objects.Add(obj);
                
                if (prefabOnlyInWalkable && !this[cell].CanWalk)
                    Destroy(this[cell].NodeObject);
            }
        }

        public void ChangeNode(ILevelGridObject obj, Vector2Int target)
        {
            this[obj.GridPosition].Objects.Remove(obj);
            this[target].Objects.Add(obj);
            obj.GridPosition = target;
            obj.gameObject.transform.position = GetCellCenterWorld(target);
            
            if (obj.TriggerOnSteppedOn)
                foreach (var gridObject in this[target].Objects)
                    gridObject.OnSteppedOn();
        }

        private void OnValidate()
        {
            GetComponent<Grid>().cellSwizzle = GridLayout.CellSwizzle.XZY;
        }

        #region GIZMOS_DRAWING
        
        private void OnDrawGizmos()
        {
            var internalGrid = GetComponent<Grid>();
            var cellSize = new Vector2(internalGrid.cellSize.x, internalGrid.cellSize.y);
            
            Gizmos.color = Color.blue;
            
            for (int i = 0; i <= Mathf.Max(gridSize.x, gridSize.y); i++)
            {
                if (i <= gridSize.x)
                    DrawVertical(transform.position, i, gridSize.y, cellSize);
                
                if (i <= gridSize.y)
                    DrawHorizontal(transform.position, i, gridSize.x, cellSize);
            }

            Gizmos.color = Color.red;
            
            foreach (var obj in GetComponentsInChildren<ILevelGridObject>())
            {
                if (obj.CanWalkThrough) continue;
                
                var cell = internalGrid.WorldToCell(obj.gameObject.transform.position);
                var worldPos = internalGrid.CellToWorld(cell);
                DrawSquare(worldPos, cellSize);
            }
            
            Gizmos.color = Color.white;
        }

        private static void DrawVertical(Vector3 origin, int i, int gridSize, Vector2 cellSize)
        {
            Vector3 from = origin;
            from.x += i * cellSize.x;
            Vector3 to = from;
            to.z += gridSize * cellSize.y;
            Gizmos.DrawLine(from, to);
        }

        private static void DrawHorizontal(Vector3 origin, int i, int gridSize, Vector2 cellSize)
        {
            Vector3 from = origin;
            from.z += i * cellSize.y;
            Vector3 to = from;
            to.x += gridSize * cellSize.x;
            Gizmos.DrawLine(from, to);
        }
        
        private static void DrawSquare(Vector3 worldPos, Vector2 cellSize)
        {
            Vector3 a = worldPos;
            Vector3 b = a;
            b.x += cellSize.x;
            Vector3 c = a;
            c.x += cellSize.x;
            c.z += cellSize.y;
            Vector3 d = a;
            d.z += cellSize.y;
            
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
        
        #endregion
    }
}