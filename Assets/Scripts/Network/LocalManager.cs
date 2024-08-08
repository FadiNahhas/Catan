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
        
        public void UpdateLobbyUI(GamePlayer _localPlayer)
        {
            lobby.UpdateLobbyButtons(_localPlayer);
        }

        public void OnReadyButtonClicked()
        {
            if (GamePlayer.LocalPlayer == null) return;
            Debug.Log($"Requesting ready status change, new value: {!GamePlayer.LocalPlayer.isReady.Value}");
            GamePlayer.LocalPlayer.RequestReadyStatusChange(!GamePlayer.LocalPlayer.isReady.Value);
        }
        
        public void OnLeaveButtonClicked()
        {
            if (GamePlayer.LocalPlayer == null) return;
        }
    }
}