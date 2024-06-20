using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
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

        public static List<CellType> GenerateMap(MapConfiguration configuration)
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
        
        /// <summary>
        /// Converts hex grid coordinates to world space position
        /// </summary>
        /// <param name="q">The q coordinate of the hex cell</param>
        /// <param name="r">The r coordinate of the hex cell</param>
        /// <param name="hexSize">The size of the hex cell</param>
        /// <returns>The world space position of the hex cell</returns>
        public static Vector3 HexToPosition(int q, int r, float hexSize)
        {
            var z = hexSize * (Mathf.Sqrt(3f) * (r + q / 2f));
            var x = hexSize * (3f / 2f * q);
            return new Vector3(x, 0, z);
        }
        
        public static void CreateVertices(TileData tileData, float hexSize, Dictionary<Vector3, HexVertex> vertices)
        {
            tileData.SetCorners(GetHexCorners(tileData.Position, hexSize));

            foreach (var corner in tileData.Corners)
            {
                var vertex = FindOrCreateVertex(corner, vertices);
                tileData.Vertices.Add(vertex);
            }
        }
        
        public static Vector3[] GetHexCorners(Vector3 center, float hexSize)
        {
            var corners = new Vector3[6];
            for (var i = 0; i < 6; i++)
            {
                float angle = 60 * i;
                var angleRad = Mathf.Deg2Rad * angle;
                corners[i] = new Vector3(center.x + hexSize * Mathf.Cos(angleRad),
                    0f,
                    center.z + hexSize * Mathf.Sin(angleRad));
            }

            return corners;
        }

        private static HexVertex FindOrCreateVertex(Vector3 position, Dictionary<Vector3, HexVertex> vertices)
        {
            var roundedPosition = new Vector3(Mathf.Round(position.x * 1000) / 1000, 0, Mathf.Round(position.z * 1000) / 1000);
            
            if (vertices.ContainsKey(roundedPosition))
            {
                return vertices[roundedPosition];
            }

            var vertex = new HexVertex(position);
            Debug.Log($"Vertex at {roundedPosition}");
            vertices.Add(roundedPosition, vertex);
            return vertices[roundedPosition];
        }
        
        public static void SetNeighborsForAllVertices(Dictionary<Vector3, HexVertex> vertices, Dictionary<(int, int), Tile> tiles)
        {
            foreach (var vertex in vertices.Values)
            {
                List<HexVertex> neighbors = new List<HexVertex>();

                foreach (var tile in tiles.Values)
                {
                    if (tile.Data.Vertices.Contains(vertex))
                    {
                        // Get the indices of the current vertex and its neighbors in the tile's vertices list
                        int currentIndex = tile.Data.Vertices.IndexOf(vertex);
                        int previousIndex = (currentIndex + tile.Data.Vertices.Count - 1) % tile.Data.Vertices.Count;
                        int nextIndex = (currentIndex + 1) % tile.Data.Vertices.Count;

                        // Add the neighbors to the list
                        neighbors.Add(tile.Data.Vertices[previousIndex]);
                        neighbors.Add(tile.Data.Vertices[nextIndex]);
                    }
                }

                vertex.SetNeighbors(neighbors.Distinct().ToList());
            }
        }
    }
}