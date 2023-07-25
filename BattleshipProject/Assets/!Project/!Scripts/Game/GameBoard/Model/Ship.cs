using System.Collections.Generic;
using System.Linq;

namespace tonigames.battleship.Game.GameBoard.Model
{
    public class Ship
    {
        private List<GameTile> tiles;

        public List<GameTile> Tiles => this.tiles;

        public bool IsDestroyed => this.tiles.All(tile => tile.Value == GameTile.Status.ShipHit) || this.tiles.All(tile => tile.Value == GameTile.Status.Destroyed);

        public Ship(List<GameTile> tiles)
        {
            this.tiles = tiles;
        }
    }
}