using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Browser
{
    public class GameBrowserItem : MonoBehaviour
    {
        [SerializeField] private Image bg;
        [SerializeField] private TextMeshProUGUI gameName;
        [SerializeField] private TextMeshProUGUI playerCount;
        [SerializeField] private Image hasPassword;

        public CSteamID LobbyId { get; private set; }
        
        public void Init(string game_name, int player_count, int max_players, bool has_password, CSteamID lobby_id, Action<GameBrowserItem> on_select)
        {
            gameName.text = game_name;
            playerCount.text = $"{player_count}/{max_players}";
            hasPassword.enabled = has_password;
        }

        public void Select()
        {
            bg.color = new Color(0.5f, 0.5f, 0.5f);
        }

        public void Deselect()
        {
            bg.color = new Color(1f, 1f, 1f);
        }
    }
}