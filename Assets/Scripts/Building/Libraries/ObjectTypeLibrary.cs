using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Building.Libraries
{
    public abstract class ObjectTypeLibrary<T> : SerializedScriptableObject
    {
        public Dictionary<T, GameObject> Prefabs = new();
        
        public GameObject GetPrefab(T type)
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