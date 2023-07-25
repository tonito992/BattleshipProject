using System;

namespace tonigames.battleship.Game.GameBoard.Model
{
    [Serializable]
    public class GameTile
    {
        public enum Status
        {
            Empty,
            Ship,
            Neighbour,
            ShipHit,
            HitMiss,
            Destroyed
        }

        public Status Value => this.value;
        public int Column => this.column;
        public int Row => this.row;

        private Status value;
        private int column;
        private int row;

        public GameTile(Status value, int column, int row)
        {
            this.value = value;
            this.column = column;
            this.row = row;
        }

        public void SetStatus(Status value)
        {
            this.value = value;
        }

        public string ToStringShort()
        {
            return Value switch
            {
                Status.Empty => "E",
                Status.Ship => "S",
                Status.Neighbour => "N",
                Status.ShipHit => "H",
                Status.Destroyed => "D",
                Status.HitMiss => "M",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}