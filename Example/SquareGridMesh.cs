using UnityEngine;

namespace FeralPug.PathFinding.Example
{
    public class SquareGridMesh
    {
        SquareGridChunk squareGrid;

        MeshFilter m_Filter;
        MeshCollider m_Collider;

        Vector3[] verts;
        int[] indices;
        Vector2[] uvs;
        Color[] colors;

        public SquareGridMesh(SquareGridChunk squareGrid)
        {
            this.squareGrid = squareGrid;
            m_Filter = squareGrid.GetComponent<MeshFilter>();
            m_Collider = squareGrid.GetComponent<MeshCollider>();
        }

        public void TriangulateSquareMesh(SquareGridCell[] squareGridCells)
        {
            int vertexCount = squareGrid.Properties.ChunkVertexCount();
            int indexCount = squareGrid.Properties.ChunkIndexCount();

            verts = new Vector3[vertexCount];
            indices = new int[indexCount];
            uvs = new Vector2[vertexCount];
            colors = new Color[vertexCount];

            for(int i = 0; i < squareGridCells.Length; i++)
            {
                var cell = squareGridCells[i];
                TriangulateCell(cell);
            }

            CreateAndAssignMesh();
        }

        void CreateAndAssignMesh()
        {
            Mesh mesh = new Mesh();

            mesh.vertices = verts;
            mesh.triangles = indices;
            mesh.uv = uvs;
            mesh.colors = colors;

            m_Filter.mesh = mesh;
            m_Collider.sharedMesh = mesh;
        }

        void TriangulateCell(SquareGridCell cell)
        {
            var localCoords = cell.LocalCoordinates;
            var properties = squareGrid.Properties;

            //x * 4 is because we do the whole quad at once
            int quadStart = (properties.cellDimensions.x * 4 * localCoords.y) + (localCoords.x * 4);

            //x * 6 is because we do two triangles at once i.e. 6 indices
            int indexStart = (properties.cellDimensions.x * 6 * localCoords.y) + (localCoords.x * 6);

            var quad = GetQuad(localCoords.x, localCoords.y);

            //assign verts
            verts[quadStart] = quad[0]; //botLeft
            verts[quadStart + 1] = quad[1]; //botright
            verts[quadStart + 2] = quad[2]; //topLeft
            verts[quadStart + 3] = quad[3]; //topRight

            //set colors
            colors[quadStart] = cell.Color; //botLeft
            colors[quadStart + 1] = cell.Color; //botright
            colors[quadStart + 2] = cell.Color; //topLeft
            colors[quadStart + 3] = cell.Color; //topRight

            //set uvs
            uvs[quadStart] = Vector2.zero; //botLeft
            uvs[quadStart + 1] = Vector2.right; //botright
            uvs[quadStart + 2] = Vector2.up; //topLeft
            uvs[quadStart + 3] = Vector2.one; //topRight

            //botLeft Tri
            indices[indexStart] = quadStart;
            indices[indexStart + 1] = quadStart + 2;
            indices[indexStart + 2] = quadStart + 1;

            //topRight Tri
            indices[indexStart + 3] = quadStart + 1;
            indices[indexStart + 4] = quadStart + 2;
            indices[indexStart + 5] = quadStart + 3;
        }

        Vector3[] GetQuad(int x, int y)
        {
            Vector3[] arr = new Vector3[4];

            var Properties = squareGrid.Properties;

            //bot left
            arr[0].x = x * Properties.cellSize;
            arr[0].z = y * Properties.cellSize;

            //bot right
            arr[1].x = x * Properties.cellSize + Properties.cellSize;
            arr[1].z = y * Properties.cellSize;

            //top left
            arr[2].x = x * Properties.cellSize;
            arr[2].z = y * Properties.cellSize + Properties.cellSize;

            //top right
            arr[3].x = x * Properties.cellSize + Properties.cellSize;
            arr[3].z = y * Properties.cellSize + Properties.cellSize;

            return arr;
        }
    }
}
