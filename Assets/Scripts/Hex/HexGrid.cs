using System.Collections.Generic;
using Helpers;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hex
    {
        public class HexGrid : Singleton<HexGrid>
        {
            [TabGroup("Board Visuals")][SerializeField] private int gridRadius = 3;
            [TabGroup("Board Visuals")][SerializeField] private float hexSize = 1f;
            [TabGroup("Board Visuals")][SerializeField] private float hexThickness = 0.1f;

            public float HexThickness => hexThickness;
            
            [TabGroup("Prefabs")][SerializeField] private BuildingPoint buildingPointPrefab;
            
            [TabGroup("Map Configuration")][SerializeField][InlineEditor] private MapConfiguration mapConfiguration;
            
            private Dictionary<(int, int), Tile> _tiles;
            private Dictionary<Vector3, HexVertex> _vertices;
            private Dictionary<Vector3, HexCorner> _corners;
            
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
                        var tileData = new HexTile(q, r, position, hexThickness);
                        
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
                    var buildPoint = InstantiateButton(corner.Position , BuildingType.Settlement);
                    corner.AssignBuildPoint(buildPoint);
                }
                
                foreach (var vertex in _vertices.Values)
                {
                    // Instantiate a build button at the vertex position
                    var buildPoint = InstantiateButton(HexHelper.GetRoadPosition(vertex), BuildingType.Road, HexHelper.GetRoadRotation(vertex));
                    // Set the button on the vertex
                    vertex.AssignBuildPoint(buildPoint);
                }
            }

            private BuildingPoint InstantiateButton(Vector3 pos, BuildingType type)
            {
                var obj = Instantiate(buildingPointPrefab, pos, Quaternion.identity);
                obj.Initialize(pos, type);
                obj.transform.SetParent(transform);
                return obj;
            }
            
            private BuildingPoint InstantiateButton(Vector3 pos, BuildingType type, Quaternion rotation)
            {
                var obj = Instantiate(buildingPointPrefab, pos, rotation);
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
            
            [TabGroup("Buttons"), Button]
            private void HideAllButtons()
            {
                foreach (var corner in _corners.Values)
                {
                    corner.ToggleBuildPointVisibility(false);
                }
                
                foreach (var vertex in _vertices.Values)
                {
                    vertex.ToggleBuildPointVisibility(false);
                }
            }
            
            [TabGroup("Buttons"), Button]
            private void ToggleSettlementButtons()
            {
                foreach (var corner in _corners.Values)
                {
                    corner.ToggleBuildPointVisibility(!corner.BuildPointVisible);
                }
            }
            
            [TabGroup("Buttons"), Button]
            private void ToggleRoadButtons()
            {
                foreach (var vertex in _vertices.Values)
                {
                    vertex.ToggleBuildPointVisibility(!vertex.BuildPointVisible);
                }
            }
        }
    }