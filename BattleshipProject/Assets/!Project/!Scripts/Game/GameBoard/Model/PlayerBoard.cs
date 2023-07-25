using tonigames.battleship.MVC.Base;

namespace tonigames.battleship.Game.GameBoard.Model
{
    [System.Serializable]
    public class PlayerBoard : BaseModel
    {
        public GameTile[,] Board
        {
            get => this.board;
            set => this.board = value;
        }

        private GameTile[,] board;

        public void SetTile(GameTile.Status status, int row, int col)
        {
            this.board[col, row].SetStatus(status);
        }

        public GameTile GetTile(int row, int col)
        {
            return this.board[col, row];
        }
    }
}