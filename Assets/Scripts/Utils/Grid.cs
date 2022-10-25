using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;


namespace BubbleShooter
{
    public class Grid : MonoBehaviour
    {
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public int CellsX { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public int CellsY { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public float XOffset { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public float YOffset { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public Transform Parent { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public Transform StartPoisiton { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public Mesh Mesh { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public float FindDistance { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public bool DrawDebugSphere { get; private set; }
        [field: Foldout("Grid")] [field: SerializeField] public Dictionary<Vector2Int, CellView> GridDictionary { get; private set; }

        private float _currentYOffset;
        
        [Button("Create Grid")]
        private void CreateGrid()
        {
            if (!Parent) Parent = new GameObject("Grid").transform;
            if(GridDictionary != null) ClearGrid();

            GridDictionary = new Dictionary<Vector2Int, CellView>();
            var intOffset = 0;

            for (int x = 0; x < CellsX; x++)
            {
                var intYOffset = x;
                for (int y = 0; y < CellsY; y++)
                {
                    if (IsCellInSecondLine(x, y, intOffset))
                    {
                        _currentYOffset += YOffset;
                        continue;
                    }

                    var go = new GameObject($"Cell {x}:{y}");
                    var xOffset = y % 2 == 0 ? 0 : XOffset;
                    _currentYOffset += x == intYOffset ? 0 : YOffset;
                    go.transform.position = new Vector3(x + xOffset, y-_currentYOffset, 0) + StartPoisiton.position;
                    go.transform.SetParent(Parent);
                    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.SetParent(go.transform);
                    sphere.transform.localPosition = Vector3.zero;
                    GridDictionary.Add(new Vector2Int(x,y), go.AddComponent<CellView>());
                    intYOffset++;
                }

                _currentYOffset = 0;
                intOffset++;
            }

           
            FindNeighbours(GridDictionary);
            
        }

        private bool IsCellInSecondLine(int x, int y, int intOffset)
        {
            if (x % 2 != 0 && y % 2 != 0)
            {
                if (intOffset % (CellsX - 1) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void FindNeighbours(Dictionary<Vector2Int, CellView> gridDictionary)
        {
            foreach (var cellView in gridDictionary.Values)
            {
                cellView.NeighboursList = new List<CellView>();
                var cells = gridDictionary.Values
                    .Where(x => Vector3.Distance(cellView.transform.position, x.transform.position) < FindDistance)
                    .Where(x => x != cellView);
                
                cellView.NeighboursList.AddRange(cells);
            }
        }

        [Button("Clear Grid")]
        private void ClearGrid()
        {
            if(GridDictionary != null)
            {
                foreach (var gameObject in GridDictionary)
                {
                    DestroyImmediate(gameObject.Value.gameObject);
                }
                
                GridDictionary.Clear();
            }
        }
        
        private void OnDrawGizmos()
        {
            if(GridDictionary != null) 
            {
                Gizmos.color = Color.cyan;
                foreach (var cell in GridDictionary)
                {
                    Gizmos.DrawWireCube(cell.Value.transform.position, cell.Value.transform.localScale);
                    if (DrawDebugSphere)
                    {
                        Gizmos.DrawSphere(cell.Value.transform.position, cell.Value.transform.localScale.x /2);
                    }
                }
            }
        }
    }
}
