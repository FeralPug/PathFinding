using UnityEngine;

namespace FeralPug.PathFinding.Example
{
    public class SquareGridCell : IPathable<SquareGridCell>
    {
        SquareGridCell[] neighboors = new SquareGridCell[4];

        public SquareGridChunk GridChunk { get; private set; }

        public Vector2Int LocalCoordinates { get; private set; }

        public Vector2Int GlobalCoordinates { get; private set; }

        public Color Color { get; private set; }

        public MoveCost GridCost { get; set; }

        public Vector3 CenterPosition
        {
            get
            {
                Vector3 position = GridChunk.transform.position + new Vector3(LocalCoordinates.x, 0f, LocalCoordinates.y);
                float halfCellSize = GridChunk.Properties.cellSize / 2;
                position += new Vector3(halfCellSize, 0f, halfCellSize);
                return position;
            }
        }

        public float PathCost => GridCost.cost;
        public bool IsPathable => PathCost >= 0f ? true : false;
        public int NeighboorsCount => neighboors.Length;

        public SquareGridCell(SquareGridChunk gridChunk, Vector2Int localCoords)
        {
            GridChunk = gridChunk;
            LocalCoordinates = localCoords;

            var globalCoords = new Vector2Int();
            globalCoords.x = localCoords.x + (GridChunk.ChunkCoord.x * GridChunk.Properties.cellDimensions.x);
            globalCoords.y = localCoords.y + (GridChunk.ChunkCoord.y * GridChunk.Properties.cellDimensions.y);
            GlobalCoordinates = globalCoords;

            Color = Color.white;
        }

        public SquareGridCell NeighboorAtIndex(int i)
        {
            if (i >= 0 && i < neighboors.Length)
            {
                return neighboors[i];
            }

            throw new System.ArgumentOutOfRangeException(nameof(i));
        }

        public int DistanceToOther(SquareGridCell other)
        {
            return Mathf.Abs(GlobalCoordinates.x - other.GlobalCoordinates.x) +
                Mathf.Abs(GlobalCoordinates.y - other.GlobalCoordinates.y);
        }

        public void SetNeighboor(SquareGridCell neighboor, SquareCellDirection direction)
        {
            int neighboorIndex = (int)direction;
            if (neighboors[neighboorIndex] == null)
            {
                neighboors[neighboorIndex] = neighboor;
                neighboor.SetNeighboor(this, direction.GetReverseDirection());
            }
        }

        public void ChangeColor(Color color)
        {
            Color = color;
            GridChunk.RefreshMesh();
        }
    }
}
