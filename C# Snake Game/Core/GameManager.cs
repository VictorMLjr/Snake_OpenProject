using System;
using System.Drawing;
using CleanSnakeGame.Data;
using CleanSnakeGame.Services;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Services
{
    public class GameManager
    {
        private readonly GameState gameState;
        private readonly Random random;

        public GameManager(GameState state)
        {
            gameState = state;
            random = new Random();
        }

        public void InitializeGame()
        {
            gameState.Snake.Clear();

            gameState.Snake.Add(new Point(10, 10));
            gameState.Snake.Add(new Point(9, 10));
            gameState.Snake.Add(new Point(8, 10));

            gameState.Direction = Direction.Right;
            gameState.NextDirection = Direction.Right;

            GenerateFood();

            gameState.Powerups.Clear();
            gameState.Obstacles.Clear();
            gameState.PowerupTimers.Clear();
            gameState.ObstacleTimers.Clear();

            gameState.Score = 0;
            gameState.Level = 1;
            gameState.Speed = SettingsManager.Settings.GetGameSpeed();

            gameState.GameRunning = true;
            gameState.GamePaused = false;
        }

        public void RestartGame()
        {
            InitializeGame();
        }

        public void GameOver()
        {
            gameState.GameRunning = false;

            if (gameState.Score > SettingsManager.Settings.BestScore)
            {
                SettingsManager.Settings.BestScore = gameState.Score;
                SettingsManager.SaveSettings();
            }
            try
            {
                Database.AddScore(
                    SettingsManager.Settings.PlayerName,
                    gameState.Score,
                    gameState.Level,
                    DateTime.Now
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save score: {ex.Message}");
            }
        }

        public void TogglePause()
        {
            if (!gameState.GameRunning)
                return;
            gameState.GamePaused = !gameState.GamePaused;
        }
        private void GenerateFood()
        {
            int x = random.Next(0, 40);
            int y = random.Next(0, 30);

            gameState.Food = new Point(x, y);
        }
    }
}