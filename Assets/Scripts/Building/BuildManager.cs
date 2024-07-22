using System.Collections.Generic;
using Board;
using Building.Libraries;
using Building.Pieces;
using Helpers;
using Hex;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Building
{
    public class BuildManager : Singleton<BuildManager>
    {
        [TabGroup("Prefabs"), SerializeField] public TilePropsLibrary tilePropsLibrary;
        [TabGroup("Prefabs"), SerializeField] public PiecesLibrary piecesLibrary;
        
        [TabGroup("Colors")][SerializeField] private List<Color> playerColors;
        
        public int currentPlayer = 0;

        public Piece Build(BuildingType type, Vector3 pos)
        {
            pos.y = HexGrid.HexThickness;
            var piece = Instantiate(piecesLibrary.GetPrefab(type), pos, Quaternion.identity);
            piece.Initialize(currentPlayer);
            return piece;
        }
        
        public Piece Build(BuildingType type, Vector3 pos, Quaternion rotation)
        {
            pos.y = HexGrid.HexThickness;
            var piece = Instantiate(piecesLibrary.GetPrefab(type), pos, rotation);
            piece.Initialize(currentPlayer);
            return piece;
        }
        
        public Color GetPlayerColor(int player)
        {
            if (player < 0 || player >= playerColors.Count) return Color.white;
            return playerColors[player];
        }

        public GameObject SpawnProps(Tile tile)
        {
            var prefab = tilePropsLibrary.GetPrefab(tile.Resource);
            if (prefab == null) return null;
            
            var prop = Instantiate(prefab, tile.transform);
            prop.transform.localPosition = Vector3.zero;
            prop.transform.localRotation = Quaternion.identity;
            return prop;
        }
    }
}
