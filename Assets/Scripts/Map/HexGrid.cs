using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class HexGrid : MonoBehaviour
    {
        [SerializeField] private int gridRadius = 3;
        [SerializeField] private float hexSize = 1f;

        private Dictionary<(int, int), HexCell> _cells;
        private Dictionary<Vector3, HexVertex> _vertices;
        
        void Start()
        {
            _cells = new Dictionary<(int, int), HexCell>();
            _vertices = new Dictionary<Vector3, HexVertex>();
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
                int r2 = Mathf.Min(gridRadius, -q + gridRadius);
                for (int r = r1; r <= r2; r++)
                {
                    var position = HexToPosition(q, r);
                    var cell = new HexCell(q, r, position);
                    _cells.Add((q, r), cell);
                    CreateVertices(cell);
                }
            }
        }
        
        private Vector3 HexToPosition(int q, int r)
        {
            var z = hexSize * (Mathf.Sqrt(3f) * (r + q / 2f));
            var x = hexSize * (3f / 2f * q);
            return new Vector3(x, 0, z);
        }

        private void CreateVertices(HexCell cell)
        {
            Vector3[] corners = GetHexCorners(cell.Position);

            foreach (Vector3 corner in corners)
            {
                HexVertex vertex = FindOrCreateVertex(corner);
                cell.Vertices.Add(vertex);
            }
        }

        private Vector3[] GetHexCorners(Vector3 center)
        {
            Vector3[] corners = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = 60 * i;
                float angleRad = Mathf.Deg2Rad * angle;
                corners[i] = new Vector3(center.x + hexSize * Mathf.Cos(angleRad),
                    0f,
                    center.z + hexSize * Mathf.Sin(angleRad));
            }

            return corners;
        }

        private HexVertex FindOrCreateVertex(Vector3 position)
        {
            if (_vertices.ContainsKey(position)) return _vertices[position];
            
            HexVertex vertex = new HexVertex(position);
            _vertices.Add(position, vertex);
            return _vertices[position];
        }

        private void OnDrawGizmos()
        {
            if (_cells == null || _vertices == null) return;
            
            foreach (var cell in _cells.Values)
            {
                // Draw cell center
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(cell.Position, 0.1f);
                
                Vector3[] corners = GetHexCorners(cell.Position);
                for (int i = 0; i < 6; i++)
                {
                    // Draw cell edges
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(corners[i], corners[(i + 1) % 6]);
                    
                    // Draw road building spots
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere((corners[i] + corners[(i + 1) % 6])/2, 0.05f);
                }
            }
            
            // Draw settlement building spots
            Gizmos.color = Color.green;
            foreach (var vertex in _vertices.Values)
            {
                Gizmos.DrawSphere(vertex.Position, 0.05f);
            }
            
        }
    }
}
