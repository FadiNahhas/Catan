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
        [AllowMutableSyncType] public readonly SyncVar<UserData> PlayerData = new();
        public string Name { get; private set; }
        public Texture2D Avatar { get; private set; }
        
        [AllowMutableSyncType] public readonly SyncVar<bool> IsReady = new();
        [AllowMutableSyncType] public readonly SyncVar<int> Index = new();

        #region Network Events

        public override void OnStartClient()
        {
            base.OnStartClient();
            if (IsOwner)
            {
                SetSteamId(UserData.Me);
            }
        }

        #endregion
        
        [ServerRpc(RequireOwnership = true)]
        private void SetSteamId(CSteamID steam_id)
        {
            PlayerData.Value = steam_id;
            Name = PlayerData.Value.Nickname;
            Avatar = PlayerData.Value.Avatar;
        }

        public bool IsHostPlayer()
        {
            var lobby = LocalGameManager.Instance.CurrentLobbyId;
            var host = SteamMatchmaking.GetLobbyOwner(lobby);
            
            return host == PlayerData.Value.id;
        }
    }
}