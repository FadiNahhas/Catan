using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
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
        
        private Dice[] _dice;
        [field: SerializeField, ReadOnly] public bool IsRolling { get; private set; }

        private void Start()
        {
            InitializeDice();
        }

        private void InitializeDice()
        {
            _dice = new Dice[diceCount];
            Dice.AssignRoller(this);
            for (var i = 0; i < diceCount; i++)
            {
                _dice[i] = Instantiate(dicePrefab, spawnPosition + (i * Vector3.right), quaternion.identity);
                _dice[i].OnDiceSettled += OnDiceSettled;
            }
        }

        private void OnDisable()
        {
            foreach (var die in _dice)
            {
                die.OnDiceSettled -= OnDiceSettled;
            }
        }

        private void OnDiceSettled()
        {
            if (!_dice.All(d => d.IsSettled())) return;
            IsRolling = false;
            ReturnDice();
        }

        private void ReturnDice()
        {
            for (var i = 0; i < diceCount; i++)
            {
                _dice[i].transform.DOMove(spawnPosition + (i * Vector3.right), 1f).SetEase(Ease.InOutCubic);
            }
        }

        [Button]
        void RollDice()
        {
            IsRolling = true;
            foreach (var die in _dice)
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