using System;
using Building;
using Building.Pieces;
using Interactions;
using Sirenix.OdinInspector;
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
    public class BuildingPoint : Interactable
    {
        [TabGroup("State")][SerializeField, ReadOnly] private Piece piece;
        public bool IsBuilt => piece != null;
        public Piece Piece => piece;
        
        [TabGroup("Data")] [SerializeField, ReadOnly] private Vector3 position;
        [TabGroup("Data")] [SerializeField, ReadOnly] private BuildingType buildingType;
        
        public event Action OnBuild;
        public event Action OnUpgrade;
        
        public override void Interact()
        {
            if (IsBuilt)
            {
                if (piece.Type != BuildingType.Settlement) return;
                
                Upgrade();
                OnUpgrade?.Invoke();
                return;
            }
            
            piece = BuildManager.Current.Build(buildingType, position, transform.rotation);
            OnBuild?.Invoke();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsHovered) return;
            
            Interact();
        }
        
        public void Initialize(Vector3 pos, BuildingType type)
        {
            position = pos;
            buildingType = type;
        }

        private void Upgrade()
        {
            var currentPiece = piece;

            piece = BuildManager.Current.Build(BuildingType.City, position, transform.rotation);
            
            Destroy(currentPiece.gameObject); 
        }
    }
}