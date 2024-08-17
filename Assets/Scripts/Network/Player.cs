using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;

namespace Network
{
    public class Player : NetworkBehaviour
    {
        public UserData PlayerData { get; private set; }
        public string Name { get; private set; }
        public Texture2D Avatar { get; private set; }
        
        [AllowMutableSyncType] public readonly SyncVar<bool> IsReady = new();
        [AllowMutableSyncType] public readonly SyncVar<int> Index = new();
        public void AssignSteamId(CSteamID steam_id)
        {
            PlayerData = steam_id;
            Name = PlayerData.Nickname;
            Avatar = PlayerData.Avatar;
        }

        public bool IsHostPlayer()
        {
            var lobby = LocalGameManager.Instance.CurrentLobbyId;
            var host = SteamMatchmaking.GetLobbyOwner(lobby);
            
            return host == PlayerData;
        }
    }
}