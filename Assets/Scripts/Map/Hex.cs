using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public enum CellType
    {
        Empty,
        Wood,
        Stone,
        Ore,
        Grain,
        Wool,
        Water
    }
    
    public class HexVertex
    {
        public Vector3 Position { get; private set; }
        
        public HexVertex(Vector3 position)
        {
            Position = position;
        }
    }

    public class HexCell
    {
        public Vector3 Position { get; private set; }
        public List<HexVertex> Vertices { get; private set; }
        
        public int Q { get; private set; }
        public int R { get; private set; }
        
        public CellType Type { get; private set; }
        
        public bool HasRobber { get; private set; }
        
        public HexCell(int q, int r, Vector3 position)
        {
            Position = position;
            Q = q;
            R = r;
            Vertices = new List<HexVertex>();
        }
    }
}