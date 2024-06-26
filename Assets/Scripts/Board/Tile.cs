using Helpers;
using Hex;
using Interactions;
using UnityEngine;

namespace Map
{
    public class Tile : Interactable
    {
        public HexTile Data { get; private set; }

        [SerializeField] private CellType cellType;
        public CellType Resource => cellType;
        public int AssignedNumber { get; private set; }
        
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
            transform.position = data.Position;
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
            
            foreach (var vertex in Data.Vertices)
            {
                vertex.OnMapGenerated();
            }
            
            foreach (var corner in Data.Corners)
            {
                corner.OnMapGenerated();
            }
        }
        
        public void SetNumber(int number)
        {
            AssignedNumber = number;
        }

        public override void Interact()
        {
            
        }
    }
}