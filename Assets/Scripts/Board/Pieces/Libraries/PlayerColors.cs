using System.Collections.Generic;
using UnityEngine;

namespace Board.Pieces.Libraries
{
    [CreateAssetMenu(fileName = "Player Colors", menuName = "Catan/Player Colors")]
    public class PlayerColors : ScriptableObject
    {
        public List<Color> colors = new();
        public Color GetColor(int index)
        {
            if (index >= 0 && index < colors.Count) return colors[index];
            
            Debug.LogError($"Index {index} is out of range for colors list");
            return Color.white;

        }
    }
}