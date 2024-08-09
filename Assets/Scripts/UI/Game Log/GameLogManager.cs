using Event_Bus;
using Space_Station_Tycoon.Scripts.Event_System;
using UnityEngine;

namespace UI.Game_Log
{
    public class GameLogManager : MonoBehaviour
    {
        [SerializeField] private GameLogText textPrefab;
        [SerializeField] private GameObject dividerPrefab;
        EventBinding<GameLogEvent> gameLogEvent;
        
        private void OnEnable()
        {
            gameLogEvent = new EventBinding<GameLogEvent>(AddMessage);
            EventBus<GameLogEvent>.Register(gameLogEvent);
        }

        private void OnDisable()
        {
            EventBus<GameLogEvent>.Deregister(gameLogEvent);
        }

        private void AddMessage(GameLogEvent e)
        {
            GameLogText text = Instantiate(textPrefab, transform);
            text.Init(e.Message);
        }
        
        private void AddDivider()
        {
            Instantiate(dividerPrefab, transform);
        }
        
        public void ClearLog()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}