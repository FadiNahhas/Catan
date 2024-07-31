using Event_Bus;
using Space_Station_Tycoon.Scripts.Event_System;
using UnityEngine;

namespace UI.Game_Log
{
    public class GameLogManager : MonoBehaviour
    {
        [SerializeField] private GameLogText textPrefab;
        EventBinding<GameLogEvent> _gameLogEvent;
        
        private void OnEnable()
        {
            _gameLogEvent = new EventBinding<GameLogEvent>(AddMessage);
            EventBus<GameLogEvent>.Register(_gameLogEvent);
        }

        private void OnDisable()
        {
            EventBus<GameLogEvent>.Deregister(_gameLogEvent);
        }

        private void AddMessage(GameLogEvent e)
        {
            GameLogText text = Instantiate(textPrefab, transform);
            text.Init(e.Message);
        }
    }
}