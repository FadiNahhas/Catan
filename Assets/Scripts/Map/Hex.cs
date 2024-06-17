using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public enum CellType
    {
        Desert,
        Wood,
        Brick,
        Ore,
        Grain,
        Wool,
        Water
    }

    public static class HexHelper
    {
        public static Color GetColor(CellType type)
        {
            return type switch
            {
                CellType.Desert => new Color(1f, 0.9f, 0.6f),
                CellType.Wood => new Color(0.1f, 0.5f, 0.1f),
                CellType.Brick => new Color(0.8f, 0.2f, 0.1f),
                CellType.Ore => new Color(0.6f, 0.6f, 0.6f),
                CellType.Grain => new Color(0.9f, 0.9f, 0.1f),
                CellType.Wool => new Color(0.4f, 0.9f, 0.4f),
                CellType.Water => new Color(0.3f, 0.6f, 1f),
                _ => Color.black
            };
        }

        public static List<CellType> GenerateCells(MapConfiguration configuration)
        {
            List<CellType> cells = new();

            foreach (var cellTypeCount in configuration.cellTypeCounts)
            {
                for (int i = 0; i < cellTypeCount.count; i++)
                {
                    cells.Add(cellTypeCount.type);
                }
            }
            
            // Shuffle the list
            System.Random random = new System.Random();
            cells = cells.OrderBy(x => random.Next()).ToList();
            
            return cells;
        }
    }
    
    public class HexVertex
    {
        public Vector3 Position { get; private set; }
        
        public HexVertex(Vector3 position)
        {
            Position = position;
        }
    }

    [Serializable]
    public class HexCell
    {
        public Vector3 Position { get; private set; }
        public List<HexVertex> Vertices { get; private set; }
        
        public int Q { get; private set; }
        public int R { get; private set; }
        
        public CellType Type { get; private set; }
        
        public bool HasRobber { get; private set; }

        public Mesh CreateMesh(float height)
        {
            Mesh mesh = new Mesh();

            // Create top and bottom vertices
            Vector3[] topVertices = Vertices.Select(v => v.Position - Position).ToArray();
            Vector3[] bottomVertices = Vertices.Select(v => v.Position - Position - new Vector3(0, height, 0)).ToArray();

            // Combine top and bottom vertices
            Vector3[] vertices = topVertices.Concat(bottomVertices).ToArray();
            mesh.vertices = vertices;

            // Create top and bottom faces
            int[] topTriangles = {
                0, 2, 1,
                0, 3, 2,
                0, 4, 3,
                0, 5, 4,
            };
            
            int[] bottomTriangles = {
                6, 8, 7,
                6, 9, 8,
                6, 10, 9,
                6, 11, 10,
            };

            // Create side faces
            int[] sideTriangles = {
                0, 7, 6,
                0, 1, 7,
                1, 8, 7,
                1, 2, 8,
                2, 9, 8,
                2, 3, 9,
                3, 10, 9,
                3, 4, 10,
                4, 11, 10,
                4, 5, 11,
                5, 6, 11,
                5, 0, 6
            };

            // Combine all triangles
            int[] triangles = topTriangles.Concat(bottomTriangles).Concat(sideTriangles).ToArray();
            mesh.triangles = triangles;

            return mesh;
        }

        public void ApplyTexture(Material material, float height = 0.1f)
        {
            Mesh mesh = CreateMesh(height);

            GameObject cell = new GameObject("Hex Cell")
            {
                transform =
                {
                    position = Position
                }
            };

            MeshFilter meshFilter = cell.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            
            MeshRenderer meshRenderer = cell.AddComponent<MeshRenderer>();
            meshRenderer.material = material;
        }
        
        public HexCell(int q, int r, Vector3 position, CellType type)
        {
            Position = position;
            Q = q;
            R = r;
            Type = type;
            Vertices = new List<HexVertex>();
        }
    }
}