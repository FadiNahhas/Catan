using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// Class representing a vertex of a hexagon
    /// </summary>
    [Serializable]
    public class HexVertex
    {
        public HexCorner Corner1 { get; private set; }
        public HexCorner Corner2 { get; private set; }
        
        public Vector3 Position1 => Corner1.Position;
        public Vector3 Position2 => Corner2.Position;

        public BuildButton Button { get; private set; }
        
        public bool IsButtonVisible => Button.gameObject.activeSelf;

        public bool CanBuild { get; private set; }
        
        public List<HexVertex> Neighbors { get; private set; }
        
        public List<Tile> AdjacentTiles { get; private set; }
        
        public HexVertex(HexCorner pos1, HexCorner pos2)
        {
            Corner1 = pos1;
            Corner2 = pos2;
            Neighbors = new List<HexVertex>();
            AdjacentTiles = new List<Tile>();
        }
        
        public void AssignButton(BuildButton button)
        {
            Button = button;
            Button.onBuild += OnBuild;
        }

        public void SetCanBuild(bool value = true)
        {
            CanBuild = value;
            if (Button != null) Button.gameObject.SetActive(value);   
        }
        
        private void OnBuild()
        {
            Button.onBuild -= OnBuild;
            
            SetCanBuild(false);
        }
        
        public void ToggleButtonVisibility(bool value = true)
        {
            if (!CanBuild) return;
            Button.gameObject.SetActive(value);
        }
        
        public void AddTile(Tile tile)
        {
            AdjacentTiles.Add(tile);
        }

        public void OnMapGenerated()
        {
            SetCanBuild(AdjacentTiles.Any(t => t.Resource != CellType.Water));
        }
    }
}