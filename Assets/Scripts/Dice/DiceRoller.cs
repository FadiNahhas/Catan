using System;
using System.Linq;
using DG.Tweening;
using Event_Bus;
using Sirenix.OdinInspector;
using Space_Station_Tycoon.Scripts.Event_System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dice
{
    public class DiceRoller : MonoBehaviour
    {
        [TabGroup("Prefabs")] [SerializeField] private Dice dicePrefab;
        
        [TabGroup("Configuration")] [SerializeField] private float forceMagnitude = 10f;
        [TabGroup("Configuration")] [SerializeField] private float torqueMagnitude = 5f;
        [TabGroup("Configuration")] [SerializeField] private int diceCount = 2;
        [TabGroup("Configuration")] [SerializeField] private Vector3 spawnPosition;
        
        private Dice[] dice;
        [field: SerializeField, ReadOnly] public bool IsRolling { get; private set; }

        private int resultsCounted;
        
        private void Start()
        {
            InitializeDice();
        }

        private void InitializeDice()
        {
            dice = new Dice[diceCount];
            Dice.AssignRoller(this);
            for (var i = 0; i < diceCount; i++)
            {
                dice[i] = Instantiate(dicePrefab, spawnPosition + (i * Vector3.right), quaternion.identity);
                dice[i].OnDiceSettled += OnDiceSettled;
                dice[i].OnResultDetected += OnResultDetected;
            }
        }

        private void OnDisable()
        {
            foreach (var die in dice)
            {
                die.OnDiceSettled -= OnDiceSettled;
                die.OnResultDetected -= OnResultDetected;
            }
        }

        private void OnDiceSettled()
        {
            if (!dice.All(d => d.IsSettled())) return;
            IsRolling = false;
            
            ReturnDice();
        }

        private void ReturnDice()
        {
            for (var i = 0; i < diceCount; i++)
            {
                dice[i].transform.DOMove(spawnPosition + (i * Vector3.right), 1f).SetEase(Ease.InOutCubic).OnComplete(dice[i].DetectResult);
            }
        }

        private void OnResultDetected()
        {
            resultsCounted++;

            if (resultsCounted != diceCount) return;
            resultsCounted = 0;
            EventBus<DiceRolledEvent>.Raise(new DiceRolledEvent(dice.Sum(d => d.Result)));
        }

        [Button(ButtonSizes.Large)]
        void RollDice()
        {
            IsRolling = true;
            foreach (var die in dice)
            {
                var directionToCenter = (Vector3.zero - die.transform.position).normalized;
                var force = directionToCenter * forceMagnitude;
                
                die.RigidBody.AddForce(force, ForceMode.Impulse);

                var torque = new Vector3(
                    Random.Range(-torqueMagnitude, torqueMagnitude),
                    Random.Range(-torqueMagnitude, torqueMagnitude),
                    Random.Range(-torqueMagnitude, torqueMagnitude)
                    );
                die.RigidBody.AddTorque(torque, ForceMode.Impulse);
            }
        }
    }
}
