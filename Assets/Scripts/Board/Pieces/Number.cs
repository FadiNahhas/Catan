using Helpers;
using TMPro;
using UnityEngine;

namespace Board.Pieces
{
    public class Number : MonoBehaviour
    {
        private const float ScaleFactor = 0.6f;
        private const float MaxProbability = 5f;
        private const float MaxFontSize = 6f;
        public int Value { get; private set; }
        [SerializeField] private TextMeshPro numberText;
        [SerializeField] private TextMeshPro probabilityText;
        public void SetValue(int value, int probability)
        {
            Value = value;
            numberText.color = probability < 5 ? Color.black : Color.red;
            probabilityText.color = probability < 5 ? Color.black : Color.red;
            numberText.text = value.ToString();
            numberText.fontSize = MaxFontSize - GetScaleFactor(probability);
            probabilityText.text = NumbersHelper.GetProbabilityString(probability);
        }
        
        private static float GetScaleFactor(int probability)
        {
            return (MaxProbability - probability) * ScaleFactor;
        }
    }
}