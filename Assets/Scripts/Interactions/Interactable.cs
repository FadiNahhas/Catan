using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactions
{
    public abstract class Interactable : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] protected Material defaultMaterial;
        [SerializeField] protected Material hoverMaterial;
        [SerializeField, ReadOnly] private MeshRenderer meshRenderer;
        
        private bool _isHovered;
        
        public bool IsHovered
        {
            get => _isHovered;
            private set
            {
                if (_isHovered == value) return;
                _isHovered = value;
                OnHoverUpdated();
            }
        }

        protected virtual void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public abstract void Interact();
        
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!IsHovered) return;
            
            Interact();
        }

        protected virtual void OnHoverUpdated()
        {
            if (!defaultMaterial || !hoverMaterial) return;
            
            meshRenderer.material = IsHovered ? hoverMaterial : defaultMaterial;
        }
    }
}