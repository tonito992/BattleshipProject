using System;
using System.Collections.Generic;
using tonigames.battleship.Game.GameBoard;
using tonigames.battleship.Game.GameBoard.View;
using UnityEngine;

namespace tonigames.battleship.Game
{
    public class GameManager : MonoBehaviour
    {
        public static event Action<int> NextTurn;

        [SerializeField] private PlayerManager playerOnePlayerBoard;
        [SerializeField] private PlayerManager playerTwoPlayerBoard;
        [SerializeField] private int timeBeforePlayerSwitch;
        [SerializeField] private GameOverScreen gameOverScreen;

        private List<PlayerManager> players = new();

        private int currentPlayer = 0;

        public void OnPlayAgain()
        {
            this.players.ForEach(player => player.Restart());
            this.gameOverScreen.gameObject.SetActive(false);
            this.currentPlayer = 0;
            this.RefreshPlayerVisibility();
        }

        private void Awake()
        {
            this.players.Add(this.playerOnePlayerBoard);
            this.players.Add(this.playerTwoPlayerBoard);
        }

        private void Start()
        {
            this.gameOverScreen.gameObject.SetActive(false);
            this.RefreshPlayerVisibility();
        }

        private void OnEnable()
        {
            TargetingBoardView.UserPlayed += this.TargetingBoardViewOnUserPlayed;
            PlayerBoardView.AllShipsDestroyed += this.PlayerBoardViewOnAllShipsDestroyed;
        }

        private void OnDisable()
        {
            TargetingBoardView.UserPlayed -= this.TargetingBoardViewOnUserPlayed;
            PlayerBoardView.AllShipsDestroyed -= this.PlayerBoardViewOnAllShipsDestroyed;
        }

        private void TargetingBoardViewOnUserPlayed()
        {
            this.currentPlayer = this.currentPlayer == 0 ? 1 : 0;
            this.Invoke(nameof(this.RefreshPlayerVisibility), this.timeBeforePlayerSwitch);
        }

        private void PlayerBoardViewOnAllShipsDestroyed()
        {
            this.gameOverScreen.gameObject.SetActive(true);
            this.gameOverScreen.Setup(this.currentPlayer + 1);
            this.players.ForEach(player => player.Hide());
        }

        private void RefreshPlayerVisibility()
        {
            for (var i = 0; i < this.players.Count; i++)
            {
                if (i == this.currentPlayer)
                {
                    this.players[i].Show();
                }
                else
                {
                    this.players[i].Hide();
                }
            }

            OnNextTurn(this.currentPlayer);
        }

        private static void OnNextTurn(int playerID)
        {
            NextTurn?.Invoke(playerID);
        }
    }
}
