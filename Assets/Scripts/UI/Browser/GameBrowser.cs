using System.Collections.Generic;
using Network;
using Sirenix.OdinInspector;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Browser
{
    public class GameBrowser : MonoBehaviour
    {

        
        [TabGroup("Prefabs"), SerializeField] private GameBrowserItem gameBrowserItemPrefab;
        [TabGroup("References"), SerializeField] private Transform content;
        [TabGroup("References"), SerializeField] private Button createButton;
        [TabGroup("References"), SerializeField] private Button joinButton;
        [TabGroup("References"), SerializeField] private Button refreshButton;
        [TabGroup("References"), SerializeField] private LobbyCreator lobbyCreator;

        private Callback<LobbyMatchList_t> _lobbyMatchListCallback;
        private List<CSteamID> _lobbies = new();
        
        private List<GameBrowserItem> _gameBrowserItems = new();
        private GameBrowserItem _selectedLobby;

        #region Unity Events

        private void OnEnable()
        {
            _lobbyMatchListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);
            refreshButton.onClick.AddListener(FetchPublicLobbies);
            createButton.onClick.AddListener(CreateLobby);
        }

        private void OnDisable()
        {
            _lobbyMatchListCallback?.Dispose();
            refreshButton.onClick.RemoveListener(FetchPublicLobbies);
            createButton.onClick.RemoveListener(CreateLobby);
        }

        #endregion

        #region Steamworks Methods

        public void FetchPublicLobbies()
        {
            SetAllButtonInteractable(false);
            SteamMatchmaking.AddRequestLobbyListStringFilter(LobbyManager.TestLobbyKey, "true", ELobbyComparison.k_ELobbyComparisonEqual);
            SteamMatchmaking.RequestLobbyList();
        }

        #endregion

        #region Callback Handlers

        // This method is called when the lobby list is received from Steam
        private void OnLobbyMatchList(LobbyMatchList_t callback)
        {
            ClearLobbies();
            
            for (var i = 0; i < callback.m_nLobbiesMatching; i++)
            {
                var lobby = SteamMatchmaking.GetLobbyByIndex(i);
                _lobbies.Add(lobby);
            }

            DisplayLobbies();
        }

        #endregion

        #region UI Methods

        private void DisplayLobbies()
        {
            foreach (var lobby in _lobbies)
            {
                bool hasStarted = SteamMatchmaking.GetLobbyData(lobby, LobbyManager.StartStatusKey) == "true";
                
                if (hasStarted) continue;
                
                string lobbyName = SteamMatchmaking.GetLobbyData(lobby, LobbyManager.NameKey);
                int currentPlayers = SteamMatchmaking.GetNumLobbyMembers(lobby);
                int maxPlayers = SteamMatchmaking.GetLobbyMemberLimit(lobby);
                bool hasPassword = SteamMatchmaking.GetLobbyData(lobby, LobbyManager.LockedKey) == "true";
                
                AddLobby(lobbyName, currentPlayers, maxPlayers, hasPassword, lobby);
            }
            
            SetAllButtonInteractable(true);
        }
        private void AddLobby(string game_name, int player_count, int max_players, bool has_password, CSteamID lobby_id)
        {
            var gameBrowserItem = Instantiate(gameBrowserItemPrefab, content);
            gameBrowserItem.Init(game_name, player_count, max_players, has_password, lobby_id, SelectLobby);
            _gameBrowserItems.Add(gameBrowserItem);
        }

        private void SelectLobby(GameBrowserItem lobby)
        {
            _selectedLobby?.Deselect();
            _selectedLobby = lobby;
            _selectedLobby.Select();
            
            SetButtonInteractable(joinButton, true);
        }

        private void CreateLobby()
        {
            lobbyCreator.gameObject.SetActive(true);
            SetAllButtonInteractable(false);
        }

        public void CancelLobbyCreation()
        {
            lobbyCreator.gameObject.SetActive(false);
            SetAllButtonInteractable(true);
        }

        private void ClearLobbies()
        {
            _selectedLobby = null;
            
            foreach (var item in _gameBrowserItems)
            {
                Destroy(item.gameObject);
            }
            
            _gameBrowserItems.Clear();
            _lobbies.Clear();
        }
        
        private void SetAllButtonInteractable(bool interactable)
        {
            createButton.interactable = interactable;
            joinButton.interactable = interactable;
            refreshButton.interactable = interactable;
        }
        
        private void SetButtonInteractable(Button button, bool enable)
        {
            button.interactable = enable;
        }
        
        #endregion
        
    }
}