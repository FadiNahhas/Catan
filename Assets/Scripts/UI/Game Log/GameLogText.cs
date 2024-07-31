using TMPro;
using UnityEngine;

namespace UI.Game_Log
{
    public class GameLogText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void Init(string message)
        {
            text.text = message;
        }
    }
}