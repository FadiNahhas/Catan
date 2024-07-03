using Board;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Building.Pieces
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Piece : MonoBehaviour
    {
        [SerializeField, ReadOnly] private int player;
        public int Player => player;

        [SerializeField, ReadOnly] private MeshRenderer mesh;

        [SerializeField] private BuildingType type;
        public BuildingType Type => type;
        
        private void Awake()
        {
            mesh = GetComponent<MeshRenderer>();
        }

        public void Initialize(int playerId)
        {
            SetPlayer(playerId);
        }

        private void SetPlayer(int playerId)
        {
            player = playerId;
            mesh.material.color = BuildManager.Current.GetPlayerColor(player);
        }
    }
}