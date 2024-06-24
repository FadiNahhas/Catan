using System;
using System.Collections.Generic;
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
        
        public bool ButtonSpawned { get; private set; }
        
        public List<HexVertex> Neighbors { get; private set; }
        
        public HexVertex(HexCorner pos1, HexCorner pos2)
        {
            Corner1 = pos1;
            Corner2 = pos2;
            Neighbors = new List<HexVertex>();
        }
        
        public void SetNeighbors(List<HexVertex> neighbors) => Neighbors = neighbors;
        
        public void SetButton(BuildButton button)
        {
            Button = button;
            Button.onBuild += OnBuild;
        }

        public void HideButton()
        {
            if (Button == null) return;
            Button.gameObject.SetActive(false);
        }

        private void OnBuild()
        {
            Button.onBuild -= OnBuild;
            Neighbors.ForEach(n => n.HideButton());
        }
    }
}