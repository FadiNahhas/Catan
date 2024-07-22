using System.Collections.Generic;
using UnityEngine;

namespace Hex
{
    /// <summary>
    /// Class representing a vertex of a hexagon
    /// </summary>
    public class HexVertex : HexElement
    {
        public HexCorner Corner1 { get; private set; }
        public HexCorner Corner2 { get; private set; }
        
        public Vector3 Position1 => Corner1.Position;
        public Vector3 Position2 => Corner2.Position;
        
        public List<HexVertex> Neighbors { get; private set; }
        
        public HexVertex(HexCorner pos1, HexCorner pos2)
        {
            Corner1 = pos1;
            Corner2 = pos2;
            Neighbors = new List<HexVertex>();
        }
        
        public override void Update()
        {
            if (!CanBuild())
                ToggleBuildPointVisibility(false);
        }
        
        protected override void OnBuild()
        {
            base.OnBuild();
            Update();
        }

        public override bool CanBuild()
        {
            return !BuildPoint.IsBuilt;
        }
    }
}