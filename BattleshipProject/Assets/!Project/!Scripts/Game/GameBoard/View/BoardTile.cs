using UnityEngine;

namespace tonigames.battleship.Game.GameBoard.View
{
    public class BoardTile : MonoBehaviour
    {
        public int Row => this.row;
        public int Col => this.col;

        [SerializeField] private Material tileTargetedMaterial;

        private int col;
        private int row;

        private MeshRenderer meshRenderer;

        public void Setup(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public void TileTargetedStatus()
        {
            this.meshRenderer.material = this.tileTargetedMaterial;
        }

        private void Awake()
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }
    }
}