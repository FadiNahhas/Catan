using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board;
using Building.Pieces;
using DG.Tweening;
using Helpers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hex
{
    /// <summary>
    ///     Class responsible for generating the game board
    /// </summary>
    public class HexGrid : Singleton<HexGrid>
    {
        public const float HexThickness = 0.1f;
        private const int MaxIterations = 100;
        public const int ProblematicNumber1 = 6;
        public const int ProblematicNumber2 = 8;
        private static readonly Vector3 NumberPileStackOffset = new(0, 0.1f, 0);
        private static readonly Vector3 YOffset = new(0, HexThickness, 0);

        #region Visuals

        [TabGroup("Board Visuals")] [SerializeField]
        private int gridRadius = 3;

        [TabGroup("Board Visuals")] [SerializeField]
        private float hexSize = 1f;


        [TabGroup("Board Visuals")] [SerializeField]
        private Vector3 numberStackSpawnPosition;

        [TabGroup("Board Visuals")] [SerializeField]
        private Vector3 tileStackSpawnPosition;
        
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
        [TabGroup("Prefabs")] [SerializeField] private Tile tilePrefab;

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
                    var tileData = new HexTile(q, r, position, HexThickness);

                    // Generate the vertices for the cell. These vertices will be used to create the mesh
                    HexHelper.CreateVertices(tileData, hexSize, _vertices, _corners);

                    // Create a new GameObject for the tile and add the Tile component
                    var tile = Instantiate(tilePrefab,
                        tileStackSpawnPosition + new Vector3(0f, 0f + HexThickness * _tiles.Count, 0f), Quaternion.identity);
                    
                    tile.gameObject.name = $"Tile ({q}, {r})";

                    // Set the parent of the tile to the HexGrid object
                    tile.transform.SetParent(transform);

                    // Add the tile to the dictionary and initialize it
                    _tiles.Add((q, r), tile);
                    tile.Init(tileData);
                }
            }

            SpawnNumbers();

            GenerateResources();

            yield return PositionTiles();

            yield return GeneralHelpers.GetWait(tilePlacementDelay);

            yield return AssignNumbers();

            yield return GeneralHelpers.GetWait(numberSwapDelay);

            yield return SwapProblematicTiles();

            yield return SpawnBuildButtons();

            _isGeneratingBoard = false;
        }

        /// <summary>
        ///     Generates a shuffled list of resources and assigns them to the tiles
        /// </summary>
        private void GenerateResources()
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

                // Set up the tile's neighbours
                tile.AddTileNeighbours();
            }
        }

        /// <summary>
        ///     Loops through the pile of hex tiles and moves them to their final position
        /// </summary>
        private IEnumerator PositionTiles()
        {
            for (var i = _tiles.Count - 1; i >= 0; i--)
            {
                _tiles.Values.ElementAt(i).transform.DOMove(_tiles.Values.ElementAt(i).Data.Position - YOffset, 0.3f)
                    .SetEase(Ease.InOutCubic);
                yield return GeneralHelpers.GetWait(tilePlacementDelay);
            }
        }

        /// <summary>
        ///     Spawns the pile of shuffled number pieces
        /// </summary>
        private void SpawnNumbers()
        {
            // Generate numbers based on the configuration
            var shuffledNumbers = HexHelper.GenerateNumbers(numbersConfiguration);
            var count = shuffledNumbers.Count;

            // Loop through the shuffled numbers and instantiate a number piece for each
            for (var i = 0; i < count; i++)
            {
                var num = Instantiate(numberPiecePrefab, numberStackSpawnPosition + NumberPileStackOffset * i,
                    Quaternion.identity);
                num.SetValue(shuffledNumbers[0], NumbersHelper.GetProbability(shuffledNumbers[0]));
                _numberPieces.Add(num);

                shuffledNumbers.RemoveAt(0);
            }

            // Reverse the list so that the last number is on top of the stack
            _numberPieces.Reverse();
        }

        /// <summary>
        ///     Randomly assigns numbers to the tiles
        /// </summary>
        private IEnumerator AssignNumbers()
        {
            // Clone the number pieces list
            var numbers = new List<Number>(_numberPieces);

            // Loop through the tiles and assign a number to each
            foreach (var tile in _tiles.Values)
            {
                // Skip water and desert tiles
                if (tile.Resource is CellType.Water or CellType.Desert)
                    continue;

                // Check if there are any numbers left to assign
                if (numbers.Count > 0)
                {
                    // Assign a random number from the list
                    var randomIndex = Random.Range(0, numbers.Count);
                    tile.AssignNumber(numbers[randomIndex]);
                    // Remove the number from the list
                    numbers.RemoveAt(randomIndex);
                }
                else
                {
                    Debug.LogError("Ran out of numbers to assign.");
                    break;
                }

                // Wait for a short delay before assigning the next number
                yield return GeneralHelpers.GetWait(numberPlacementDelay);
            }
        }

        /// <summary>
        ///     Swaps numbers around in case of tiles having 6s or 8s next to each other
        /// </summary>
        private IEnumerator SwapProblematicTiles()
        {
            var conflictsExist = true;
            var iterations = 0;

            while (conflictsExist && iterations < MaxIterations)
            {
                conflictsExist = false;
                iterations++;

                // Get a list of tiles that have 6s or 8s next to each other
                var problematicTiles = _tiles.Values.Where(TileHelpers.IsProblematicTile).ToList();

                // Try to swap the problematic tiles with safe tiles
                if (problematicTiles.Any(TrySwapWithSafeTile))
                {
                    conflictsExist = true;
                    yield return GeneralHelpers.GetWait(numberSwapDelay);
                }

                yield return null;
            }

            // Log a warning if the maximum number of iterations is reached
            if (iterations >= MaxIterations) Debug.LogWarning("Reached maximum iterations. Some conflicts may remain.");
        }

        /// <summary>
        ///     Try to swap a problematic tile with a safe tile
        /// </summary>
        private bool TrySwapWithSafeTile(Tile problematicTile)
        {
            // Loop through all tiles
            foreach (var tile in _tiles.Values)
            {
                // Skip if the tile is not safe
                if (!TileHelpers.IsSafeTile(tile)) continue;

                // Check if swapping would create a new conflict
                if (TileHelpers.WouldCreateConflict(problematicTile, tile)) continue;

                // Swap the numbers
                TileHelpers.SwapNumbers(problematicTile, tile);
                return true;
            }

            return false;
        }

        #endregion

        
        private IEnumerator SpawnBuildButtons()
        {
            foreach (var corner in _corners.Values)
            {
                if (corner.AdjacentTiles.All(t => t.Resource == CellType.Water)) continue;
                
                var buildPoint = InstantiateButton(corner.Position, BuildingType.Settlement);
                corner.AssignBuildPoint(buildPoint);
                yield return GeneralHelpers.GetWait(0.025f);

            }
            
            foreach (var vertex in _vertices.Values)
            {
                if (vertex.AdjacentTiles.All(t => t.Resource == CellType.Water)) continue;
                
                // Instantiate a build button at the vertex position
                var buildPoint = InstantiateButton(HexHelper.GetRoadPosition(vertex), BuildingType.Road,
                    HexHelper.GetRoadRotation(vertex));
                // Set the button on the vertex
                vertex.AssignBuildPoint(buildPoint);
                yield return GeneralHelpers.GetWait(0.025f);
            }

            foreach (var tile in _tiles.Values) tile.RefreshButtons();
            
            yield return null;
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

#if UNITY_EDITOR
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

        #region Testing

        [TabGroup("Testing")]
        [Button]
        private void HideAllButtons()
        {
            foreach (var corner in _corners.Values) corner.ToggleBuildPointVisibility(false);

            foreach (var vertex in _vertices.Values) vertex.ToggleBuildPointVisibility(false);
        }

        [TabGroup("Testing")]
        [Button]
        private void ToggleSettlementButtons()
        {
            foreach (var corner in _corners.Values) corner.ToggleBuildPointVisibility(!corner.BuildPointVisible());
        }

        [TabGroup("Testing")]
        [Button]
        private void ToggleRoadButtons()
        {
            foreach (var vertex in _vertices.Values) vertex.ToggleBuildPointVisibility(!vertex.BuildPointVisible());
        }

        [TabGroup("Testing")]
        [Button]
        private void ClearAndRun()
        {
            if (_isGeneratingBoard) return;

            foreach (var tile in _tiles.Values) Destroy(tile.gameObject);

            foreach (var vertex in _vertices.Values)
            {
                if (!vertex.BuildPoint) continue;
                Destroy(vertex.BuildPoint.gameObject);
            }

            foreach (var corner in _corners.Values)
            {
                if (!corner.BuildPoint) continue;
                Destroy(corner.BuildPoint.gameObject);
            }

            foreach (var num in _numberPieces) Destroy(num.gameObject);

            _tiles.Clear();
            _vertices.Clear();
            _corners.Clear();
            _numberPieces.Clear();

            StartCoroutine(GenerateBoard());
        }

        #endregion

#endif
    }
}