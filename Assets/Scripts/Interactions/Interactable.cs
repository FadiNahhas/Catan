using UnityEngine;
using UnityEngine.EventSystems;

namespace Interactions
{
    public class Interactable : MonoBehaviour, IInteractable, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public virtual void Interact()
        {
            
        }
        
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}