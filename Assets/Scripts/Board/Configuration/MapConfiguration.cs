using System;
using System.Collections.Generic;
using Hex;
using UnityEngine;

namespace Board.Configuration
{
    [CreateAssetMenu(fileName = "Map Configuration", menuName = "Catan/Map Configuration")]
    public class MapConfiguration : ScriptableObject
    {
        public List<CellTypeCount> cellTypeCounts;

        private void OnValidate()
        {
            // Get a dictionary with the count of each cell type
            var typeCountDictionary = new Dictionary<CellType, int>();
            
            // Add all types with count 0
            foreach (CellType type in Enum.GetValues(typeof(CellType)))
            {
                typeCountDictionary.Add(type, 0);
            }
            
            // Add the count of each cell type
            foreach (var cellTypeCount in cellTypeCounts)
            {
                typeCountDictionary[cellTypeCount.type]++;
            }
            
            // Check if there are any cell types with count greater than 1
            foreach (var (type, count) in typeCountDictionary)
            {
                if (count > 1)
                {
                    Debug.LogError($"There are {count} cells of type {type}");
                }
            }
        }
    }

    [Serializable]
    public class CellTypeCount
    {
        public CellType type;
        public int count;
    }
}