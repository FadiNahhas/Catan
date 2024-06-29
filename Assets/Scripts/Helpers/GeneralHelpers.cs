using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class GeneralHelpers
    {
        public static T GetChildWithTag<T>(this Transform parent, string tag) where T : Component
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(tag))
                {
                    return child.GetComponent<T>();
                }
            }
            return null;
        }
        
        public static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
            
            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }
    }
}