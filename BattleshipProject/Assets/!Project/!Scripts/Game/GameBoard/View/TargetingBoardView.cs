using System;
using tonigames.battleship.Game.GameBoard.Controller;
using tonigames.battleship.Game.GameBoard.Model;
using tonigames.battleship.MVC.Base;
using UnityEngine;
using UnityEngine.UI;

namespace tonigames.battleship.Game.GameBoard.View
{
    public class TargetingBoardView : View<PlayerBoard, TargetingBoardController<PlayerBoard>>
    {
        public static event Action UserPlayed;

        [SerializeField] private int playerID;
        [SerializeField] private PlayerBoardView opponentBoard;
        [SerializeField] private Transform container;
        [SerializeField] private GameObject raycastBlock;
        [SerializeField] private GameObject uiGrid;
        [SerializeField] private GameObject targetingButtonPrefab;
        [SerializeField] private Color targetButtonHitColor;
        [SerializeField] private Color targetButtonMissColor;
        [SerializeField] private Color targetButtonDisabledColor;

        private Button[,] targetButtons = new Button[BoardProperties.BoardSize, BoardProperties.BoardSize];

        public void Show()
        {
            this.container.gameObject.SetActive(true);

            this.UpdateView();
        }

        public void Restart()
        {
            this.Controller.Restart();
            this.UpdateView();
        }

        private void UpdateView()
        {
            for (var col = 0; col < BoardProperties.BoardSize; col++)
            {
                for (var row = 0; row < BoardProperties.BoardSize; row++)
                {
                    var tile = this.opponentBoard.Model.Board[col, row];
                    var colors = this.targetButtons[col, row].colors;
                    if (tile.Value == GameTile.Status.ShipHit)
                    {
                        colors.disabledColor = this.targetButtonHitColor;
                        this.targetButtons[col, row].colors = colors;
                        this.targetButtons[col, row].interactable = false;
                    } else if (tile.Value == GameTile.Status.Destroyed)
                    {
                        colors.disabledColor = this.targetButtonDisabledColor;
                        this.targetButtons[col, row].colors = colors;
                        this.targetButtons[col, row].interactable = false;
                    } else if (tile.Value == GameTile.Status.HitMiss)
                    {
                        colors.disabledColor = this.targetButtonMissColor;
                        this.targetButtons[col, row].colors = colors;
                        this.targetButtons[col, row].interactable = false;
                    }
                    else
                    {
                        this.targetButtons[col, row].interactable = true;
                    }
                }
            }
        }

        public void Hide()
        {
            this.container.gameObject.SetActive(false);
        }

        private void Setup()
        {
            for (var i = 0; i < BoardProperties.BoardSize; i++)
            {
                for (var j = 0; j < BoardProperties.BoardSize; j++)
                {
                    var targetButton = Instantiate(this.targetingButtonPrefab, this.uiGrid.transform).GetComponent<Button>();
                    var row = j;
                    var col = i;
                    targetButton.onClick.AddListener(delegate { this.OnTargetClick(row, col, targetButton); });
                    this.targetButtons[i, j] = targetButton;
                }
            }

            this.Controller.GenerateBoard();
        }

        private void OnTargetClick(int row, int col, Button targetButton)
        {
            var shipHit = false;

            this.Controller.RegisterHit(row, col, this.opponentBoard);

            targetButton.interactable = false;
            var colors = targetButton.colors;

            if (this.Model.GetTile(row, col).Value is GameTile.Status.ShipHit or GameTile.Status.Destroyed)
            {
                colors.disabledColor = this.targetButtonHitColor;
                targetButton.colors = colors;
                shipHit = true;
            }
            else
            {
                colors.disabledColor = this.targetButtonMissColor;
                targetButton.colors = colors;
            }

            this.UpdateView();

            if (shipHit) return;

            this.raycastBlock.gameObject.SetActive(true);
            OnUserPlayed();
        }

        private static void OnUserPlayed()
        {
            UserPlayed?.Invoke();
        }

        private void TurnManagerOnNextTurn(int playerID)
        {
            if (playerID == this.playerID)
            {
                this.raycastBlock.gameObject.SetActive(false);
            }
        }

        public override void Awake()
        {
            this.Model = new PlayerBoard();
            base.Awake();
        }

        private void Start()
        {
            this.Setup();
        }

        private void OnEnable()
        {
            GameManager.NextTurn += this.TurnManagerOnNextTurn;
        }

        private void OnDisable()
        {
            GameManager.NextTurn -= this.TurnManagerOnNextTurn;
        }
    }
}