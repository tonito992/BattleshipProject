using tonigames.battleship.Game.GameBoard.View;
using UnityEngine;

namespace tonigames.battleship.Game.GameBoard
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerBoardView playerBoardView;
        [SerializeField] private TargetingBoardView targetingBoardView;

        public void Show()
        {
            this.playerBoardView.Show();
            this.targetingBoardView.Show();
        }

        public void Hide()
        {
            this.playerBoardView.Hide();
            this.targetingBoardView.Hide();
        }

        public void Restart()
        {
            this.targetingBoardView.Restart();
            this.playerBoardView.Restart();
        }
    }
}