using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Board.Configuration;
using Board.Pieces;
using DG.Tweening;
using Helpers;
using Hex;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board.Generation
{
    /// <summary>
    ///     Class responsible for generating the game board
    /// </summary>
    public class BoardGenerator : Singleton<BoardGenerator>
    {
        #region Constants

        public const float HexThickness = 0.1f; // Thickness of the hexagon
        private const int MaxIterations = 100; // Maximum number of iterations for swapping numbers
        public const int ProblematicNumber1 = 6;
        public const int ProblematicNumber2 = 8;
        private static readonly Vector3 NumberStackYOffset = new(0, 0.1f, 0); // Vertical offset for the number pile
        private static readonly Vector3 TilePositionYOffset = new(0, HexThickness, 0); // Tile offset off the table

        #endregion

        #region Visuals
        
        [TabGroup("Board Visuals")] [SerializeField]
        private int gridRadius = 3; // How many cells from the center to generate

        [TabGroup("Board Visuals")] [SerializeField]
        private float hexSize = 1f; // Size of the hexagon

        [TabGroup("Board Visuals")] [SerializeField]
        private Vector3 numberStackSpawnPosition; // Where to spawn the number pile

        [TabGroup("Board Visuals")] [SerializeField]
        private Vector3 tileStackSpawnPosition; // Where to spawn the tile pile
        
        #endregion

        #region Configuration

        [TabGroup("Configuration")] [SerializeField]
        private MapConfiguration mapConfiguration; // Holds data for number of tiles of each type

        [TabGroup("Configuration")] [SerializeField]
        private NumbersConfiguration numbersConfiguration; // Holds data for number of each number piece

        [TabGroup("Configuration")] [SerializeField] [Unit(Units.Second)]
        private float tilePlacementDelay = 0.1f; // Delay for positioning tiles

        [TabGroup("Configuration")] [SerializeField] [Unit(Units.Second)]
        private float numberPlacementDelay = 0.1f; // Delay for assigning numbers

        [TabGroup("Configuration")] [SerializeField] [Unit(Units.Second)]
        private float numberSwapDelay = 0.1f; // Delay for swapping numbers
        
        [TabGroup("Configuration")] [SerializeField]
        private Transform tileParent; // Parent object for the tiles

        #endregion

        #region Prefabs

        [TabGroup("Prefabs")] [SerializeField] private BuildingPoint buildingPointPrefab;
        [TabGroup("Prefabs")] [SerializeField] private Number numberPiecePrefab;
        [TabGroup("Prefabs")] [SerializeField] private Tile tilePrefab;

        #endregion

        #region Data

        private Dictionary<(int, int), Tile> _tiles; // Dictionary of tiles based on their coordinates
        private Dictionary<Vector3, HexVertex> _vertices; // Dictionary of vertices based on a unique position generated using vertex's start and end points
        private Dictionary<Vector3, HexCorner> _corners; // Dictionary of corners based on their position
        private List<Number> _numberPieces; // All number pieces in the game
        private bool _isGeneratingBoard;
        
        #endregion

        #region Unity Events

        /// <summary>
        /// Initialize dictionaries and lists when the script is enabled
        /// </summary>
        private void OnEnable()
        {
            _tiles = new Dictionary<(int, int), Tile>();
            _vertices = new Dictionary<Vector3, HexVertex>();
            _corners = new Dictionary<Vector3, HexCorner>();
            _numberPieces = new List<Number>();
        }

        /// <summary>
        /// Temporary method to start generating the board
        /// </summary>
        private void Start() => StartCoroutine(GenerateBoard());

        #endregion

        #region Board Functions

        /// <summary>
        ///    Generates the game board by instantiating tiles, numbers,
        /// setting up resources, positioning tiles, assigning numbers, and building buttons.
        /// </summary>
        private IEnumerator GenerateBoard()
        {
            // Check if the board is already being generated
            if (_isGeneratingBoard) yield break;
            
            _isGeneratingBoard = true;

            InstantiateTiles();

            InstantiateNumbers();

            GenerateResources();

            yield return PositionTiles();

            yield return GeneralHelpers.GetWait(tilePlacementDelay);

            yield return AssignNumbers();

            yield return GeneralHelpers.GetWait(numberSwapDelay);

            yield return SwapProblematicTiles();

            yield return InstantiateBuildButtons();

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

        #endregion

        #region Instantiation Methods
        
        /// <summary>
        /// Instantiates the board tiles stack
        /// </summary>
        private void InstantiateTiles()
        {
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

                    // Generate tile vertices
                    HexHelper.CreateVertices(tileData, hexSize, _vertices, _corners);

                    // Instantiate a new tile object at the calculated stack position
                    var tile = Instantiate(tilePrefab,
                        tileStackSpawnPosition + new Vector3(0f, 0f + HexThickness * _tiles.Count, 0f), Quaternion.identity);
                    
                    tile.gameObject.name = $"Tile ({q}, {r})";

                    // Set the parent of the tile to the HexGrid object
                    tile.transform.SetParent(tileParent);

                    // Add the tile to the dictionary and initialize it
                    _tiles.Add((q, r), tile);
                    tile.Init(tileData);
                }
            }
        }
        
        /// <summary>
        ///     Instantiates the pile of shuffled number pieces
        /// </summary>
        private void InstantiateNumbers()
        {
            // Generate numbers based on the configuration
            var shuffledNumbers = HexHelper.GenerateNumbers(numbersConfiguration);
            var count = shuffledNumbers.Count;

            // Loop through the shuffled numbers and instantiate a number piece for each
            for (var i = 0; i < count; i++)
            {
                var num = Instantiate(numberPiecePrefab, numberStackSpawnPosition + NumberStackYOffset * i,
                    Quaternion.identity);
                num.SetValue(shuffledNumbers[0], NumbersHelper.GetProbability(shuffledNumbers[0]));
                _numberPieces.Add(num);

                shuffledNumbers.RemoveAt(0);
            }

            // Reverse the list so that the last number is on top of the stack
            _numberPieces.Reverse();
        }
        
        /// <summary>
        /// Instantiates the build buttons for settlements and roads
        /// </summary>
        private IEnumerator InstantiateBuildButtons()
        {
            foreach (var corner in _corners.Values)
            {
                if (corner.AdjacentTiles.All(t => t.Resource == CellType.Water)) continue;
                
                var buildPoint = InstantiateButton(corner.Position, BuildingType.Settlement);
                corner.AssignBuildPoint(buildPoint);

            }
            
            foreach (var vertex in _vertices.Values)
            {
                if (vertex.AdjacentTiles.All(t => t.Resource == CellType.Water)) continue;
                
                // Instantiate a build button at the vertex position
                var buildPoint = InstantiateButton(HexHelper.GetRoadPosition(vertex), BuildingType.Road,
                    HexHelper.GetRoadRotation(vertex));
                // Set the button on the vertex
                vertex.AssignBuildPoint(buildPoint);
            }

            foreach (var tile in _tiles.Values) tile.RefreshButtons();
            
            yield return null;
        }

        private BuildingPoint InstantiateButton(Vector3 pos, BuildingType type, Quaternion rotation = default)
        {
            if (rotation == default)
            {
                rotation = Quaternion.identity;
            }
            
            var obj = Instantiate(buildingPointPrefab, pos, rotation);
            obj.Initialize(pos, type);
            obj.transform.SetParent(transform);
            return obj;
        }
        
        #endregion

        #region Board Animation Methods

        /// <summary>
        ///     Loops through the pile of hex tiles and moves them to their final position
        /// </summary>
        private IEnumerator PositionTiles()
        {
            for (var i = _tiles.Count - 1; i >= 0; i--)
            {
                var tile = _tiles.Values.ElementAt(i);
                
                tile.transform.DOMove(tile.Data.Position - TilePositionYOffset, 0.3f)
                    .SetEase(Ease.InOutCubic).OnComplete(tile.SpawnAndAnimateProps);
                yield return GeneralHelpers.GetWait(tilePlacementDelay);
            }
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


        #endregion

        #region Number Assignment Methods

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
        private bool TrySwapWithSafeTile(Tile problematic_tile)
        {
            // Loop through all tiles
            foreach (var tile in _tiles.Values)
            {
                // Skip if the tile is not safe
                if (!TileHelpers.IsSafeTile(tile)) continue;

                // Check if swapping would create a new conflict
                if (TileHelpers.WouldCreateConflict(problematic_tile, tile)) continue;

                // Swap the numbers
                TileHelpers.SwapNumbers(problematic_tile, tile);
                return true;
            }

            return false;
        }

        #endregion
        

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