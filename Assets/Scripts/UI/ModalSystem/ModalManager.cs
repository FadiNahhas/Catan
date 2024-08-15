using UnityEngine;

namespace UI.ModalSystem
{
    public static class ModalManager
    {
        private static readonly Modal Prefab;
        private static Modal _currentInstance;
        static ModalManager()
        {
            Prefab = Resources.Load<Modal>("Prefabs/Modal");
            if (Prefab == null)
            {
                Debug.LogError("Modal prefab not found in Resources/Prefabs/Modal");
            }
        }
        
        public static void Show(string title, string content)
        {
            var canvas = Object.FindObjectOfType<Canvas>();
            if (_currentInstance != null)
            {
                Object.Destroy(_currentInstance.gameObject);
            }
            
            _currentInstance = Object.Instantiate(Prefab, canvas.transform);
            _currentInstance.SetTitle(title);
            _currentInstance.SetContent(content);
        }

        public static void Hide()
        {
            if (_currentInstance != null)
            {
                Object.Destroy(_currentInstance.gameObject);
            }
        }
    }
}