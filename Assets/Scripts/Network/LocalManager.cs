using Helpers;
using Lobby;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Network
{
    public class LocalManager : Singleton<LocalManager>
    {
        [SerializeField, Required] private GameLobby lobby;
        [SerializeField, Required] private GameManager gameManager;
        
        public void ShowLobby()
        {
            lobby.gameObject.SetActive(true);
        }
        
        public void HideLobby()
        {
            lobby.gameObject.SetActive(false);
        }
        
        public void UpdateLobbyUI(GamePlayer local_player)
        {
            lobby.UpdateLobbyButtons(local_player);
        }

        public void OnReadyButtonClicked()
        {
            if (GamePlayer.LocalPlayer == null) return;
            Debug.Log($"Requesting ready status change, new value: {!GamePlayer.LocalPlayer.IsReady.Value}");
            GamePlayer.LocalPlayer.RequestReadyStatusChange(!GamePlayer.LocalPlayer.IsReady.Value);
        }
        
        public void OnLeaveButtonClicked()
        {
            if (GamePlayer.LocalPlayer == null) return;
        }

        public void CheckIfCanStart()
        {
            var players = gameManager.Players.Collection;
            
            // Get all players except the host
            var readyPlayers = players.FindAll(p => p != GamePlayer.LocalPlayer);
            
            // Check if all players are ready
            var canStart = readyPlayers.TrueForAll(p => p.IsReady.Value);
            
            lobby.ToggleStartButtonInteractable(canStart);
        }

        public void OnStartButtonClicked()
        {
            if (GamePlayer.LocalPlayer == null) return;
            if (!GamePlayer.LocalPlayer.IsHostPlayer.Value) return;
            /*
             * TODO:
             * Initialize board generator
             * Send board data to all players
             * animate board generation locally
             */
            gameManager.StartGame();
        }
    }
}