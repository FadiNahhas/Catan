using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Map
{
    public class HexGrid : MonoBehaviour
    {
        [TabGroup("Board Visuals")][SerializeField] private int gridRadius = 3;
        [TabGroup("Board Visuals")][SerializeField] private float hexSize = 1f;
        [TabGroup("Board Visuals")][SerializeField] private float hexThickness = 0.1f;
        
        [TabGroup("Map Configuration")][SerializeField] private MapConfiguration mapConfiguration;
        
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
            List<CellType> shuffledList = HexHelper.GenerateCells(mapConfiguration);
            
            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
                int r2 = Mathf.Min(gridRadius, -q + gridRadius);
                for (int r = r1; r <= r2; r++)
                {
                    var position = HexToPosition(q, r);
                    CellType type;
                    if (q == -gridRadius || q == gridRadius || r == r1 || r == r2)
                    {
                        // If it is, set as water cell
                        type = CellType.Water;
                    }
                    else
                    {
                        // If it's not, choose a random CellType
                        if (shuffledList.Count > 0)
                        {
                            type = shuffledList[0];
                            Debug.Log(type.ToString());
                            shuffledList.RemoveAt(0);
                        }
                        else
                        {
                            type = CellType.Water;
                        }
                    }
                    
                    var cell = new HexCell(q, r, position, type);
                    _cells.Add((q, r), cell);
                    
                    var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
                    {
                        color = HexHelper.GetColor(cell.Type)
                    };
                    CreateVertices(cell);
                    cell.ApplyTexture(material, hexThickness);
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
