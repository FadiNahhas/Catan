using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Lobby;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Network
{
    public class GamePlayer : NetworkBehaviour
    {
        public static GamePlayer LocalPlayer { get; private set; }
        
        [TabGroup("Data")] public int index;
        [TabGroup("Data")] public string playerName = "testPlayer";
        
        [TabGroup("State")][AllowMutableSyncType] public readonly SyncVar<bool> IsHostPlayer = new();
        [TabGroup("State")][AllowMutableSyncType] public readonly SyncVar<bool> IsReady = new();
        [TabGroup("State")] public bool host;
        [TabGroup("State")] public bool ready;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                LocalPlayer = this;
                if (IsHostInitialized)
                {
                    IsHostPlayer.Value = true;
                }
                LocalManager.Instance.ShowLobby(); // Show lobby for the player who owns this object
                LocalManager.Instance.UpdateLobbyUI(this);
                RequestAddToLobby();
            }
            IsHostPlayer.OnChange += OnHostPlayerChanged;
            IsReady.OnChange += OnReadyChanged;
        }
        
        private void OnReadyChanged(bool prev, bool next, bool asserver)
        {
            Debug.Log($"Ready status changed: {prev} -> {next}");
            ready = next;
            var lobbyPlayer = GameLobby.Instance.GetPlayer(this);
            lobbyPlayer.SetReadyStatus(next ? ReadyStatus.Ready : ReadyStatus.NotReady);
        }

        private void OnHostPlayerChanged(bool prev, bool next, bool as_server)
        {
            host = next;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();

            if (IsOwner)
            {
                LocalPlayer = null;
                GameLobby.Instance.ClearLobby();
                LocalManager.Instance.HideLobby();
            }
            IsHostPlayer.OnChange -= OnHostPlayerChanged;
            IsReady.OnChange -= OnReadyChanged;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            RequestRemoveFromLobby();
        }

        private void RequestAddToLobby() => AddToLobbyServerRpc();
        
        private void RequestRemoveFromLobby() => RemoveFromLobby();
        
        public void RequestReadyStatusChange(bool value) => ChangeReadyStatusServerRpc(value);

        [ServerRpc]
        private void ChangeReadyStatusServerRpc(bool value)
        {
            FindObjectOfType<GameManager>().SetReadyStatus(this, value);
        }

        [ServerRpc]
        private void AddToLobbyServerRpc()
        {
            FindObjectOfType<GameManager>().AddToLobby(this);
        }

        private void RemoveFromLobby()
        {
            FindObjectOfType<GameManager>().RemoveFromLobby(this);
        }
    }
}