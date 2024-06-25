using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public static class HexHelper
    {
        public const int NumCorners = 6;
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
        
        public static void CreateVertices(TileData tileData, float hexSize, Dictionary<Vector3, HexVertex> vertices, Dictionary<Vector3, HexCorner> corners)
        {
            tileData.SetCorners(GetHexCorners(tileData.Position, hexSize, corners));

            for (var i = 0; i < tileData.Corners.Length; i++)
            {
                var corner1 = tileData.Corners[i];
                var corner2 = tileData.Corners[(i + 1) % NumCorners];
                var vertex = FindOrCreateVertex(corner1, corner2, vertices);
                tileData.Vertices.Add(vertex);
                var roundedPosition = new Vector3(Mathf.Round(corner1.Position.x * 1000) / 1000, 0,
                    Mathf.Round(corner1.Position.z * 1000) / 1000);

                if (corners.ContainsKey(roundedPosition))
                {
                    continue;
                }

                corners.Add(roundedPosition, corner1);
            }
        }
        
        public static HexCorner[] GetHexCorners(Vector3 center, float hexSize, Dictionary<Vector3, HexCorner> cornersDictionary)
        {
            var corners = new HexCorner[6];
            for (var i = 0; i < 6; i++)
            {
                float angle = 60 * i;
                var angleRad = Mathf.Deg2Rad * angle;
                var position = new Vector3(center.x + hexSize * Mathf.Cos(angleRad),
                    0f,
                    center.z + hexSize * Mathf.Sin(angleRad));

                corners[i] = FindOrCreateCorner(position, cornersDictionary);
            }

            return corners;
        }
        
        private static HexCorner FindOrCreateCorner(Vector3 position, Dictionary<Vector3, HexCorner> corners)
        {
            var roundedPosition = new Vector3(Mathf.Round(position.x * 1000) / 1000, 0, Mathf.Round(position.z * 1000) / 1000);
            
            if (corners.ContainsKey(roundedPosition))
            {
                return corners[roundedPosition];
            }

            var corner = new HexCorner(position);
            corners.Add(roundedPosition, corner);
            return corners[roundedPosition];
        }

        private static HexVertex FindOrCreateVertex(HexCorner startCorner, HexCorner endCorner, Dictionary<Vector3, HexVertex> vertices)
        {
            
            // Round the sum of the x and z coordinates of the start and end corners to 3 decimal places
            var uniqueVertexIdentifier = GetUniqueVertexIdentifier(startCorner.Position, endCorner.Position);
            
            if (vertices.TryGetValue(uniqueVertexIdentifier, out var foundVertex))
            {
                return foundVertex;
            }

            var vertex = new HexVertex(startCorner, endCorner);
            vertices.Add(uniqueVertexIdentifier, vertex);
            
            return vertices[uniqueVertexIdentifier];
        }
        
        public static Vector3 GetUniqueVertexIdentifier(Vector3 position1, Vector3 position2)
        {
            var combinedPosition = position1 + position2;
            
            return new Vector3(Mathf.Round(combinedPosition.x * 1000) / 1000, 0, Mathf.Round(combinedPosition.z * 1000) / 1000);
        }

        public static Vector3 GetRoadPosition(HexVertex vertex)
        {
            return (vertex.Position1 + vertex.Position2) / 2;
        }
        
        // Get road rotation on Y axis based on the direction of the road
        public static Quaternion GetRoadRotation(HexVertex vertex)
        {
            var midPoint = GetRoadPosition(vertex);
            
            var direction = (vertex.Position1 - midPoint).normalized;

            return Quaternion.LookRotation(direction);
        }
    }
}