using System.Collections;
using System.Collections.Generic;
using Dependency_Injection;
using Network;
using Sirenix.OdinInspector;
using Steamworks;
using UI.ModalSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Browser
{
    public class GameBrowser : MonoBehaviour, IDependencyProvider
    {
        [TabGroup("Prefabs"), SerializeField] private GameBrowserItem gameBrowserItemPrefab;
        [TabGroup("References"), SerializeField] private Transform content;
        [TabGroup("References"), SerializeField] private Button createButton;
        [TabGroup("References"), SerializeField] private Button joinButton;
        [TabGroup("References"), SerializeField] private Button refreshButton;
        [TabGroup("References"), SerializeField] private GameObject loadingWheel;
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
            joinButton.onClick.AddListener(JoinLobby);
        }

        private void OnDisable()
        {
            _lobbyMatchListCallback?.Dispose();
            refreshButton.onClick.RemoveListener(FetchPublicLobbies);
            createButton.onClick.RemoveListener(CreateLobby);
            joinButton.onClick.RemoveListener(JoinLobby);
        }

        #endregion

        #region DependencyInjection

        [Provide]
        private GameBrowser ProvideGameBrowser() => this;

        #endregion

        #region Steamworks Methods

        public void FetchPublicLobbies()
        {
            SetAllButtonInteractable(false);
            loadingWheel.SetActive(true);
            SteamMatchmaking.AddRequestLobbyListStringFilter(LobbyManager.TestLobbyKey, "true", ELobbyComparison.k_ELobbyComparisonEqual);
            SteamMatchmaking.RequestLobbyList();
        }

        private void JoinLobby()
        {
            if (_selectedLobby == null)
            {
                ModalManager.Show("Error", "Please select a lobby to join.");
                return;
            } 
            
            var hasPassword = SteamMatchmaking.GetLobbyData(_selectedLobby.LobbyId, LobbyManager.LockedKey) == "true";
            if (hasPassword)
            {
                StartCoroutine(JoinLockedLobby());
                return;
            }
            
            LobbyManager.Instance.JoinLobby(_selectedLobby.LobbyId);
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
            loadingWheel.SetActive(false);
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
            SetButtonInteractable(joinButton, false);
        }
        
        private void AddLobby(string game_name, int player_count, int max_players, bool has_password, CSteamID lobby_id)
        {
            var gameBrowserItem = Instantiate(gameBrowserItemPrefab, content);
            gameBrowserItem.Init(game_name, player_count, max_players, has_password, lobby_id, SelectLobby);
            _gameBrowserItems.Add(gameBrowserItem);
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
        
        #region Button Handlers
        
        private void SelectLobby(GameBrowserItem lobby)
        {
            if (lobby == _selectedLobby)
            {
                _selectedLobby.Deselect();
                _selectedLobby = null;
                SetButtonInteractable(joinButton, false);
                return;
            }
            
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
        
        #endregion

        #region Coroutines

        private IEnumerator JoinLockedLobby()
        {
            var isPasswordCorrect = false;
            PasswordModalSystem.GetPassword();
            
            while (!isPasswordCorrect)
            {
                while (PasswordModalSystem.Response == PasswordModalResponse.None) yield return null;

                switch (PasswordModalSystem.Response)
                {
                    case PasswordModalResponse.Cancel:
                        PasswordModalSystem.Close();
                        yield break;
                    case PasswordModalResponse.Join:
                    {
                        var lobbyPassword = SteamMatchmaking.GetLobbyData(_selectedLobby.LobbyId, LobbyManager.PasswordKey);
                        var password = PasswordModalSystem.Password;

                        if (lobbyPassword != password)
                        {
                            ModalManager.Show("Error", "Incorrect password.");
                            PasswordModalSystem.ResetResponse();
                        }
                        else
                        {
                            isPasswordCorrect = true;
                            PasswordModalSystem.Close();
                            LobbyManager.Instance.JoinLobby(_selectedLobby.LobbyId);
                        }

                        break;
                    }
                }

                yield return null;
            }

            yield return null;
        }

        #endregion
        
    }
}