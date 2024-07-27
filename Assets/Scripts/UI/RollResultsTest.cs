using Event_Bus;
using Space_Station_Tycoon.Scripts.Event_System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RollResultsTest : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultText;
        EventBinding<DiceRolledEvent> _diceRolledEventBinding;

        private void OnEnable()
        {
            _diceRolledEventBinding = new EventBinding<DiceRolledEvent>(UpdateResult);
            EventBus<DiceRolledEvent>.Register(_diceRolledEventBinding);
        }

        private void OnDisable()
        {
            EventBus<DiceRolledEvent>.Deregister(_diceRolledEventBinding);
        }

        private void UpdateResult(DiceRolledEvent e)
        {
            resultText.text = e.Result.ToString();
        }
        
    }
}