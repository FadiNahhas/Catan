using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactions
{
    public class Interactable : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
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

        public virtual void Interact()
        {
            
        }
        
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
            
        }
        
        protected virtual void OnHoverUpdated(){}
    }
}