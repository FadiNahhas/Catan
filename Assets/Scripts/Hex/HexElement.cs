using System;
using System.Collections.Generic;
using System.Linq;
using Map;

namespace Hex
{
    [Serializable]
    public abstract class HexElement
    {
        public BuildingPoint BuildPoint { get; private set; }
        public bool BuildPointVisible => BuildPoint.gameObject.activeSelf;
        
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
            if (BuildPoint == null) return;
            
            BuildPoint.gameObject.SetActive(IsInPlayableArea && value);
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

        public abstract void Update();
    }
}