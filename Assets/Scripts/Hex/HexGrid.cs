using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board;
using Building.Pieces;
using DG.Tweening;
using Helpers;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hex
{
    public class HexGrid : Singleton<HexGrid>
    {
        private const int MaxIterations = 100;
        public const int ProblematicNumber1 = 6;
        public const int ProblematicNumber2 = 8;
        
        #region Visuals

        [TabGroup("Board Visuals")] [SerializeField]
        private int gridRadius = 3;

        [TabGroup("Board Visuals")] [SerializeField]
        private float hexSize = 1f;

        [TabGroup("Board Visuals")] [SerializeField]
        private float hexThickness = 0.1f;
        
        [TabGroup("Board Visuals")] [SerializeField]
        private Vector3 numberStackSpawnPosition;

        [TabGroup("Board Visuals")] [SerializeField]
        private Vector3 tileStackSpawnPosition;

        public float HexThickness => hexThickness;

        #endregion
        
        #region Configuration

        [TabGroup("Configuration")] [SerializeField]
        private MapConfiguration mapConfiguration;

        [TabGroup("Configuration")] [SerializeField]
        private NumbersConfiguration numbersConfiguration;

        [TabGroup("Configuration")] [SerializeField] [Unit(Units.Second)]
        private float tilePlacementDelay = 0.1f;

        [TabGroup("Configuration")] [SerializeField] [Unit(Units.Second)]
        private float numberPlacementDelay = 0.1f;

        [TabGroup("Configuration")] [SerializeField] [Unit(Units.Second)]
        private float numberSwapDelay = 0.1f;

        #endregion
        
        [TabGroup("Prefabs")] [SerializeField] private BuildingPoint buildingPointPrefab;
        [TabGroup("Prefabs")] [SerializeField] private Number numberPiecePrefab;
        
        private Dictionary<(int, int), Tile> _tiles;
        private Dictionary<Vector3, HexVertex> _vertices;
        private Dictionary<Vector3, HexCorner> _corners;
        private List<Number> _numberPieces;

        // State
        private bool _isGeneratingBoard;

        private void Start()
        {
            _tiles = new Dictionary<(int, int), Tile>();
            _vertices = new Dictionary<Vector3, HexVertex>();
            _corners = new Dictionary<Vector3, HexCorner>();
            _numberPieces = new List<Number>();
            StartCoroutine(GenerateBoard());
        }

        #region Board Functions

        /// <summary>
        ///     Generates the hexagonal board
        /// </summary>
        private IEnumerator GenerateBoard()
        {
            _isGeneratingBoard = true;
            
            // Loop through each row in the grid
            for (var r = -gridRadius; r <= gridRadius; r++)
            {
                // Calculate the range of cells in the current column
                var q1 = Mathf.Max(-gridRadius, -r - gridRadius);
                var q2 = Mathf.Min(gridRadius, -r + gridRadius);
                for (var q = q1; q <= q2; q++)
                {
                    // Convert the hex grid coordinates (q, r) to world space position (x, y, z)
                    var position = HexHelper.HexToPosition(q, r, hexSize);

                    // Create a new TileData object
                    var tileData = new HexTile(q, r, position, hexThickness);

                    // Generate the vertices for the cell. These vertices will be used to create the mesh
                    HexHelper.CreateVertices(tileData, hexSize, _vertices, _corners);

                    // Create a new GameObject for the tile and add the Tile component
                    var tile = new GameObject($"Tile {q}, {r}").AddComponent<Tile>();
                    tile.transform.position = tileStackSpawnPosition +
                                              new Vector3(0f, 0f + hexThickness * _tiles.Count, 0f);
                    // Set the parent of the tile to the HexGrid object
                    tile.transform.SetParent(transform);

                    // Add the tile to the dictionary and initialize it
                    _tiles.Add((q, r), tile);
                    tile.Init(tileData);
                }
            }

            SpawnNumbers();
            GenerateMap();

            yield return PositionTiles();

            yield return GeneralHelpers.GetWait(tilePlacementDelay);

            yield return AssignNumbers();

            yield return GeneralHelpers.GetWait(numberSwapDelay);

            yield return SwapProblematicTiles();

            SpawnBuildButtons();
            
            _isGeneratingBoard = false;
        }

        /// <summary>
        ///     Randomizes the map by shuffling the list of cell types based on the configuration
        /// </summary>
        private void GenerateMap()
        {
            var shuffledList = HexHelper.GenerateMap(mapConfiguration);

            // Loop through tiles and set type of edge tiles to water and everything else based on the shuffled list
            foreach (var tile in _tiles.Values)
            {
                if (Mathf.Abs(tile.Data.Q) == gridRadius || Mathf.Abs(tile.Data.R) == gridRadius ||
                    Mathf.Abs(tile.Data.Q + tile.Data.R) == gridRadius)
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

                AddTileNeighbours(tile);
            }
        }

        private IEnumerator PositionTiles()
        {
            for (var i = _tiles.Count - 1; i >= 0; i--)
            {
                _tiles.Values.ElementAt(i).transform.DOMove(_tiles.Values.ElementAt(i).Data.Position, 0.3f)
                    .SetEase(Ease.InOutCubic);
                yield return GeneralHelpers.GetWait(tilePlacementDelay);
            }
        }

        private void SpawnNumbers()
        {
            var shuffledNumbers = HexHelper.GenerateNumbers(numbersConfiguration);
            var count = shuffledNumbers.Count;
            for (var i = 0; i < count; i++)
            {
                var num = Instantiate(numberPiecePrefab, numberStackSpawnPosition + new Vector3(0, 0.1f * i, 0),
                    Quaternion.identity);
                num.SetValue(shuffledNumbers[0], NumbersHelper.GetProbability(shuffledNumbers[0]));
                _numberPieces.Add(num);

                shuffledNumbers.RemoveAt(0);
            }

            _numberPieces.Reverse();
        }

        private int _tileIndex;


        private IEnumerator AssignNumbers()
        {
            var numbers = new List<Number>(_numberPieces);
            foreach (var tile in _tiles.Values)
            {
                if (tile.Resource is CellType.Water or CellType.Desert)
                    continue;

                if (numbers.Count > 0)
                {
                    var randomIndex = Random.Range(0, numbers.Count);
                    tile.AssignNumber(numbers[randomIndex]);
                    numbers.RemoveAt(randomIndex);
                }
                else
                {
                    Debug.LogError("Ran out of numbers to assign.");
                    break;
                }

                yield return GeneralHelpers.GetWait(numberPlacementDelay);
            }
        }
        
        private IEnumerator SwapProblematicTiles()
        {
            var conflictsExist = true;
            var iterations = 0;

            while (conflictsExist && iterations < MaxIterations)
            {
                conflictsExist = false;
                iterations++;

                var problematicTiles = _tiles.Values.Where(TileHelpers.IsProblematicTile).ToList();

                if (problematicTiles.Any(TrySwapWithSafeTile))
                {
                    conflictsExist = true;
                    yield return GeneralHelpers.GetWait(numberSwapDelay);
                }
                
                yield return null;
            }

            if (iterations >= MaxIterations) Debug.LogWarning("Reached maximum iterations. Some conflicts may remain.");
        }
        
        private bool TrySwapWithSafeTile(Tile problematicTile)
        {
            foreach (var tile in _tiles.Values)
            {
                if (!TileHelpers.IsSafeTile(tile)) continue;
                // Check if swapping would create a new conflict
                if (WouldCreateConflict(problematicTile, tile)) continue;
                
                SwapNumbers(problematicTile, tile);
                return true;
            }

            return false;
        }

        private bool WouldCreateConflict(Tile tile1, Tile tile2)
        {
            // Temporarily swap numbers
            var tempNumber = tile1.Number;
            tile1.AssignNumber(tile2.Number);
            tile2.AssignNumber(tempNumber);

            // Check for conflicts
            var conflict = ((tile1.Number.Value == 6 || tile1.Number.Value == 8) && tile1.NeighbourHasRedNumber()) ||
                           ((tile2.Number.Value == 6 || tile2.Number.Value == 8) && tile2.NeighbourHasRedNumber());

            // Swap back
            tile2.AssignNumber(tile1.Number);
            tile1.AssignNumber(tempNumber);

            return conflict;
        }

        private void SwapNumbers(Tile tile1, Tile tile2)
        {
            var tempNumber = tile1.Number;
            tile1.AssignNumber(tile2.Number);
            tile2.AssignNumber(tempNumber);
        }

        #endregion

        private void SpawnBuildButtons()
        {
            foreach (var corner in _corners.Values)
            {
                var buildPoint = InstantiateButton(corner.Position, BuildingType.Settlement);
                corner.AssignBuildPoint(buildPoint);
            }

            foreach (var vertex in _vertices.Values)
            {
                // Instantiate a build button at the vertex position
                var buildPoint = InstantiateButton(HexHelper.GetRoadPosition(vertex), BuildingType.Road,
                    HexHelper.GetRoadRotation(vertex));
                // Set the button on the vertex
                vertex.AssignBuildPoint(buildPoint);
            }

            foreach (var tile in _tiles.Values) tile.RefreshButtons();
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

        private void AddTileNeighbours(Tile tile)
        {
            foreach (var corner in tile.Data.Corners)
            foreach (var neighbour in corner.AdjacentTiles)
            {
                if (neighbour == tile) continue;

                tile.AddNeighbour(neighbour);
            }
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
        }

        [TabGroup("Buttons")]
        [Button]
        private void HideAllButtons()
        {
            foreach (var corner in _corners.Values) corner.ToggleBuildPointVisibility(false);

            foreach (var vertex in _vertices.Values) vertex.ToggleBuildPointVisibility(false);
        }

        [TabGroup("Buttons")]
        [Button]
        private void ToggleSettlementButtons()
        {
            foreach (var corner in _corners.Values) corner.ToggleBuildPointVisibility(!corner.BuildPointVisible);
        }

        [TabGroup("Buttons")]
        [Button]
        private void ToggleRoadButtons()
        {
            foreach (var vertex in _vertices.Values) vertex.ToggleBuildPointVisibility(!vertex.BuildPointVisible);
        }

        [Button]
        private void ClearAndRun()
        {
            if (_isGeneratingBoard) return;
            
            foreach (var tile in _tiles.Values) Destroy(tile.gameObject);

            foreach (var vertex in _vertices.Values) Destroy(vertex.BuildPoint.gameObject);

            foreach (var corner in _corners.Values) Destroy(corner.BuildPoint.gameObject);

            foreach (var num in _numberPieces) Destroy(num.gameObject);

            _tiles.Clear();
            _vertices.Clear();
            _corners.Clear();
            _numberPieces.Clear();

            StartCoroutine(GenerateBoard());
        }
    }
}