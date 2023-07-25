using System;
using System.Collections.Generic;
using tonigames.battleship.Game.GameBoard.Controller;
using tonigames.battleship.Game.GameBoard.Model;
using tonigames.battleship.MVC.Base;
using UnityEngine;

namespace tonigames.battleship.Game.GameBoard.View
{
    public class PlayerBoardView : View<PlayerBoard, PlayerBoardController<PlayerBoard>>
    {
        public static event Action AllShipsDestroyed;

        [SerializeField] private Transform viewContainer;
        [SerializeField] private Transform shipContainer;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private float tileSize;
        [SerializeField] private GameObject shipPrefab;

        private List<ShipSegment> shipSegments = new();
        private BoardTile[,] boardTiles = new BoardTile[BoardProperties.BoardSize, BoardProperties.BoardSize];
        private bool[,] shots = new bool[BoardProperties.BoardSize, BoardProperties.BoardSize];
        private readonly int zOffset = 5;

        public void Show()
        {
            this.viewContainer.gameObject.SetActive(true);

            foreach (var shipSegment in this.shipSegments)
            {
                if (this.Model.Board[shipSegment.Col, shipSegment.Row].Value == GameTile.Status.Destroyed)
                {
                    shipSegment.HideShipSegment();
                }

                if (this.Model.Board[shipSegment.Col, shipSegment.Row].Value == GameTile.Status.ShipHit)
                {
                    shipSegment.ShipHitStatus();
                }
            }

            for (var col = 0; col < BoardProperties.BoardSize; col++)
            {
                for (var row = 0; row < BoardProperties.BoardSize; row++)
                {
                    if (this.shots[col, row])
                    {
                        this.boardTiles[col, row].TileTargetedStatus();
                    }
                }
            }
        }

        public void Hide()
        {
            this.viewContainer.gameObject.SetActive(false);
        }

        public override void Awake()
        {
            this.Model = new PlayerBoard();
            base.Awake();
        }

        public void Restart()
        {
            foreach (var shipSegment in this.shipSegments)
            {
                Destroy(shipSegment.gameObject);
            }

            this.shipSegments.Clear();

            foreach (var tile in this.boardTiles)
            {
                Destroy(tile.gameObject);
            }

            this.shots = new bool[BoardProperties.BoardSize, BoardProperties.BoardSize];

            this.Initialize();
        }

        private void Initialize()
        {
            this.Controller.GenerateBoard();

            for (var i = 0; i < BoardProperties.BoardSize; i++)
            {
                for (var j = 0; j < BoardProperties.BoardSize; j++)
                {
                    var tile = Instantiate(this.tilePrefab, this.shipContainer).GetComponent<BoardTile>();
                    tile.Setup(i, j);
                    this.boardTiles[j, i] = tile;
                    tile.transform.localPosition =
                        new Vector3(i * this.tileSize, 0.5f, -j * this.tileSize - this.zOffset);

                    if (this.Model.Board[j, i].Value == GameTile.Status.Ship)
                    {
                        var ship = Instantiate(this.shipPrefab, this.shipContainer).GetComponent<ShipSegment>();
                        ship.Setup(i, j);
                        this.shipSegments.Add(ship);
                        ship.transform.localPosition =
                            new Vector3(i * this.tileSize, 1, -j * this.tileSize - this.zOffset);
                    }
                }
            }
        }

        private void Start()
        {
            this.Initialize();
        }

        public void RegisterHit(int row, int col)
        {
            this.Controller.OnTileAttack(row, col);
            this.shots[col, row] = true;

            if (this.Controller.AreShipsDestroyed())
            {
                this.OnAllShipsDestroyed();
            }
        }

        private void OnAllShipsDestroyed()
        {
            AllShipsDestroyed?.Invoke();
        }
    }
}