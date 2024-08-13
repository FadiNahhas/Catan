using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Lobby;
using UnityEngine;

namespace Network
{
    public class GameManager : NetworkBehaviour
    {
        private const int InvalidID = -1;
        
        [AllowMutableSyncType] public readonly SyncList<GamePlayer> Players = new(); 
        public readonly SyncList<int> AvailableIDs = new();
        public List<int> availableIdsEditor = new();

        private int _maxPlayers = 4;

        public override void OnStartServer()
        {
            base.OnStartServer();
            InitializeIds();
        }
        
        public override void OnStopServer()
        {
            base.OnStopServer();
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
            player.Index.Value = GetAvailableID();
            Players.Add(player);
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
            if (as_server) return;
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
            // Return the player's ID to the available IDs list
            ReturnID(player.Index.Value);
        }
        
        private void InitializeIds()
        {
            if(!IsServerInitialized) return;
            for (int i = 0; i < _maxPlayers; i++)
            {
                AvailableIDs.Add(i);
            }
        }

        private void SetMaxPlayers(int max_players)
        {
            _maxPlayers = max_players;
        }
        
        private int GetAvailableID()
        {
            if(!IsServerInitialized) return InvalidID;

            var id = InvalidID;
            
            if (AvailableIDs.Count > 0)
            {
                id = AvailableIDs.Min();
            }
            
            if (id != InvalidID)
            {
                AvailableIDs.Remove(id);
                Debug.Log($"Assigned ID: {id}, Remaining IDs: {string.Join(",", AvailableIDs)}");
            }

            return id;
        }
        
        private void ReturnID(int id)
        {
            if(!IsServerInitialized) return;
            AvailableIDs.Add(id);
            Debug.Log($"Returned ID: {id}, Available IDs: {string.Join(",", AvailableIDs)}");
        }

        [ServerRpc]
        public void StartGame()
        {
            
        }
    }
}