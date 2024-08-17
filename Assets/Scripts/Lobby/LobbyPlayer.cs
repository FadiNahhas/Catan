using System;
using DG.Tweening;
using Network;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyPlayer : MonoBehaviour
    {
        private static ReadyStatusColors _readyStatusColors;
        
        [SerializeField] private Image avatar;
        [SerializeField] private Image readyStatus;
        [SerializeField] private TextMeshProUGUI playerName;

        [SerializeField] private ReadyStatus status;
        public Player Player { get; private set; }

        private void Awake()
        {
            if (_readyStatusColors != null) return;
            _readyStatusColors = Resources.Load<ReadyStatusColors>("Ready Status Colors");
        }

        public void Init(Player player, Color player_color)
        {
            Player = player;
            SetPlayerName(player.Name);

            if (player.IsHostPlayer())
            {
                SetReadyStatus(ReadyStatus.Host);
            }
            else
            {
                SetReadyStatus(player.IsReady.Value ?  ReadyStatus.Ready : ReadyStatus.NotReady);
            }
            SetPlayerColor(player_color);
        }

        public void SetReadyStatus(ReadyStatus new_status)
        {
            status = new_status;
            RefreshVisuals();
        }

        private void SetPlayerName(string player_name)
        {
            playerName.text = player_name;
        }

        public void SetPlayerColor(Color player_color)
        {
            avatar.color = player_color;
        }

        private void RefreshVisuals()
        {
            readyStatus.DOColor(_readyStatusColors.GetColor(status), 0.75f);
        }

        #region Test Functions  

        [ButtonGroup("Ready Status")][Button(ButtonSizes.Medium)]
        public void TestSetReady()
        {
            SetReadyStatus(ReadyStatus.Ready);
        }
        
        [ButtonGroup("Ready Status")][Button(ButtonSizes.Medium)]
        public void TestSetNotReady()
        {
            SetReadyStatus(ReadyStatus.NotReady);
        }
        
        [ButtonGroup("Ready Status")][Button(ButtonSizes.Medium)]
        public void TestSetHost()
        {
            SetReadyStatus(ReadyStatus.Host);
        }

        #endregion
        
    }
    
    [Serializable]
    public enum ReadyStatus
    {
        NotReady,
        Ready,
        Host
    }
}
