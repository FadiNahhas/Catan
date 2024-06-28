using System.Collections.Generic;
using Building.Pieces;
using DG.Tweening;
using Helpers;
using Hex;
using Interactions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Board
{
    public class Tile : Interactable
    {
        public HexTile Data { get; private set; }
        [TabGroup("Data")][SerializeField] private List<Tile> neighbours;
        public List<Tile> Neighbours => neighbours;
        [TabGroup("State")][SerializeField] private CellType cellType;
        public CellType Resource => cellType;
        
        [TabGroup("State")][SerializeField] private Number number;
        public Number Number => number;
        
        [field: TabGroup("State"), SerializeField, ReadOnly] public bool HasRobber { get; private set; }
        
        [TabGroup("Components")][SerializeField][ReadOnly] private MeshFilter meshFilter;
        [TabGroup("Components")][SerializeField][ReadOnly] private MeshCollider meshCollider;

        protected override void Awake()
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        public void Initialize(HexTile data)
        {
            Data = data;
            neighbours = new List<Tile>();
            meshFilter.mesh = data.CreateMesh();
            meshRenderer.material = MaterialHelper.GetDefaultMaterial();
            meshCollider.sharedMesh = meshFilter.mesh;
            
            foreach (var vertex in Data.Vertices)
            {
                vertex.AddTile(this);
            }
            
            foreach (var corner in Data.Corners)
            {
                corner.AddTile(this);
            }
        }
        
        public void SetType(CellType type)
        {
            cellType = type;
            meshRenderer.material = MaterialHelper.GetMaterial(HexHelper.GetColor(cellType));
        }

        public void RefreshButtons()
        {
            foreach (var vertex in Data.Vertices)
            {
                vertex.OnMapGenerated();
            }

            foreach (var corner in Data.Corners)
            {
                corner.OnMapGenerated();
            }
        }

        public void AssignNumber(Number num)
        {
            number = num;

            if (num != null)
            {
                num.transform.SetParent(transform);
                num.transform.DOMove(Data.Position, 0.5f).SetEase(Ease.InOutCubic);
            }
        }
        
        public void AddNeighbour(Tile tile)
        {
            if (neighbours.Contains(tile)) return;
            
            neighbours.Add(tile);
        }

        public override void Interact() {}
        
        public int GetNumber()
        {
            return !number ? 0 : number.Value;
        }
    }
}