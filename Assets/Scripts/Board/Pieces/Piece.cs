using Building;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Board.Pieces
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

        public void Initialize(int player_id)
        {
            SetPlayer(player_id);
        }

        private void SetPlayer(int player_id)
        {
            player = player_id;
            mesh.material.color = BuildManager.Current.GetPlayerColor(player);
        }
    }
}