using System;
using Building;
using Interactions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Map
{
    [Serializable]
    public enum BuildingType
    {
        Settlement,
        City,
        Road
    }
    public class BuildButton : Interactable
    {
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material hoverMaterial;
        
        private Vector3 _position;
        private BuildingType _buildingType;
        
        private MeshRenderer _meshRenderer;

        public event Action onBuild;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public override void Interact()
        {
            BuildManager.Current.Build(_buildingType, _position,transform.rotation);
            onBuild?.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsHovered) return;
            
            Interact();
        }

        protected override void OnHoverUpdated()
        {
            _meshRenderer.material = IsHovered ? hoverMaterial : defaultMaterial;
        }
        
        public void Initialize(Vector3 position, BuildingType buildingType)
        {
            _position = position;
            _buildingType = buildingType;
        }
    }
}