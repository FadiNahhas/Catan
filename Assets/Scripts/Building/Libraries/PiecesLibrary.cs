using System.Collections.Generic;
using Board;
using Building.Pieces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Building.Libraries
{
    [CreateAssetMenu(menuName = "Catan/New BuildingType Library", fileName = "BuildingLibrary")]
    public class PiecesLibrary : SerializedScriptableObject
    {
        public Dictionary<BuildingType, Piece> Prefabs = new();
        
        public Piece GetPrefab(BuildingType type)
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