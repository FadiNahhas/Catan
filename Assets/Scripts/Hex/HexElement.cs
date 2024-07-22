using System;
using System.Collections.Generic;
using System.Linq;
using Board;

namespace Hex
{
    [Serializable]
    public abstract class HexElement
    {
        public BuildingPoint BuildPoint { get; private set; }
        
        public bool IsInPlayableArea { get; private set; }
        
        public List<Tile> AdjacentTiles { get; private set; } = new();
        
        public virtual void AssignBuildPoint(BuildingPoint buildPoint)
        {
            BuildPoint = buildPoint;
            BuildPoint.OnBuild += OnBuild;
        }

        protected virtual void OnBuild()
        {
            BuildPoint.OnBuild -= OnBuild;
        }
        
        public virtual void ToggleBuildPointVisibility(bool value = true)
        {
            if (!BuildPoint) return;

            if (IsInPlayableArea && value && CanBuild())
                BuildPoint.Show();
            else
                BuildPoint.Hide();
        }   
        
        public void AddTile(Tile tile)
        {
            if (AdjacentTiles.Contains(tile)) return;
            
            AdjacentTiles.Add(tile);
        }

        public virtual void OnMapGenerated()
        {
            IsInPlayableArea = AdjacentTiles.Any(t => t.Resource != CellType.Water);

            if (!IsInPlayableArea)
                ToggleBuildPointVisibility(false);
        }

        public bool BuildPointVisible()
        {
            if (!BuildPoint) return false;

            return BuildPoint.IsVisible;
        }

        public abstract void Update();
        
        public abstract bool CanBuild();
    }
}