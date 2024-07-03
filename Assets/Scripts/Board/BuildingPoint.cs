using System;
using Building;
using Building.Pieces;
using DG.Tweening;
using Interactions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Board
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
        
        [TabGroup("Components")] [SerializeField] private Collider buttonCollider; 
        private static readonly Vector3 TargetScale = new(0.3f, 0.3f, 0.3f);
        private const float ScaleDuration = 0.75f;
        private const Ease ScaleEase = Ease.OutBack;
        
        public event Action OnBuild;
        public event Action OnUpgrade;
        
        public bool IsVisible { get; private set; }
        
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

            Show();
        }

        public void Show()
        {
            transform.DOScale(TargetScale, ScaleDuration).SetEase(ScaleEase);
            buttonCollider.enabled = true;
            IsVisible = true;
        }

        public void Hide()
        {
            transform.DOScale(Vector3.zero, ScaleDuration).SetEase(ScaleEase); 
            buttonCollider.enabled = false;
            IsVisible = false;
        }

        public void Hide(Action callback)
        {
            transform.DOScale(Vector3.zero, ScaleDuration).SetEase(ScaleEase).OnComplete(() => callback?.Invoke());
            buttonCollider.enabled = false;
            IsVisible = false;
        }
        
        private void Upgrade()
        {
            var currentPiece = piece;

            piece = BuildManager.Current.Build(BuildingType.City, position, transform.rotation);
            
            Hide(() =>Destroy(currentPiece.gameObject)); 
        }
    }
}