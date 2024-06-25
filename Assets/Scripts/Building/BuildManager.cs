using System.Collections.Generic;
using Helpers;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Building
{
    public class BuildManager : Singleton<BuildManager>
    {
        [TabGroup("Prefabs")] public GameObject settlementPrefab;
        [TabGroup("Prefabs")] public GameObject roadPrefab;
        [TabGroup("Prefabs")] public GameObject cityPrefab;
        
        [TabGroup("Colors")][SerializeField] private List<Color> playerColors;
        
        public int currentPlayer = 0;

        public GameObject Build(BuildingType type, Vector3 pos)
        {
            pos.y = HexGrid.Current.HexThickness;
            var obj = Instantiate(GetPrefab(type), pos, Quaternion.identity);
            obj.GetComponent<MeshRenderer>().material.color = GetPlayerColor(currentPlayer);
            return obj;
        }
        
        public GameObject Build(BuildingType type, Vector3 pos, Quaternion rotation)
        {
            pos.y = HexGrid.Current.HexThickness;
            var obj = Instantiate(GetPrefab(type), pos, rotation);
            obj.GetComponent<MeshRenderer>().material.color = GetPlayerColor(currentPlayer);
            return obj;
        }
    
        private GameObject GetPrefab(BuildingType type)
        {
            switch (type)
            {
                case BuildingType.Settlement:
                    return settlementPrefab;
                case BuildingType.City:
                    return cityPrefab;
                case BuildingType.Road:
                    return roadPrefab;
                default:
                    return null;
            }
        }

        private Color GetPlayerColor(int player)
        {
            if (player < 0 || player >= playerColors.Count) return Color.white;
            return playerColors[player];
        }
    }
}
