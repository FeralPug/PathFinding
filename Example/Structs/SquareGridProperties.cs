using UnityEngine;

namespace FeralPug.PathFinding.Example
{
    [System.Serializable]
    public struct SquareGridProperties
    {
        public Vector2Int chunkDimensions;
        public Vector2Int cellDimensions;
        public float cellSize;
        public MoveCost[] moveCosts;

        public int ChunkCount()
        {
            return chunkDimensions.x * chunkDimensions.y;
        }

        public int CellPerChunkCount()
        {
            return cellDimensions.x * cellDimensions.y;
        }

        public int TotalCellCount()
        {
            return ChunkCount() * CellPerChunkCount();
        }

        public Vector2Int TotalCellDimensions()
        {
            return new Vector2Int(chunkDimensions.x * cellDimensions.x, chunkDimensions.y * cellDimensions.y);
        }

        public int ChunkVertexCount()
        {
            //the four is because we are doing squares
            return cellDimensions.x * 4 * cellDimensions.y;
        }

        public int ChunkIndexCount()
        {
            //the 2 is because we are doing squares and each square has 2 triangles
            int triCount = CellPerChunkCount() * 2;
            return triCount * 3;
        }

        public void ValidateMeshDimensions()
        {
            //127 is because with that many cells, any more will be greater than int16.MaxValue
            //which is default index format in unity
            cellDimensions.x = cellDimensions.x > 127 ? 127 : cellDimensions.x;
            cellDimensions.y = cellDimensions.y > 127 ? 127 : cellDimensions.y;
        }

        public float TotalMoveCostWeight()
        {
            float value = 0;
            for(int i = 0; i < moveCosts.Length; i++)
            {
                value += moveCosts[i].weight;
            }
            return value;
        }
    }
}
