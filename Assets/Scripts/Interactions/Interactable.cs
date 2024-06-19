using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactions
{
    public class Interactable : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool IsHovered { get; private set; }
        
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
    }
}