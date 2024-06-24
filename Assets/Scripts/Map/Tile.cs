using Helpers;
using Interactions;
using UnityEngine;

namespace Map
{
    public class Tile : Interactable
    {
        public TileData Data { get; private set; }

        [SerializeField] private CellType cellType;
        public int AssignedNumber { get; private set; }
        
        [field: SerializeField] public bool HasRobber { get; private set; }
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MeshCollider _meshCollider;
        
        public void Initialize(TileData data)
        {
            Data = data;
            transform.position = data.Position;
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshFilter.mesh = data.CreateMesh();
            
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = MaterialHelper.GetDefaultMaterial();
            
            _meshCollider = gameObject.AddComponent<MeshCollider>();
            _meshCollider.sharedMesh = _meshFilter.mesh;
        }
        
        public void SetType(CellType type)
        {
            cellType = type;
            _meshRenderer.material = MaterialHelper.GetMaterial(HexHelper.GetColor(cellType));
        }
        
        public void SetNumber(int number)
        {
            AssignedNumber = number;
        }
    }
}