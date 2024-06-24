using System;
using System.Collections.Generic;
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
        
        public bool ButtonsSpawned { get; private set; }
        
        public HexCorner(Vector3 position)
        {
            Position = position;
            Neighbors = new List<HexCorner>();
        }
        
        public void SetNeighbors(List<HexCorner> neighbors) => Neighbors = neighbors;
        
        public void SetButtonsSpawned(bool value = true) => ButtonsSpawned = value;
    }
}