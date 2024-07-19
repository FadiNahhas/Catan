using System.Collections.Generic;
using Board;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Building.Libraries
{
    [CreateAssetMenu(menuName = "Catan/New BuildingType Library", fileName = "BuildingLibrary")]
    public class PiecesLibraryLibrary : SerializedScriptableObject
    {
        public Dictionary<BuildingType, GameObject> Prefabs = new();
        
        public GameObject GetPrefab(BuildingType type)
        {
            if (!Prefabs.TryGetValue(type, out var prefab))
            {
                Debug.LogWarning($"No prefab found for type {type}");
                return null;
            }

            return prefab;
        }
    }
}