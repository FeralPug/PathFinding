using UnityEngine;

namespace FeralPug.PathFinding.Example
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
    public class SquareGridChunk : MonoBehaviour
    {
        [SerializeField]
        MeshRenderer m_Renderer;

        [SerializeField]
        MeshFilter m_Filter;

        [SerializeField]
        MeshCollider m_Collider;

        SquareGridManager gridManager;

        SquareGridMesh gridMesh;

        public SquareGridCell[] GridCells { get; private set; }

        public int ChunkIndex { get; private set; }

        public Vector2Int ChunkCoord { get; private set; }

        public SquareGridProperties Properties { get; private set; }

        public int CellCount => GridCells.Length;

        public void Initialize(SquareGridManager gridManager, Vector2Int chunkCoord)
        {
            AssignData(gridManager, chunkCoord);
            CreateCells();
        }

        private void LateUpdate()
        {
            TriangulateMesh();
            enabled = false;
        }

        public void RefreshMesh()
        {
            enabled = true;
        }

        public void TriangulateMesh()
        {
            if (GridCells == null) return;

            gridMesh.TriangulateSquareMesh(GridCells);
        }

        public SquareGridCell GetCellAtGlobalCellCoords(Vector2Int globalCellCoord)
        {
            var localCoords = new Vector2Int();
            localCoords.x = globalCellCoord.x % Properties.cellDimensions.x;
            localCoords.y = globalCellCoord.y % Properties.cellDimensions.y;

            return GridCells[Properties.cellDimensions.x * localCoords.y + localCoords.x];
        }

        public SquareGridCell GetCellAtIndex(int index)
        {
            if(index < GridCells.Length)
            {
                return GridCells[index];
            }
            return null;
        }

        void AssignData(SquareGridManager gridManager, Vector2Int chunkCoord)
        {
            this.gridManager = gridManager;
            Properties = gridManager.GridProperties;
            
            ChunkCoord = chunkCoord;
            ChunkIndex = Properties.chunkDimensions.x * ChunkCoord.y + chunkCoord.x;
       
            GridCells = new SquareGridCell[Properties.CellPerChunkCount()];

            gridMesh = new SquareGridMesh(this);

            Vector3 origin = new Vector3();
            origin.x = chunkCoord.x * Properties.cellDimensions.x * Properties.cellSize;
            origin.z = chunkCoord.y * Properties.cellDimensions.y * Properties.cellSize;

            transform.localPosition = origin;
        }

        void CreateCells()
        {
            if (GridCells == null) return;

            for(int y = 0; y < Properties.cellDimensions.y; y++)
            {
                for(int x = 0; x < Properties.cellDimensions.x; x++)
                {
                    var localCoords = new Vector2Int(x, y);
                    var index = Properties.cellDimensions.x * y + x;

                    var cell = new SquareGridCell(this, localCoords);
                    GridCells[index] = cell;

                    cell.GridCost = GetRandomMoveCostForCell();
                    cell.ChangeColor(cell.GridCost.color);

                    if (GridCells[index] == null) Debug.Log("Cell in arr is null");

                    if (x > 0)
                    {
                        cell.SetNeighboor(GridCells[index - 1], SquareCellDirection.West);
                    }
                    else if(ChunkCoord.x > 0)
                    {
                        var neighboor = gridManager.GetCellFromGlobalCellCoord(cell.GlobalCoordinates + SquareCellDirection.West.ToCoordOffset());
                        cell.SetNeighboor(neighboor, SquareCellDirection.West);
                    }
                    if(y > 0)
                    {
                        cell.SetNeighboor(GridCells[index - Properties.cellDimensions.x], SquareCellDirection.South);
                    }
                    else if (ChunkCoord.y > 0)
                    {
                        var neighboor = gridManager.GetCellFromGlobalCellCoord(cell.GlobalCoordinates + SquareCellDirection.South.ToCoordOffset());
                        cell.SetNeighboor(neighboor, SquareCellDirection.South);
                    }
                }
            }
        }

        MoveCost GetRandomMoveCostForCell()
        {
            var totalWeight = Properties.TotalMoveCostWeight();
            var rand = Random.Range(0f, totalWeight);

            for(int i = 0; i < Properties.moveCosts.Length; i++)
            {
                rand -= Properties.moveCosts[i].weight;

                if(rand < 0f)
                {
                    return Properties.moveCosts[i];
                }
            }

            return Properties.moveCosts[Properties.moveCosts.Length - 1];
        }
    }
}
