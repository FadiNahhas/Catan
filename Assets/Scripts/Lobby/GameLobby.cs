using System.Collections.Generic;
using Helpers;
using Network;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class GameLobby : Singleton<GameLobby>
    {
        [TabGroup("References"), SerializeField] private TextMeshProUGUI lobbyName;
        [TabGroup("References"), SerializeField] private Button leaveButton;
        [TabGroup("References"), SerializeField] private Button readyButton;
        [TabGroup("References"), SerializeField] private Button startButton;
        [TabGroup("References"), SerializeField] private Transform playerContainer;
        
        [TabGroup("Prefabs"), SerializeField] private LobbyPlayer lobbyPlayerPrefab;

        [TabGroup("State"), SerializeField] private List<LobbyPlayer> players = new();
        
        public void SetLobbyName(string _name)
        {
            lobbyName.text = _name;
        }
        
        public void AddPlayer(GamePlayer _player)
        {
            var player = Instantiate(lobbyPlayerPrefab, playerContainer);
            // Initialize player variables (host, name, color, etc.)
            player.Init(_player);
            players.Add(player);
        }

        // TODO: Add player class and implement function
        public void RemovePlayer(GamePlayer _player)
        {
            var player = players.Find(_p => _p.Player == _player);
            
            if (player == null){
                Debug.LogWarning("Couldn't find player!");   
                return;
            }
            
            Destroy(player.gameObject);
            players.Remove(player);
        }
        
        public void ClearLobby()
        {
            foreach (var player in players)
            {
                Destroy(player.gameObject);
            }
            players.Clear();
        }
    }
}
