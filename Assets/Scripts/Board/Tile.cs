using Building.Pieces;
using DG.Tweening;
using Helpers;
using Hex;
using Interactions;
using UnityEngine;

namespace Board
{
    public class Tile : Interactable
    {
        public HexTile Data { get; private set; }

        [SerializeField] private CellType cellType;
        public CellType Resource => cellType;
        private Number _number;
        public int Number => _number.Value;
        
        [field: SerializeField] public bool HasRobber { get; private set; }
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;

        protected override void Awake()
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        public void Initialize(HexTile data)
        {
            Data = data;
            //_meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshFilter.mesh = data.CreateMesh();
            
            //_meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = MaterialHelper.GetDefaultMaterial();
            
            //_meshCollider = gameObject.AddComponent<MeshCollider>();
            _meshCollider.sharedMesh = _meshFilter.mesh;
            
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
            _meshRenderer.material = MaterialHelper.GetMaterial(HexHelper.GetColor(cellType));
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

        public void AssignNumber(Number number)
        {
            _number = number;
            number.transform.SetParent(transform);
            number.transform.DOMove(Data.Position, 0.5f).SetEase(Ease.InOutCubic);
        }

        public override void Interact()
        {
            
        }
    }
}