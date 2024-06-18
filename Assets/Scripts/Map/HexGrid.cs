using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Map
{
    public class HexGrid : MonoBehaviour
    {
        [TabGroup("Board Visuals")][SerializeField] private int gridRadius = 3;
        [TabGroup("Board Visuals")][SerializeField] private float hexSize = 1f;
        [TabGroup("Board Visuals")][SerializeField] private float hexThickness = 0.1f;
        
        [TabGroup("Map Configuration")][SerializeField] private MapConfiguration mapConfiguration;
        
        private Dictionary<(int, int), Tile> _tiles;
        private Dictionary<Vector3, HexVertex> _vertices;
        
        void Start()
        {
            _tiles = new Dictionary<(int, int), Tile>();
            _vertices = new Dictionary<Vector3, HexVertex>();
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
                int r2 = Mathf.Min(gridRadius, -q + gridRadius);
                for (int r = r1; r <= r2; r++)
                {
                    var position = HexToPosition(q, r);
                    var cell = new TileData(q, r, position, hexThickness);
                    var tile = new GameObject($"Tile {q}, {r}").AddComponent<Tile>();
                    _tiles.Add((q, r), tile);
                    CreateVertices(cell);
                    tile.Initialize(cell);
                }
            }
        }
        
        [Button("Randomize Board")]
        private void RandomizeBoard()
        {
            List<CellType> shuffledList = HexHelper.GenerateBoard(mapConfiguration);

            // Loop through tiles and set type of edge tiles to water and everything else based on the shuffled list
            foreach (var tile in _tiles.Values)
            {
                if (Mathf.Abs(tile.Data.Q) == gridRadius || Mathf.Abs(tile.Data.R) == gridRadius || Mathf.Abs(tile.Data.Q + tile.Data.R) == gridRadius)
                {
                    tile.SetType(CellType.Water);
                }
                else
                {
                    // Check if shuffledList is not empty before accessing its elements
                    if (shuffledList.Count > 0)
                    {
                        tile.SetType(shuffledList[0]);
                        shuffledList.RemoveAt(0);
                    }
                    else
                    {
                        // shuffledList is empty, skip setting the type or set it to a default value
                        tile.SetType(CellType.Water);
                    }
                }
            }
        }
        
        private Vector3 HexToPosition(int q, int r)
        {
            var z = hexSize * (Mathf.Sqrt(3f) * (r + q / 2f));
            var x = hexSize * (3f / 2f * q);
            return new Vector3(x, 0, z);
        }

        private void CreateVertices(TileData tileData)
        {
            Vector3[] corners = GetHexCorners(tileData.Position);

            foreach (Vector3 corner in corners)
            {
                HexVertex vertex = FindOrCreateVertex(corner);
                tileData.Vertices.Add(vertex);
            }
        }

        private Vector3[] GetHexCorners(Vector3 center)
        {
            Vector3[] corners = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = 60 * i;
                float angleRad = Mathf.Deg2Rad * angle;
                corners[i] = new Vector3(center.x + hexSize * Mathf.Cos(angleRad),
                    0f,
                    center.z + hexSize * Mathf.Sin(angleRad));
            }

            return corners;
        }

        private HexVertex FindOrCreateVertex(Vector3 position)
        {
            if (_vertices.ContainsKey(position)) return _vertices[position];
            
            HexVertex vertex = new HexVertex(position);
            _vertices.Add(position, vertex);
            return _vertices[position];
        }

        private void OnDrawGizmos()
        {
            if (_tiles == null || _vertices == null) return;
            
            foreach (var cell in _tiles.Values)
            {
                // Draw cell center
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(cell.Data.Position, 0.1f);
                
                Vector3[] corners = GetHexCorners(cell.Data.Position);
                for (int i = 0; i < 6; i++)
                {
                    // Draw cell edges
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(corners[i], corners[(i + 1) % 6]);
                    
                    // Draw road building spots
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere((corners[i] + corners[(i + 1) % 6])/2, 0.05f);
                }
            }
            
            // Draw settlement building spots
            Gizmos.color = Color.green;
            foreach (var vertex in _vertices.Values)
            {
                Gizmos.DrawSphere(vertex.Position, 0.05f);
            }
            
        }
    }
}
