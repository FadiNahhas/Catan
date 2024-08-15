using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ModalSystem
{
    public class PasswordModal : MonoBehaviour
    {
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button joinButton;
        public event Action<string> OnJoin;
        [SerializeField] private Button cancelButton;
        public event Action OnCancel;

        #region Unity Events

        private void OnEnable()
        {
            joinButton.onClick.AddListener(Join);
            cancelButton.onClick.AddListener(Cancel);
        }

        private void OnDisable()
        {
            joinButton.onClick.RemoveListener(Join);
            cancelButton.onClick.RemoveListener(Cancel);
        }

        #endregion

        #region Button Handlers

        private void Join()
        {
            OnJoin?.Invoke(passwordInput.text);
        }

        private void Cancel()
        {
            OnCancel?.Invoke();
        }

        #endregion
    }
}