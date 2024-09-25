using Photon.Pun;
using UnityEngine;

namespace SSpot.Grids
{
    [RequireComponent(typeof(Grid))]
    public class LevelGrid : MonoBehaviourPun
    {
        [SerializeField] private int gridSize = 10;

        [SerializeField] private GameObject nodePrefab;

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

        public bool InGrid(Vector3 worldPos) => InGrid(WorldToCell(worldPos));
        
        
        private void Awake()
        {
            InternalGrid = GetComponent<Grid>();
            
            _nodes = new Node[gridSize][];
            for (int i = 0; i < gridSize; i++)
            {
                _nodes[i] = new Node[gridSize];
                for (int j = 0; j < gridSize; j++)
                {
                    _nodes[i][j] = new();
                    if (nodePrefab != null)
                    {
                        var obj = Instantiate(nodePrefab, transform);
                        obj.transform.position = GetCellCenterWorld(new(i, j));
                    }
                }
            }
            
            foreach (var obj in GetComponentsInChildren<ILevelGridObject>())
            {
                obj.Grid = this;
                var cell = InternalGrid.WorldToCell(transform.position);
                obj.GridPosition = new(cell.x, cell.y);
                this[obj.GridPosition].Objects.Add(obj);
            }
        }

        public void ChangeNode(ILevelGridObject obj, Vector2Int target)
        {
            this[obj.GridPosition].Objects.Remove(obj);
            this[target].Objects.Add(obj);
            obj.GridPosition = target;
            obj.GameObject.transform.position = GetCellCenterWorld(target);
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
        #endif
    }
}