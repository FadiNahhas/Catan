﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lobby
{
    [CreateAssetMenu(fileName = "Ready Status Colors", menuName = "Catan/Ready Status Colors")]
    public class ReadyStatusColors : ScriptableObject
    {
        [SerializeField] private List<Helpers.KeyValuePair<ReadyStatus, Color>> statusColors = new();
        
        public Color GetColor(ReadyStatus _status)
        {
            foreach (var pair in statusColors.Where(_pair => _pair.key == _status))
            {
                return pair.value;
            }

            return Color.white;
        }
    }
}