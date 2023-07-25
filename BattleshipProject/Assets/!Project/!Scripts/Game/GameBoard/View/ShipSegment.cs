using UnityEngine;

namespace tonigames.battleship.Game.GameBoard.View
{
    public class ShipSegment : MonoBehaviour
    {
        public int Row => this.row;
        public int Col => this.col;

        [SerializeField] private ParticleSystem destroyedEffect;
        private MeshRenderer meshRenderer;
        private int row;
        private int col;

        public void Setup(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        private void Awake()
        {
            this.meshRenderer = this.GetComponent<MeshRenderer>();
        }

        public void ShipHitStatus()
        {
            this.destroyedEffect.Play();
        }

        public void HideShipSegment()
        {
            this.meshRenderer.enabled = false;
            this.destroyedEffect.gameObject.SetActive(false);
        }
    }
}