using System.Collections.Generic;
using UnityEngine;

namespace Board
{
    [CreateAssetMenu(fileName = "Player Colors", menuName = "Catan/Player Colors")]
    public class PlayerColors : ScriptableObject
    {
        public List<Color> colors = new();
        public Color GetColor(int _index)
        {
            if (_index >= 0 && _index < colors.Count) return colors[_index];
            
            Debug.LogError($"Index {_index} is out of range for colors list");
            return Color.white;

        }
    }
}