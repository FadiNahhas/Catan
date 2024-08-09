using Board;
using Board.Generation;
using Board.Pieces;
using Event_Bus;
using Helpers;
using Board.Pieces.Libraries;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Building
{
    public class BuildManager : Singleton<BuildManager>
    {
        [TabGroup("Prefabs"), SerializeField] public TilePropsLibrary tilePropsLibrary;
        [TabGroup("Prefabs"), SerializeField] public PiecesLibrary piecesLibrary;
        [TabGroup("Libraries"), SerializeField] public PlayerColors playerColors;
        [TabGroup("State")] public int currentPlayer = 0;

        #region Build Methods
        
        public Piece Build(BuildingType type, Vector3 pos)
        {
            pos.y = BoardGenerator.HexThickness;
            var piece = Instantiate(piecesLibrary.GetPrefab(type), pos, Quaternion.identity);
            piece.Initialize(currentPlayer);
            var e = new BuildEvent(type, currentPlayer);
            return piece;
        }
        
        public Piece Build(BuildingType type, Vector3 pos, Quaternion rotation)
        {
            pos.y = BoardGenerator.HexThickness;
            var piece = Instantiate(piecesLibrary.GetPrefab(type), pos, rotation);
            piece.Initialize(currentPlayer);
            var e = new BuildEvent(type, currentPlayer);
            return piece;
        }
        
        #endregion

        public Color GetPlayerColor(int player) => playerColors.GetColor(player);

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
