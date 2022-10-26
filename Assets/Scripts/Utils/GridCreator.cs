using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UniRx;
using UnityEngine;


namespace BubbleShooter
{
    public class GridCreator : MonoBehaviour
    {
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public int CellsX { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public int CellsY { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public float XOffset { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public float YOffset { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public Vector3 LocalSphereScale { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public Transform Parent { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public Transform StartPoisiton { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public float FindDistance { get; private set; }
        [field: BoxGroup("Grid Settings")] [field: SerializeField] public bool DrawDebugSphere { get; private set; }
        [field: Range(0,100)]
        [field: BoxGroup("Random Settings")] [field: SerializeField] public int ChanceToColorizeInSameColor { get; private set; }
        [field: Foldout("Materials Settings")] [field: SerializeField] public List<Material> Materials { get; private set; }
        [field: Foldout("Materials Settings")] [field: SerializeField] public Material SelectionMaterial { get; private set; }
        [field: Foldout("Grid")] [field: SerializeField] public List<CellView> CellsList { get; private set; }

        private float _currentYOffset;
        private bool[] _chance = new bool[100];
        private int _previousColorValue;

        private void Awake()
        {
            var grid = FindObjectsOfType<GridCreator>(true);
            if (grid.Length > 1)
            {
                Destroy(gameObject);
            }

            if (CellsList == null)
            {
                print("NULL");
            }
            
            DontDestroyOnLoad(gameObject);
            InitChanceArray();
            
            this.ObserveEveryValueChanged(x => x.ChanceToColorizeInSameColor)
                .Subscribe(_ => InitChanceArray())
                .AddTo(this);
        }

        private void InitChanceArray()
        {
            for (int i = 0; i < _chance.Length; i++)
            {
                if (i < ChanceToColorizeInSameColor)
                {
                    _chance[i] = true;
                }
                else
                {
                    _chance[i] = false;
                }
            }
        }

        [Button("Create Grid")]
        private void CreateGrid()
        {
            if (!Parent) Parent = new GameObject("Grid").transform;
            if(CellsList != null) ClearGrid();

            CellsList = new List<CellView>();
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
                    go.transform.localScale = Vector3.one;
                    var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.SetParent(go.transform);
                    sphere.transform.localPosition = Vector3.zero;
                    sphere.transform.localScale = LocalSphereScale;
                    var renderer = sphere.GetComponent<Renderer>();
                    var collider = sphere.GetComponent<Collider>();
                    var cellVeiw = go.AddComponent<CellView>();
                    cellVeiw.Init(renderer, collider, Materials, sphere.transform.localScale);
                    CellsList.Add(cellVeiw);
                    intYOffset++;
                }
                _currentYOffset = 0;
                intOffset++;
            }
            
            FindNeighbours(CellsList);
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

        private void FindNeighbours(List<CellView> cellsList)
        {
            foreach (var cellView in cellsList)
            {
                cellView.NeighboursList = new List<CellView>();
                var cells = cellsList
                    .Where(x => Vector3.Distance(cellView.transform.position, x.transform.position) < FindDistance)
                    .Where(x => x != cellView);
                
                cellView.NeighboursList.AddRange(cells);
            }
        }

        [Button("Clear Grid")]
        private void ClearGrid()
        {
            if(CellsList != null)
            {
                foreach (var gameObject in CellsList)
                {
                    DestroyImmediate(gameObject.gameObject);
                }
                
                CellsList.Clear();
            }
            else
            {
                var cells = FindObjectsOfType<CellView>();
                foreach (var cell in cells)
                {
                    DestroyImmediate(cell.gameObject);
                }
            }
        }

        public void ResetAllColors()
        {
            if (CellsList == null)
            {
                print($"{CellsList} is null");
                return;
            } 
            
            foreach (var cellView in CellsList)
            {
                cellView.ColorTypes = ColorTypes.None;
            }
        }
        
        public void RandomizeColors()
        {
            if (CellsList == null)
            {
                print($"{CellsList} is null");
                return;
            }

            if (ChanceToColorizeInSameColor == _chance.Length)
            {
                _previousColorValue++;
                if (_previousColorValue >= 5)
                {
                    _previousColorValue = 0;
                }
            }
            
            foreach (var cellView in CellsList)
            {
                var value = _chance[Random.Range(0, 99)];
                var colorInt = value ? _previousColorValue : Random.Range(0, 5);
                cellView.ColorTypes = (ColorTypes) colorInt;
                _previousColorValue = colorInt;
            }
        }

        public void OnPlayButton()
        {
            if (CellsList == null)
            {
                print($"{CellsList} is null");
                return;
            } 
            
            foreach (var cellView in CellsList)
            {
                if (cellView.ColorTypes == ColorTypes.None)
                {
                    cellView.Collider.enabled = false;
                    cellView.Renderer.enabled = false;
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if(CellsList != null) 
            {
                Gizmos.color = Color.cyan;
                foreach (var cell in CellsList)
                {
                    Gizmos.DrawWireCube(cell.transform.position, cell.transform.localScale);
                    if (DrawDebugSphere)
                    {
                        Gizmos.DrawSphere(cell.transform.position, cell.transform.localScale.x /2);
                    }
                }
            }
        }
    }
}
