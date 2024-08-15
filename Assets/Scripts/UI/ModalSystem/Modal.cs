using TMPro;
using UnityEngine;

namespace UI.ModalSystem
{
    public class Modal : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI content;
        
        public void SetTitle(string modal_title) => title.text = modal_title;
        public void SetContent(string modal_content) => content.text = modal_content;
        
        public void Close() => ModalManager.Hide();
    }
}