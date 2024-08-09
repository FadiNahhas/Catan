using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Board.Configuration
{
    [CreateAssetMenu(fileName = "Number Configuration", menuName = "Catan/Number Configuration")]
    public class NumbersConfiguration : ScriptableObject
    {
        public List<NumberCount> numberCounts;

        private void OnValidate()
        {
            // Remove any duplicates
            var numberCountDictionary = new Dictionary<int, int>();
            
            foreach (var numberCount in numberCounts)
            {
                if (numberCountDictionary.ContainsKey(numberCount.number))
                {
                    Debug.LogError($"There are multiple entries for number {numberCount.number}");
                }
                else
                {
                    numberCountDictionary.Add(numberCount.number, 1);
                }
            }
        }
    }
    
    [Serializable]
    public class NumberCount
    {
        [MaxValue(12), MinValue(2)] public int number;
        public bool isSingle;
    }
}