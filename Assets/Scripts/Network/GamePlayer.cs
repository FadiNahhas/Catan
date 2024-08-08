using FishNet.Object;
using Lobby;
using Sirenix.OdinInspector;

namespace Network
{
    public class GamePlayer : NetworkBehaviour
    {
        [TabGroup("Data")] public int index;
        [TabGroup("Data")] public string playerName = "testPlayer";
        
        [TabGroup("State")] public bool isHost;
        [TabGroup("State")] public bool isReady;

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                LocalManager.Instance.ShowLobby(); // Show lobby for the player who owns this object
                RequestAddToLobby();
            }
        }
        
        public override void OnStopClient()
        {
            base.OnStopClient();

            if (IsOwner)
            {
                GameLobby.Instance.ClearLobby();
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            RequestRemoveFromLobby();
        }

        private void RequestAddToLobby() => AddToLobbyServerRpc();
        
        private void RequestRemoveFromLobby() => RemoveFromLobbyServerRpc();
        
        [ServerRpc]
        private void AddToLobbyServerRpc()
        {
            FindObjectOfType<GameManager>().AddToLobby(this);
        }

        private void RemoveFromLobbyServerRpc()
        {
            FindObjectOfType<GameManager>().RemoveFromLobby(this);
        }
    }
}