using System;
using Network;
using Sirenix.OdinInspector;
using TMPro;
using UI.ModalSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Browser
{
    public class LobbyCreator : MonoBehaviour
    {
        [TabGroup("References"), SerializeField] private TMP_InputField nameInputField;
        [TabGroup("References"), SerializeField] private TMP_InputField passwordInputField;
        [TabGroup("References"), SerializeField] private Slider maxPlayersSlider;
        [TabGroup("References"), SerializeField] private Button createButton;
        [TabGroup("References"), SerializeField] private Button cancelButton;

        [TabGroup("Dependencies"), SerializeField] private GameBrowser gameBrowser;

        private void OnEnable()
        {
            createButton.onClick.AddListener(CreateLobby);
            cancelButton.onClick.AddListener(Cancel);
        }
        
        private void OnDisable()
        {
            createButton.onClick.RemoveListener(CreateLobby);
            cancelButton.onClick.RemoveListener(Cancel);
        }

        private void CreateLobby()
        {
            if (nameInputField.text.Length == 0)
            {
                ModalManager.Show("Error", "Please enter a name for the lobby.");
                return;
            }
            
            LobbyManager.Instance.CreateLobby(nameInputField.text,
                (int)maxPlayersSlider.value, passwordInputField.text);
        }

        private void Cancel()
        {
            Clear();
            gameBrowser.CancelLobbyCreation();
        }

        private void Clear()
        {
            nameInputField.text = "";
            passwordInputField.text = "";
        }
        
    }
}