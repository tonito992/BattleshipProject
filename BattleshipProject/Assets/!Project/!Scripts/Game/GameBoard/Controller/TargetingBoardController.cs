using tonigames.battleship.Game.GameBoard.Model;
using tonigames.battleship.Game.GameBoard.View;
using tonigames.battleship.MVC.Base;

namespace tonigames.battleship.Game.GameBoard.Controller
{
    public class TargetingBoardController<TM> : Controller<TM> where TM : PlayerBoard
    {
        private GameTile[,] targetingBoard =
            new GameTile[BoardProperties.BoardSize, BoardProperties.BoardSize];

        public void RegisterHit(int row, int col, PlayerBoardView opponentBoard)
        {
            opponentBoard.RegisterHit(row, col);
            this.Model.SetTile(opponentBoard.Model.Board[col, row].Value, row, col);
        }

        public void GenerateBoard()
        {
            for (var col = 0; col < BoardProperties.BoardSize; col++)
            {
                for (var row = 0; row < BoardProperties.BoardSize; row++)
                {
                    this.targetingBoard[col, row] = new GameTile(GameTile.Status.Empty, col, row);
                }
            }

            this.Model.Board = this.targetingBoard;
        }

        public void Restart()
        {
            this.GenerateBoard();
        }
    }
}