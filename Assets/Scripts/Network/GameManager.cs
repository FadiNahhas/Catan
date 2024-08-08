using System;
using System.Collections.Generic;
using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Lobby;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Network
{
    public class GameManager : NetworkBehaviour
    {
        [AllowMutableSyncType] public readonly SyncList<GamePlayer> Players = new();
        
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
        
        public void AddToLobby(GamePlayer _player)
        {
            if(!IsServerInitialized) return;
            Players.Add(_player);
        }
        
        public void RemoveFromLobby(GamePlayer _player)
        {
            if(!IsServerInitialized) return;
            Players.Remove(_player);
        }
        
        private void OnPlayerListChanged(SyncListOperation _op, int _index, GamePlayer _oldItem, GamePlayer _newItem, bool _asServer)
        {
            Debug.Log($"SyncList Changed: Operation: {_op}, Index: {_index}, OldItem: {_oldItem}, NewItem: {_newItem}, AsServer: {_asServer}");
            if (!IsClientInitialized) return;
            switch (_op)
            {
                case SyncListOperation.Add:
                case SyncListOperation.Insert:
                case SyncListOperation.RemoveAt:
                case SyncListOperation.Clear:
                    UpdateLobbyUI();
                    break;
                case SyncListOperation.Set:
                    break;
                case SyncListOperation.Complete:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_op), _op, null);
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
    }
}