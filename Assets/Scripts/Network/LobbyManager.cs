using Helpers;
using Steamworks;
using UnityEngine;

public struct LobbyMetadata
{
    public string Name;
    public string Password;
    public int MaxPlayers;
    
    public string HasPassword => string.IsNullOrEmpty(Password) ? "false" : "true";
}

namespace Network
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        public const string TestLobbyKey = "catan_test_lobby";
        public const string NameKey = "name";
        public const string PasswordKey = "password";
        public const string LockedKey = "hasPassword";
        public const string StartStatusKey = "hasStarted";
        private Callback<LobbyCreated_t> _lobbyCreatedCallback;
        
        private LobbyMetadata _pendingLobbyMetadata;

        #region Unity Events

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        }
        
        private void OnDisable()
        {
            _lobbyCreatedCallback?.Dispose();
        }

        #endregion

        #region Lobby Methods

        public void CreateLobby(string lobby_name, int max_players, string lobby_password)
        {
            _pendingLobbyMetadata = new LobbyMetadata
            {
                Name = lobby_name,
                Password = lobby_password,
                MaxPlayers = max_players
            };
            
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, max_players);
        }
        
        public void JoinLobby(CSteamID lobby_id)
        {
            SteamMatchmaking.JoinLobby(lobby_id);
            Debug.Log($"Attempting to join lobby: {lobby_id}");
        }

        private void SetLobbyData(CSteamID lobby_id)
        {
            SteamMatchmaking.SetLobbyData(lobby_id, TestLobbyKey, "true");
            SteamMatchmaking.SetLobbyData(lobby_id, NameKey, _pendingLobbyMetadata.Name);
            SteamMatchmaking.SetLobbyData(lobby_id, LockedKey, _pendingLobbyMetadata.HasPassword);
            SteamMatchmaking.SetLobbyData(lobby_id, PasswordKey, _pendingLobbyMetadata.Password);
            SteamMatchmaking.SetLobbyData(lobby_id, StartStatusKey, "false");
        }

        #endregion

        #region Callback Handlers

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult == EResult.k_EResultOK)
            {
                var lobbyId = new CSteamID(callback.m_ulSteamIDLobby);
                if (lobbyId.IsValid())
                {
                    SetLobbyData(lobbyId);
                    Debug.Log($"Lobby metadata set for {lobbyId}");
                }
                else
                {
                    Debug.Log("Invalid lobby ID");
                }

                Debug.Log($"Lobby created: {lobbyId}");
            }
            else
            {
                Debug.LogWarning($"Failed to create lobby: {callback.m_eResult}");
            }
        }

        #endregion
    }
}