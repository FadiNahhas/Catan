using UnityEngine;
using UnityEngine.UIElements;

namespace UI.ModalSystem
{
    public enum PasswordModalResponse
    {
        None,
        Join,
        Cancel
    }
    
    public static class PasswordModalSystem
    {
        private static readonly PasswordModal Prefab;
        private static PasswordModal _instance;

        public static PasswordModalResponse Response { get; private set; } = PasswordModalResponse.None;
        public static string Password { get; private set; }
        static PasswordModalSystem()
        {
            Prefab = Resources.Load<PasswordModal>("Prefabs/PasswordModal");
        }

        public static void GetPassword()
        {
            var canvas =  Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No canvas found in scene");
                return;
            }
            
            _instance = Object.Instantiate(Prefab, canvas.transform);
            _instance.OnJoin += OnJoin;
            _instance.OnCancel += OnCancel;
        }
        
        private static void OnJoin(string password)
        {
            Password = password;
            Response = PasswordModalResponse.Join;
        }
        
        private static void OnCancel()
        {
            Response = PasswordModalResponse.Cancel;
            Close();
        }

        public static void Close()
        {
            if (_instance == null)
                return;

            Object.Destroy(_instance.gameObject);
            _instance = null;
        }

        public static void ResetResponse()
        {
            Response = PasswordModalResponse.None;
            Password = null;
        }
    }
}