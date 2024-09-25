using Photon.Pun;
using UnityEngine;

namespace SSpot.Grids
{
    [RequireComponent(typeof(Grid))]
    public class LevelGrid : MonoBehaviourPun
    {
        [SerializeField] private int gridSize = 10;

        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private bool prefabOnlyInWalkable = true;

        public int GridSize => gridSize;

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

        public bool InGrid(int coord) => coord >= 0 && coord < gridSize;

        public bool InGrid(Vector2Int cell) => InGrid(cell.x) && InGrid(cell.y);

        public bool IsWorldPosInGrid(Vector3 worldPos) => InGrid(WorldToCell(worldPos));
        
        
        private void Awake()
        {
            InternalGrid = GetComponent<Grid>();
            
            _nodes = new Node[gridSize][];
            for (int x = 0; x < gridSize; x++)
            {
                _nodes[x] = new Node[gridSize];
                for (int y = 0; y < gridSize; y++)
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
        
        #if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            var cellSize = GetComponent<Grid>().cellSize;
            
            Gizmos.color = Color.blue;
            
            for (int i = 0; i <= gridSize; i++)
            {
                DrawVertical(transform.position, i, gridSize, cellSize);
                DrawHorizontal(transform.position, i, gridSize, cellSize);
            }

            Gizmos.color = Color.red;
            
            var internalGrid = GetComponent<Grid>();
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
        
        #endif
    }
}