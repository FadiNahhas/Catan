using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lobby
{
    [CreateAssetMenu(fileName = "Ready Status Colors", menuName = "Catan/Ready Status Colors")]
    public class ReadyStatusColors : ScriptableObject
    {
        [SerializeField] private List<Helpers.KeyValuePair<ReadyStatus, Color>> statusColors = new();
        
        public Color GetColor(ReadyStatus status)
        {
            foreach (var pair in statusColors.Where(pair => pair.key == status))
            {
                return pair.value;
            }

            return Color.white;
        }
    }
}