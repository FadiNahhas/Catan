using Event_Bus;
using Space_Station_Tycoon.Scripts.Event_System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RollResultsTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultText;
        EventBinding<DiceRolledEvent> diceRolledEventBinding;

        private void OnEnable()
        {
            diceRolledEventBinding = new EventBinding<DiceRolledEvent>(UpdateResult);
            EventBus<DiceRolledEvent>.Register(diceRolledEventBinding);
        }

        private void OnDisable()
        {
            EventBus<DiceRolledEvent>.Deregister(diceRolledEventBinding);
        }

        private void UpdateResult(DiceRolledEvent e)
        {
            resultText.text = e.Result.ToString();
        }
        
    }
}