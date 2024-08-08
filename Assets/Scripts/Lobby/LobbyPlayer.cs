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
        [SerializeField] private Color color;

        private GamePlayer player;
        public GamePlayer Player => player;

        private void Awake()
        {
            if (_readyStatusColors != null) return;
            _readyStatusColors = Resources.Load<ReadyStatusColors>("Ready Status Colors");
            Debug.Log(_readyStatusColors);
        }

        public void Init(GamePlayer _player)
        {
            player = _player;
            SetPlayerName(_player.playerName);

            if (_player.isHost.Value)
            {
                SetReadyStatus(ReadyStatus.Host);
                return;
            }
            
            SetReadyStatus(_player.isReady.Value ?  ReadyStatus.Ready : ReadyStatus.NotReady);
        }

        public void SetReadyStatus(ReadyStatus _status)
        {
            status = _status;
            RefreshVisuals();
        }

        private void SetPlayerName(string _name)
        {
            playerName.text = _name;
        }

        public void SetPlayerColor(Color _color)
        {
            color = _color;
            RefreshVisuals();
        }

        private void RefreshVisuals()
        {
            avatar.color = color;
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
