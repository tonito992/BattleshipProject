using TMPro;
using UnityEngine;

namespace tonigames.battleship.Game.GameBoard.View
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text gameOverText;

        public void Setup(int winnerPlayerID)
        {
            this.gameOverText.SetText($"Winner is P{winnerPlayerID}");
        }
    }
}