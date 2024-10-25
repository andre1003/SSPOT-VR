using System.Linq;
using SSpot.Grids;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace SSPot.Editor
{
    [EditorTool("Grid Placer")]
    public class GridPlacerTool : EditorTool
    {
        private GridObject _selectedPrefab;
        private Vector3Int _lastCell;
        
        public override void OnToolGUI(EditorWindow window)
        {
            if (window is not SceneView view) return;
            var grid = FindObjectOfType<LevelGrid>();
            if (grid == null) return;
            var internalGrid = grid.GetComponent<Grid>();
            if (internalGrid == null) return;
            var cellSize = new Vector2(internalGrid.cellSize.x, internalGrid.cellSize.y);
            
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (Selection.activeObject is GameObject obj && obj.TryGetComponent(out GridObject selected))
            {
                _selectedPrefab = selected;
            }
            
            DrawGui();
            
            var e = Event.current;
            
            if (!TryGetCell(internalGrid, out var cell) || !grid.InGrid(new Vector2Int(cell.x, cell.y)))
                return;
            
            DrawSquare(internalGrid.CellToWorld(cell), cellSize);
            if (cell != _lastCell)
            {
                view.Repaint();
                _lastCell = cell;
            }

            HandleInput(cell, internalGrid, e);
        }

        private void HandleInput(Vector3Int cell, Grid grid, Event e)
        {
            if (e.type is not EventType.MouseDown and not EventType.MouseUp and not EventType.MouseDrag) return;
            
            e.Use();
            
            var existingObject = grid
                .GetComponentsInChildren<GridObject>()
                .FirstOrDefault(obj => grid.WorldToCell(obj.gameObject.transform.position) == cell);
            switch (e.button)
            {
                case 0 when existingObject == null && _selectedPrefab != null:
                {
                    var instance = (GridObject)PrefabUtility.InstantiatePrefab(_selectedPrefab, grid.transform);
                    instance.name = $"({cell.x}, {cell.y}) {instance.name}";
                    instance.transform.position = grid.GetCellCenterWorld(cell);
                
                    Undo.RegisterCreatedObjectUndo(instance.gameObject, $"Create {instance.name}");
                    break;
                }
                case 1 when existingObject != null:
                    Undo.DestroyObjectImmediate(existingObject.gameObject);
                    break;
            }
        }

        private void DrawGui()
        {
            Handles.BeginGUI();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new GUILayout.VerticalScope())
                {
                    GUILayout.FlexibleSpace();
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Label("Prefab to place:");
                        var value =
                            EditorGUILayout.ObjectField(
                                    _selectedPrefab ? _selectedPrefab.gameObject : null, 
                                    typeof(GameObject),
                                    allowSceneObjects: false)
                                as GameObject;
                        if (value && value.TryGetComponent(out GridObject gridObject) && gridObject != _selectedPrefab)
                        {
                            _selectedPrefab = gridObject;
                            Selection.activeObject = null;
                        }
                    }
                }
            }
            Handles.EndGUI();
        }

        private static bool TryGetCell(Grid grid, out Vector3Int cell)
        {
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var gridPlane = new Plane(grid.transform.up, grid.transform.position);
            if (!gridPlane.Raycast(ray, out float enter))
            {
                cell = default;
                return false;
            }
            
            var gridPoint = ray.GetPoint(enter);
            cell = grid.WorldToCell(gridPoint);
            return true;
        }
        
        private static void DrawSquare(Vector3 worldPos, Vector2 cellSize)
        {
            const float lineThickness = 2f;
            
            Vector3 a = worldPos;
            Vector3 b = a;
            b.x += cellSize.x;
            Vector3 c = a;
            c.x += cellSize.x;
            c.z += cellSize.y;
            Vector3 d = a;
            d.z += cellSize.y;
            
            Handles.color = Color.yellow;
            
            Handles.DrawLine(a, b, lineThickness);
            Handles.DrawLine(b, c, lineThickness);
            Handles.DrawLine(c, d, lineThickness);
            Handles.DrawLine(d, a, lineThickness);
            
            Handles.color = Color.white;
        }
    }
}