using System;
using Sirenix.OdinInspector;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Browser
{
    public class GameBrowserItem : MonoBehaviour
    {
        [TabGroup("References")][SerializeField] private Image bg;
        [TabGroup("References")][SerializeField] private TextMeshProUGUI gameName;
        [TabGroup("References")][SerializeField] private TextMeshProUGUI playerCount;
        [TabGroup("References")][SerializeField] private Image hasPassword;
        [TabGroup("References")][SerializeField] private Button selectButton;
        
        [TabGroup("Visuals")][SerializeField] private Sprite selectedSprite;
        [TabGroup("Visuals")][SerializeField] private Sprite unselectedSprite;
        
        public CSteamID LobbyId { get; private set; }
        private Action<GameBrowserItem> _onSelect;

        private void OnEnable()
        {
            selectButton.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            selectButton.onClick.RemoveListener(OnClick);            
        }

        public void Init(string game_name, int player_count, int max_players, bool has_password, CSteamID lobby_id, Action<GameBrowserItem> on_select)
        {
            gameName.text = game_name;
            playerCount.text = $"{player_count}/{max_players}";
            hasPassword.enabled = has_password;
            LobbyId = lobby_id;
            _onSelect = on_select;
        }

        public void Select()
        {
            bg.sprite = selectedSprite;
        }

        public void Deselect()
        {
            bg.sprite = unselectedSprite;
        }

        private void OnClick()
        {
            _onSelect.Invoke(this);
        }
    }
}