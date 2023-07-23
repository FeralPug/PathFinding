using UnityEngine;
using System.Threading;
using System.Collections.Generic;

namespace FeralPug.PathFinding.Example
{
    public class SquareGridManager : MonoBehaviour
    {
        [SerializeField]
        SquareGridProperties gridProperties;

        [SerializeField]
        SquareGridChunk gridPrefab;

        SquareGridChunk[] gridChunks;

        PathFinder<SquareGridCell> gridPathManager;

        Queue<PathResult<SquareGridCell>> pathResults = new Queue<PathResult<SquareGridCell>>();

        public List<SquareGridCell> StartCells = new List<SquareGridCell>();
        public SquareGridCell EndCell { get; set; }

        public SquareGridProperties GridProperties => gridProperties;

        private void Awake()
        {
            Initialize();
            SetCameraPosition();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectCell();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ClearCells();
            }

            DequeueResults();
        }

        void DequeueResults()
        {
            lock(pathResults)
            {
                while (pathResults.Count > 0)
                {
                    var result = pathResults.Dequeue();
                    result.callBack(result.pathableWrappers, result.success);
                }
            }
        }

        public SquareGridCell RayCastToCell()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var hitPos = hit.point;
                return GetCellAtPosition(hitPos);
            }

            return null;
        }

        public SquareGridCell GetCellAtPosition(Vector3 position)
        {
            if (gridChunks == null) return null;

            Vector2Int globalCoords = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));

            var chunk = GetChunkFromGlobalCellCoord(globalCoords);

            var cell = chunk.GetCellAtGlobalCellCoords(globalCoords);

            return cell;
        }

        public SquareGridCell GetCellFromGlobalCellCoord(Vector2Int globalCellCoord)
        {
            var chunk = GetChunkFromGlobalCellCoord(globalCellCoord);
            return chunk.GetCellAtGlobalCellCoords(globalCellCoord);
        }

        private void Initialize()
        {
            gridPathManager = new PathFinder<SquareGridCell>(gridProperties.TotalCellCount());
            ValidateMeshDimensions();
            CreateSquareGridChunks();
        }

        void ValidateMeshDimensions()
        {
            gridProperties.ValidateMeshDimensions();
        }

        void CreateSquareGridChunks()
        {
            gridChunks = new SquareGridChunk[gridProperties.ChunkCount()];

            for(int y = 0; y < gridProperties.chunkDimensions.y; y++)
            {
                for (int x = 0; x < gridProperties.chunkDimensions.x; x++)
                {
                    var chunk = Instantiate(gridPrefab, transform);
                    chunk.Initialize(this, new Vector2Int(x, y));
                    gridChunks[gridProperties.chunkDimensions.x * y + x] = chunk;
                }
            }
        }

        void SelectCell()
        {
            var cell = RayCastToCell();

            if (cell != null)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (EndCell != null)
                    {
                        EndCell.ChangeColor(EndCell.GridCost.color);
                    }

                    if (cell != EndCell && !StartCells.Contains(cell))
                    {
                        EndCell = cell;
                        EndCell.ChangeColor(Color.green);
                    }
                    else
                    {
                        EndCell = null;
                    }
                }
                else
                {
                    if (!StartCells.Contains(cell) && cell != EndCell)
                    {
                        cell.ChangeColor(Color.blue);
                        StartCells.Add(cell);
                    }

                    else if(StartCells.Contains(cell))
                    {
                        cell.ChangeColor(cell.GridCost.color);
                        StartCells.Remove(cell);

                    }
                }
            }

            if (StartCells.Count > 0 && EndCell != null)
            {
                DoActionForEveryCell((cell) =>
                    {
                        if (!StartCells.Contains(cell) && cell != EndCell)
                        {
                            cell.ChangeColor(cell.GridCost.color);
                        }
                    }
                );

                for(int i = 0; i <  StartCells.Count; i++)
                {
                    PathRequest<SquareGridCell> pathRequest = new PathRequest<SquareGridCell>(StartCells[i], EndCell, (x, y) => DrawPath(x, y));
                    ThreadStart threadStart = delegate {
                        gridPathManager.FindPath(pathRequest, QueueResult);
                    };
                    threadStart.Invoke();
                }
            }
        }

        void ClearCells()
        {
            StartCells.Clear();
            EndCell = null;
            DoActionForEveryCell(cell => cell.ChangeColor(cell.GridCost.color));
        }

        void QueueResult(PathResult<SquareGridCell> result)
        {
            lock (pathResults)
            {
                pathResults.Enqueue(result);
            }
        }


        void DrawPath(PathableWrapper<SquareGridCell>[] pathables, bool success)
        {
            if (success)
            {
                for(int i = 1;  i < pathables.Length - 1; i++)
                {
                    var cell = pathables[i].pathable;
                    cell.ChangeColor(Color.red);
                }
            }
        }

        SquareGridChunk GetChunkFromGlobalCellCoord(Vector2Int globalCellCoord)
        {
            Vector2Int chunkCoord = new Vector2Int();
            chunkCoord.x = globalCellCoord.x / gridProperties.cellDimensions.x;
            chunkCoord.y = globalCellCoord.y / gridProperties.cellDimensions.y;
            return gridChunks[gridProperties.chunkDimensions.x * chunkCoord.y + chunkCoord.x];
        }

        void SetCameraPosition()
        {
            var maxCellWidth = gridProperties.cellDimensions.x > gridProperties.cellDimensions.y ?
                gridProperties.cellDimensions.x : gridProperties.cellDimensions.y;

            var maxChunkWidth = gridProperties.chunkDimensions.x > gridProperties.chunkDimensions.y ?
                gridProperties.chunkDimensions.x : gridProperties.chunkDimensions.y;

            var width = maxCellWidth * gridProperties.cellSize * maxChunkWidth;

            var dist = (width * 1.25 / 2f) / Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2f);

            float horPos = gridProperties.cellDimensions.x * gridProperties.cellSize * gridProperties.chunkDimensions.x;
            horPos /= 2f;
            float vertPos = gridProperties.cellDimensions.y * gridProperties.cellSize * gridProperties.chunkDimensions.y;
            vertPos /= 2f;

            Camera.main.transform.position = new Vector3(horPos, dist, vertPos);
            Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        void DoActionForEveryCell(System.Action<SquareGridCell> action)
        {
            for (int i = 0; i < gridChunks.Length; i++)
            {
                var chunk = gridChunks[i];
                int cellCount = chunk.CellCount;
                for (int j = 0; j < cellCount; j++)
                {
                    var chunkCell = chunk.GetCellAtIndex(j);
                    action(chunkCell);
                }
            }
        }
    }
}
