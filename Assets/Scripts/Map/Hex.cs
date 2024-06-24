using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    [Serializable]
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

    [Serializable]
    public class TileData
    {
        public Vector3 Position { get; private set; }
        public List<HexVertex> Vertices { get; private set; }

        public HexCorner[] Corners { get; private set; }
        
        public int Q { get; private set; }
        public int R { get; private set; }
        
        public float Height { get; private set; }
        
        public TileData(int q, int r, Vector3 position, float height)
        {
            Position = position;
            Q = q;
            R = r;
            Height = height;
            Vertices = new List<HexVertex>();
        }
        
        public Mesh CreateMesh()
        {
            // Create a new mesh
            Mesh mesh = new Mesh();

            // Create top and bottom vertices
            Vector3[] topVertices = Corners.Select(c => c.Position - Position).ToArray();
            Vector3[] bottomVertices = Corners.Select(c => c.Position - Position - new Vector3(0, Height, 0)).ToArray();

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
        
        public void SetCorners(HexCorner[] corners)
        {
            Corners = corners;

            for (int i = 0; i < Corners.Length; i++)
            {
                if (!Corners[i].Neighbors.Contains(Corners[(i + 1) % Corners.Length]))
                {
                    Debug.Log("Adding neighbor");
                    Corners[i].Neighbors.Add(Corners[(i + 1) % Corners.Length]);
                }

                if (!Corners[i].Neighbors.Contains(Corners[(i + 5) % Corners.Length]))
                {
                    Debug.Log("Adding neighbor");
                    Corners[i].Neighbors.Add(Corners[(i + 5) % Corners.Length]);
                }
            }
        }
    }
}