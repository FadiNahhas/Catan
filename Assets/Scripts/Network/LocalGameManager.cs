using FishNet.Managing.Scened;
using Helpers;
using Steamworks;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Network
{
    public class LocalGameManager : Singleton<LocalGameManager>
    {
        private Callback<LobbyEnter_t> _lobbyEnterCallback;
        
        private CSteamID _currentLobbyId;

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _lobbyEnterCallback = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        }
        
        private void OnDisable()
        {
            _lobbyEnterCallback?.Dispose();
        }

        #endregion

        #region Callback Handlers

        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            if (callback.m_EChatRoomEnterResponse == (int)EChatRoomEnterResponse.k_EChatRoomEnterResponseSuccess)
            {
                Debug.Log($"Successfully joined lobby: {callback.m_ulSteamIDLobby}");
                
                _currentLobbyId = new CSteamID(callback.m_ulSteamIDLobby);
                SceneManager.LoadScene(sceneBuildIndex: 1);
            }
            else
            {
                Debug.LogWarning($"Failed to join lobby: {callback.m_ulSteamIDLobby}");
            }
        }

        #endregion
    }
}