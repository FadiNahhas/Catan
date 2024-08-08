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
        
        [TabGroup("State")][AllowMutableSyncType] public readonly SyncVar<bool> isHost = new();
        [TabGroup("State")][AllowMutableSyncType] public readonly SyncVar<bool> isReady = new();
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
                    isHost.Value = true;
                }
                LocalManager.Instance.ShowLobby(); // Show lobby for the player who owns this object
                LocalManager.Instance.UpdateLobbyUI(this);
                RequestAddToLobby();
            }
            isHost.OnChange += OnHostChanged;
            isReady.OnChange += OnReadyChanged;
        }
        
        private void OnReadyChanged(bool _prev, bool _next, bool _asserver)
        {
            Debug.Log($"Ready status changed: {_prev} -> {_next}");
            ready = _next;
            var lobbyPlayer = GameLobby.Instance.GetPlayer(this);
            lobbyPlayer.SetReadyStatus(_next ? ReadyStatus.Ready : ReadyStatus.NotReady);
        }

        private void OnHostChanged(bool _prev, bool _next, bool _asServer)
        {
            host = _next;
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
            isHost.OnChange -= OnHostChanged;
            isReady.OnChange -= OnReadyChanged;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            RequestRemoveFromLobby();
        }

        private void RequestAddToLobby() => AddToLobbyServerRpc();
        
        private void RequestRemoveFromLobby() => RemoveFromLobby();
        
        public void RequestReadyStatusChange(bool _value) => ChangeReadyStatusServerRpc(_value);

        [ServerRpc]
        private void ChangeReadyStatusServerRpc(bool _value)
        {
            FindObjectOfType<GameManager>().SetReadyStatus(this, _value);
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