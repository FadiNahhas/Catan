using System;
using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Lobby;

namespace Network
{
    public class GameManager : NetworkBehaviour
    {
        [AllowMutableSyncType] public readonly SyncList<GamePlayer> Players = new();
        [AllowMutableSyncType] public readonly SyncVar<int> NextID = new();

        public override void OnStartServer()
        {
            base.OnStartServer();
            NextID.SetInitialValues(0);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Players.OnChange += OnPlayerListChanged;
        }
        
        public override void OnStopClient()
        {
            base.OnStopClient();
            Players.OnChange -= OnPlayerListChanged;
        }
        
        public void AddToLobby(GamePlayer player)
        {
            if(!IsServerInitialized) return;
            player.Index.Value = NextID.Value;
            Players.Add(player);
            NextID.Value++;
        }
        
        public void RemoveFromLobby(GamePlayer player)
        {
            if(!IsServerInitialized) return;
            Players.Remove(player);
        }
        
        public void SetReadyStatus(GamePlayer player, bool ready)
        {
            if(!IsServerInitialized) return;
            player.IsReady.Value = ready;
        }
        
        private void OnPlayerListChanged(SyncListOperation op, int index, GamePlayer old_item, GamePlayer new_item, bool as_server)
        {
            if (!IsClientInitialized) return;
            switch (op)
            {
                case SyncListOperation.Add:
                case SyncListOperation.Insert:
                case SyncListOperation.Clear:
                    UpdateLobbyUI();
                    break;
                case SyncListOperation.RemoveAt:
                    RemovePlayerFromLobby(old_item);
                    break;
                case SyncListOperation.Set:
                    break;
                case SyncListOperation.Complete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(op), op, null);
            }
        }
        
        private void UpdateLobbyUI()
        {
            GameLobby.Instance.ClearLobby();
            foreach (var player in Players.Collection)
            {
                GameLobby.Instance.AddPlayer(player);
            }
        }
        
        private void RemovePlayerFromLobby(GamePlayer player)
        {
            GameLobby.Instance.RemovePlayer(player);
        }
    }
}