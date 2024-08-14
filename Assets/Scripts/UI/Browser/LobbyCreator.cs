using System;
using Sirenix.OdinInspector;
using TMPro;
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