using System.Collections.Generic;
using System.Linq;
using Board;
using UnityEngine;

namespace Hex
{
    /// <summary>
    /// Class representing a corner of a hexagon
    /// </summary>
    public class HexCorner : HexElement
    {
        public Vector3 Position { get; private set; }
        public List<HexCorner> Neighbors { get; private set; }
        public HexCorner(Vector3 position)
        {
            Position = position;
            Neighbors = new List<HexCorner>();
        }
        
        public void SetNeighbors(List<HexCorner> neighbors) => Neighbors = neighbors;
        public void AddNeighbor(HexCorner neighbor) => Neighbors.Add(neighbor);
        
        public override void Update()
        {
            ToggleBuildPointVisibility(CanBuild() || CanUpgrade());
            
            if (CanUpgrade())
                BuildPoint.OnUpgrade += OnUpgrade;
        }

        protected override void OnBuild()
        {
            base.OnBuild();
            Update();
            foreach (var n in Neighbors)
            {
                n.Update();
            }
        }

        private void OnUpgrade()
        {
            BuildPoint.OnUpgrade -= OnUpgrade;
            Update();
        }

        private bool CanUpgrade()
        {
            if (!BuildPoint) return false;
            
            if (!BuildPoint.IsBuilt) return false;
            
            return BuildPoint.Piece.Type != BuildingType.City;
        }

        private bool CanBuild()
        {
            var buildableNeighbours = Neighbors.Where(n => n.BuildPoint).ToList();
            
            return !(buildableNeighbours.Any(n => n.BuildPoint.IsBuilt) || BuildPoint.IsBuilt);
        }
    }
}