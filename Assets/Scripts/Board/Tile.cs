using System.Collections.Generic;
using Building.Pieces;
using DG.Tweening;
using Helpers;
using Hex;
using Interactions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Board
{
    public class Tile : Interactable
    {
        public HexTile Data { get; private set; }
        [TabGroup("Data")][SerializeField] private List<Tile> neighbours;
        public List<Tile> Neighbours => neighbours;
        [TabGroup("State")][SerializeField] private CellType cellType;
        public CellType Resource => cellType;
        
        [TabGroup("State")][SerializeField] private Number number;
        public Number Number => number;
        
        public bool HasNumber => number != null;
        
        
        [field: TabGroup("State"), SerializeField, ReadOnly] public bool HasRobber { get; private set; }


        protected override void Awake()
        {
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            neighbours = new List<Tile>();
        }
        
        /// <summary>
        /// Initialize the tile with the given data
        /// </summary>
        /// <param name="data">The data to initialize the tile with</param>
        public void Init(HexTile data)
        {
            Data = data;
            
            // Add tile reference to all of its vertices and corners
            foreach (var vertex in Data.Vertices)
            {
                vertex.AddTile(this);
            }
            
            foreach (var corner in Data.Corners)
            {
                corner.AddTile(this);
            }
        }
        
        /// <summary>
        /// Set the resource type of the tile and update the material
        /// </summary>
        /// <param name="type">The type of the resource</param>
        public void SetType(CellType type)
        {
            Debug.Log($"Setting type of {name} to {type}");
            cellType = type;

            var materials = meshRenderer.materials;
            materials[1] = MaterialHelper.GetMaterial(HexHelper.GetColor(cellType));
            
            meshRenderer.materials = materials;
        }

        /// <summary>
        /// Refresh the build buttons of the tile on map generation
        /// </summary>
        public void RefreshButtons()
        {
            foreach (var vertex in Data.Vertices)
            {
                vertex.OnMapGenerated();
            }

            foreach (var corner in Data.Corners)
            {
                corner.OnMapGenerated();
            }
        }

        /// <summary>
        /// Assign a number to the tile
        /// </summary>
        /// <param name="num">Number to assign</param>
        public void AssignNumber(Number num)
        {
            number = num;

            if (!HasNumber) return;
            
            num.transform.SetParent(transform);
            num.transform.DOMove(Data.Position, 0.5f).SetEase(Ease.InOutCubic);
        }
        
        /// <summary>
        /// Add neighbour to the tile
        /// </summary>
        /// <param name="tile">Neighbour to add</param>
        public void AddNeighbour(Tile tile)
        {
            if (neighbours.Contains(tile)) return;
            
            neighbours.Add(tile);
        }

        public override void Interact() {}
    }
}