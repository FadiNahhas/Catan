using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    /// <summary>
    /// Class representing a corner of a hexagon
    /// </summary>
    [Serializable]
    public class HexCorner
    {
        public Vector3 Position { get; private set; }
        
        public List<HexCorner> Neighbors { get; private set; }
        
        public List<Tile> AdjacentTiles { get; private set; }
        
        public BuildButton Button { get; private set; }

        public bool CanBuild { get; private set; }
        
        public bool ButtonsSpawned { get; private set; }

        public bool IsButtonVisible => Button.gameObject.activeSelf;
        
        public HexCorner(Vector3 position)
        {
            Position = position;
            Neighbors = new List<HexCorner>();
            AdjacentTiles = new List<Tile>();
        }

        public void SetCanBuild(bool value = true)
        {
            CanBuild = value;
            if (Button != null) Button.gameObject.SetActive(value);   
        }
        
        public void AssignButton(BuildButton button)
        {
            Button = button;
            Button.onBuild += OnBuild;
        }
        
        private void OnBuild()
        {
            Button.onBuild -= OnBuild;
            
            SetCanBuild(false);
            
            foreach (var neighbor in Neighbors)
            {
                neighbor.SetCanBuild(false);
            }
        }
        
        public void SetNeighbors(List<HexCorner> neighbors) => Neighbors = neighbors;
        
        public void SetButtonsSpawned(bool value = true) => ButtonsSpawned = value;
        
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