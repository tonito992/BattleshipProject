using System;
using System.Collections.Generic;
using System.Linq;
using tonigames.battleship.Game.GameBoard.Model;
using tonigames.battleship.MVC.Base;
using tonigames.battleship.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace tonigames.battleship.Game.GameBoard.Controller
{
    public class PlayerBoardController<TM> : Controller<TM> where TM : PlayerBoard
    {
        public static event Action AllShipsDestroyed;

        private GameTile[,] gameBoard = new GameTile[BoardProperties.BoardSize, BoardProperties.BoardSize];
        private List<Ship> ships = new();
        private bool stopGenerating;
        private const int MaxIterationsPerShip = 500;

        private enum Direction
        {
            Horizontal,
            Vertical
        }

        private readonly int[] shipDimensions = { 5, 4, 3, 2, 2, 1, 1 };

        public void GenerateBoard()
        {
            var startTime = Time.realtimeSinceStartup;
            for (var col = 0; col < BoardProperties.BoardSize; col++)
            {
                for (var row = 0; row < BoardProperties.BoardSize; row++)
                {
                    this.gameBoard[col, row] = new GameTile(GameTile.Status.Empty, col, row);
                }
            }

            this.GenerateShips();
            this.DebugBoard();
            this.Model.Board = this.gameBoard;

            Debug.Log($"Board generation time: {Time.realtimeSinceStartup - startTime}");
        }

        private void GenerateShips()
        {
            this.ships = new List<Ship>();

            this.ResetBoard();

            foreach (var shipSize in this.shipDimensions)
            {
                this.PlaceShip(Random.Range(0, this.gameBoard.GetLength(0)),
                    Random.Range(0, this.gameBoard.GetLength(1)),
                    shipSize);

                if (this.stopGenerating)
                {
                    this.stopGenerating = false;
                    ArrayRandomizer.RandomizeArray(this.shipDimensions);
                    this.GenerateShips();
                    break;
                }
            }
        }

        private void ResetBoard()
        {
            for (var col = 0; col < this.gameBoard.GetLength(0); col++)
            {
                for (var row = 0; row < this.gameBoard.GetLength(1); row++)
                {
                    this.gameBoard[col, row].SetStatus(GameTile.Status.Empty);
                }
            }
        }

        private void PlaceShip(int rowStart, int colStart, int size)
        {
            var shipPlaced = false;
            var iteration = 0;

            while (!shipPlaced)
            {
                iteration++;
                var tempBoard = (GameTile[,])this.gameBoard.Clone();
                var shipValid = true;
                var shipTiles = new List<GameTile>();

                var direction = GetRandomDirection();
                for (var i = 0; i < size; i++)
                {
                    if (direction == Direction.Vertical)
                    {
                        if (this.IsValidAndEmptyCoordinate(rowStart + i, colStart))
                        {
                            tempBoard[colStart, rowStart + i].SetStatus(GameTile.Status.Ship);
                            shipTiles.Add(tempBoard[colStart, rowStart + i]);
                        }
                        else
                        {
                            shipValid = false;
                        }
                    }
                    else if (direction == Direction.Horizontal)
                    {
                        if (this.IsValidAndEmptyCoordinate(rowStart, colStart + i))
                        {
                            tempBoard[colStart + i, rowStart].SetStatus(GameTile.Status.Ship);
                            shipTiles.Add(tempBoard[colStart + i, rowStart]);
                        }
                        else
                        {
                            shipValid = false;
                        }
                    }
                }

                if (shipValid)
                {
                    var ship = new Ship(shipTiles);
                    this.ships.Add(ship);
                    this.gameBoard = tempBoard.Clone() as GameTile[,];
                    this.UpdateNeighboringTiles(shipTiles);
                    shipPlaced = true;
                }

                if (iteration > MaxIterationsPerShip)
                {
                    this.stopGenerating = true;
                    break;
                }
            }
        }

        private bool IsValidAndEmptyCoordinate(int row, int col)
        {
            if (!this.IsCoordinateInsideBoard(row, col))
            {
                return false;
            }

            return this.gameBoard[col, row].Value == GameTile.Status.Empty;
        }

        private bool IsCoordinateInsideBoard(int row, int col)
        {
            if (row < 0 || col < 0) return false;
            return row < this.gameBoard.GetLength(0) && col < this.gameBoard.GetLength(1);
        }

        private void UpdateNeighboringTiles(List<GameTile> shipTiles)
        {
            foreach (var tile in shipTiles)
            {
                for (var col = tile.Column - 1; col <= tile.Column + 1; col++)
                {
                    for (var row = tile.Row - 1; row <= tile.Row + 1; row++)
                    {
                        if (this.IsValidAndEmptyCoordinate(row, col))
                        {
                            this.gameBoard[col, row].SetStatus(GameTile.Status.Neighbour);
                        }
                    }
                }
            }
        }

        public void OnTileAttack(int row, int col)
        {
            foreach (var ship in this.ships)
            {
                foreach (var tile in ship.Tiles)
                {
                    if (tile.Column == col && tile.Row == row)
                    {
                        tile.SetStatus(GameTile.Status.ShipHit);
                    }
                }
            }

            if (this.gameBoard[col, row].Value == GameTile.Status.ShipHit)
            {
                this.DestroyedShipsUpdate();
            }
            else
            {
                this.gameBoard[col, row].SetStatus(GameTile.Status.HitMiss);
            }

            this.DebugBoard();
        }

        private void DestroyedShipsUpdate()
        {
            foreach (var ship in this.ships.Where(ship => ship.IsDestroyed))
            {
                foreach (var tile in ship.Tiles)
                {
                    for (var i = tile.Column - 1; i <= tile.Column + 1; i++)
                    {
                        for (var j = tile.Row - 1; j <= tile.Row + 1; j++)
                        {
                            if (this.IsCoordinateInsideBoard(j, i))
                            {
                                    this.gameBoard[i, j].SetStatus(GameTile.Status.Destroyed);
                            }
                        }
                    }
                }
            }
        }

        private static Direction GetRandomDirection() =>
            (Direction)Random.Range(0, Enum.GetValues(typeof(Direction)).Length);

        private void DebugBoard()
        {
            var output = "";
            for (var col = 0; col < this.gameBoard.GetLength(1); col++)
            {
                for (var row = 0; row < this.gameBoard.GetLength(0); row++)
                {
                    output += this.gameBoard[col, row].ToStringShort() + ",";
                }

                output += "\n";
            }

            Debug.Log(output);
        }

        public bool AreShipsDestroyed()
        {
            return this.ships.All(ship => ship.IsDestroyed);
        }
    }
}