using System.Collections.Generic;
using Board.Pieces.Libraries;
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
        [TabGroup("References"), SerializeField, Required] private PlayerColors playerColors;
        
        [TabGroup("Prefabs"), SerializeField] private LobbyPlayer lobbyPlayerPrefab;

        [TabGroup("State"), SerializeField] private List<LobbyPlayer> players = new();
        
        public void SetLobbyName(string lobby_name)
        {
            lobbyName.text = lobby_name;
        }
        
        public void AddPlayer(GamePlayer player)
        {
            var p = Instantiate(lobbyPlayerPrefab, playerContainer);
            // Initialize player variables (host, name, color, etc.)
            p.Init(player, playerColors.GetColor(player.Index.Value));
            players.Add(p);
        }

        // TODO: Add player class and implement function
        public void RemovePlayer(GamePlayer player_to_remove)
        {
            var player = players.Find(p => p.Player == player_to_remove);
            
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

        public void UpdateLobbyButtons(GamePlayer local_player)
        {
            if (local_player.IsHostPlayer.Value)
            {
                startButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(false);
            }
            else
            {
                startButton.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(true);
            }
        }
        
        public LobbyPlayer GetPlayer(GamePlayer player)
        {
            return players.Find(p => p.Player == player);
        }
        
        public void ToggleStartButtonInteractable(bool interactable)
        {
            startButton.interactable = interactable;
        }
    }
}
