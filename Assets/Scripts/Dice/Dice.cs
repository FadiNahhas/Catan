using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dice
{
    public class Dice : MonoBehaviour
    {
        public Rigidbody RigidBody { get; private set;  }
        private static DiceRoller _diceRoller;
        
        [SerializeField, ReadOnly] private int result;
        public int Result => result;
        
        public event Action OnDiceSettled;
        public event Action OnResultDetected;

        private void Awake()
        {
            RigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!_diceRoller.IsRolling) return;
            
            if (IsSettled())
            {
                OnDiceSettled?.Invoke();
            }
        }
        
        public static void AssignRoller(DiceRoller diceRoller)
        {
            if (_diceRoller != null)
            {
                Debug.LogWarning("Dice roller already assigned to a dice.");
                return;
            }
            
            _diceRoller = diceRoller;
        }
        
        public bool IsSettled()
        {
            return RigidBody.IsSleeping();
        }

        public void DetectResult()
        {
            float maxDot = -1;
            Vector3 up = Vector3.up;

            foreach (Transform face in transform)
            {
                Vector3 direction = (face.position - transform.position).normalized;
                float dot = Vector3.Dot(direction, up);
                
                if (dot > maxDot)
                {
                    maxDot = dot;
                    result = face.GetComponent<Face>().FaceValue;
                }
            }
            
            OnResultDetected?.Invoke();
        }
    }
}