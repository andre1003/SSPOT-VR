using SSpot.Grids;
using UnityEditor;
using UnityEngine;

namespace SSPot.Editor
{
    [CustomEditor(typeof(LevelGrid))]
    public class LevelGridCustomEditor : UnityEditor.Editor
    {
        private const float LineThickness = 2f;
        
        private void OnSceneGUI()
        {
            if (target is not LevelGrid grid) return;

            var cellSize = grid.GetComponent<Grid>().cellSize;
            DrawGrid(grid.transform.position, grid.GridSize, new(cellSize.x, cellSize.y));
        }
        
        private static void DrawGrid(Vector3 origin, int gridSize, Vector2 cellSize)
        {
            Handles.color = Color.blue;
            
            for (int i = 0; i <= gridSize; i++)
            {
                DrawVertical(origin, i, gridSize, cellSize);
                DrawHorizontal(origin, i, gridSize, cellSize);
            }
            
            Handles.color = Color.white;
        }
        
        private static void DrawVertical(Vector3 origin, int i, int gridSize, Vector2 cellSize)
        {
            Vector3 from = origin;
            from.x += i * cellSize.x;
            Vector3 to = from;
            to.z += gridSize * cellSize.y;
            Handles.DrawLine(from, to, LineThickness);
        }

        private static void DrawHorizontal(Vector3 origin, int i, int gridSize, Vector2 cellSize)
        {
            Vector3 from = origin;
            from.z += i * cellSize.y;
            Vector3 to = from;
            to.x += gridSize * cellSize.x;
            Handles.DrawLine(from, to, LineThickness);
        }
    }
}