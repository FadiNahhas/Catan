using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace Map
{
    public class HexGrid : SerializedMonoBehaviour
    {
        [TabGroup("Board Visuals")][SerializeField] private int gridRadius = 3;
        [TabGroup("Board Visuals")][SerializeField] private float hexSize = 1f;
        [TabGroup("Board Visuals")][SerializeField] private float hexThickness = 0.1f;
        
        [TabGroup("Prefabs")][SerializeField] private BuildButton buildButtonPrefab;
        
        [TabGroup("Map Configuration")][SerializeField][InlineEditor] private MapConfiguration mapConfiguration;
        
        [OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "Coordinates", ValueLabel = "Tiles")] private Dictionary<(int, int), Tile> _tiles;
        [OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "Position", ValueLabel = "Vertices")] private Dictionary<Vector3, HexVertex> _vertices;
        [OdinSerialize] [DictionaryDrawerSettings(KeyLabel = "Positions", ValueLabel = "Corners")] private Dictionary<Vector3, HexCorner> _corners;
        
        void Start()
        {
            _tiles = new Dictionary<(int, int), Tile>();
            _vertices = new Dictionary<Vector3, HexVertex>();
            _corners = new Dictionary<Vector3, HexCorner>();
            GenerateBoard();
        }

        #region Board Functions

        /// <summary>
        /// Generates the hexagonal board
        /// </summary>
        private void GenerateBoard()
        {
            // Loop through each cell in the grid
            for (var q = -gridRadius; q <= gridRadius; q++)
            {
                // Calculate the range of cells in the current row
                var r1 = Mathf.Max(-gridRadius, -q - gridRadius);
                var r2 = Mathf.Min(gridRadius, -q + gridRadius);
                for (var r = r1; r <= r2; r++)
                {
                    // Convert the hex grid coordinates (q, r) to world space position (x, y, z)
                    var position = HexHelper.HexToPosition(q, r, hexSize);
                    
                    // Create a new TileData object
                    var tileData = new TileData(q, r, position, hexThickness);
                    
                    // Generate the vertices for the cell. These vertices will be used to create the mesh
                    HexHelper.CreateVertices(tileData, hexSize, _vertices, _corners);
                    
                    // Create a new GameObject for the tile and add the Tile component
                    var tile = new GameObject($"Tile {q}, {r}").AddComponent<Tile>();
                    // Set the parent of the tile to the HexGrid object
                    tile.transform.SetParent(transform);
                    
                    // Add the tile to the dictionary and initialize it
                    _tiles.Add((q, r), tile);
                    tile.Initialize(tileData);
                }
            }
            HexHelper.SetNeighborsForAllVertices(_vertices, _tiles);
            SpawnBuildButtons();
        }
        
        /// <summary>
        /// Randomizes the map by shuffling the list of cell types based on the configuration
        /// </summary>
        [Button]
        private void GenerateMap()
        {
            var shuffledList = HexHelper.GenerateMap(mapConfiguration);

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
        
        /// <summary>
        /// Clears the board by setting all tiles to water
        /// </summary>
        [Button("Clear Board")]
        private void ClearBoard()
        {
            foreach (var tile in _tiles.Values)
            {
                tile.SetType(CellType.Water);
            }
        }
        
        #endregion
        
        private void SpawnBuildButtons()
        {
            foreach (var corner in _corners.Values)
            {
                if (corner.ButtonsSpawned)
                {
                    Debug.Log("Skipping corner");
                    continue;
                }
                
                var btn = InstantiateButton(corner.Position , BuildingType.Settlement);
                corner.SetButtonsSpawned();
            }
            
            foreach (var vertex in _vertices.Values)
            {
                // Instantiate a build button at the vertex position
                var btn = InstantiateButton(HexHelper.GetRoadPosition(vertex), BuildingType.Road, HexHelper.GetRoadRotation(vertex));
                // Set the button on the vertex
                vertex.SetButton(btn);
            }
        }

        private BuildButton InstantiateButton(Vector3 pos, BuildingType type)
        {
            var obj = Instantiate(buildButtonPrefab, pos, Quaternion.identity);
            obj.Initialize(pos, type);
            obj.transform.SetParent(transform);
            return obj;
        }
        
        private BuildButton InstantiateButton(Vector3 pos, BuildingType type, Quaternion rotation)
        {
            var obj = Instantiate(buildButtonPrefab, pos, rotation);
            obj.Initialize(pos, type);
            obj.transform.SetParent(transform);
            return obj;
        }
        
        private void OnDrawGizmos()
        {
            if (_tiles == null || _vertices == null) return;
            
            foreach (var tile in _tiles.Values)
            {
                // Draw cell center

                Gizmos.color = tile.IsHovered ? Color.green : Color.red;
                Gizmos.DrawSphere(tile.Data.Position, tile.IsHovered ? 0.2f : 0.1f);

                for (var i = 0; i < 6; i++)
                {
                    // Draw cell edges
                    
                    Gizmos.color = tile.IsHovered ? Color.red : Color.blue;
                    Gizmos.DrawLine(tile.Data.Corners[i].Position, tile.Data.Corners[(i + 1) % 6].Position);
                }
            }

            foreach (var ver in _vertices.Values)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(HexHelper.GetRoadPosition(ver), 0.1f);
                
            }
            
        }
    }
}
