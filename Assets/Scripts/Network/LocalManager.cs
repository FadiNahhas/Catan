using Helpers;
using Lobby;
using UnityEngine;

namespace Network
{
    public class LocalManager : Singleton<LocalManager>
    {
        [SerializeField] private GameLobby lobby;
        
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

        public void OnStartButtonClicked()
        {
            /*
             * TODO:
             * Check for at least 2 players
             * Initialize board generator
             * Send board data to all players
             * animate board generation locally
             */
        }
    }
}