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
    }
}