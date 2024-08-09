using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactions
{
    public abstract class Interactable : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [TabGroup("Materials")][SerializeField] protected Material defaultMaterial;
        [TabGroup("Materials")][SerializeField] protected Material hoverMaterial;
        [TabGroup("Components"), SerializeField, ReadOnly] protected MeshRenderer meshRenderer;
        
        private bool isHovered;
        
        public bool IsHovered
        {
            get => isHovered;
            private set
            {
                if (isHovered == value) return;
                isHovered = value;
                OnHoverUpdated();
            }
        }

        protected virtual void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public abstract void Interact();
        
        public virtual void OnPointerEnter(PointerEventData event_data)
        {
            IsHovered = true;
        }

        public virtual void OnPointerExit(PointerEventData event_data)
        {
            IsHovered = false;
        }

        public virtual void OnPointerClick(PointerEventData event_data)
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