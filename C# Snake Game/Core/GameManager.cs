using System;
using System.Drawing;
using System.Windows.Forms;
using CleanSnakeGame.Core;
using CleanSnakeGame.Data;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Services
{
    public class GameManager
    {
        private readonly GameState state;
        private readonly GameEngine engine;

        private readonly Timer gameTimer;

        private readonly Action updateUI;
        private readonly Action showGameOverDialog;
        private readonly Panel pausePanel;

        public GameManager(
            GameState state,
            GameEngine engine,
            Timer timer,
            Panel pausePanel,
            Action updateUI,
            Action showGameOverDialog)
        {
            this.state = state;
            this.engine = engine;
            this.gameTimer = timer;
            this.pausePanel = pausePanel;
            this.updateUI = updateUI;
            this.showGameOverDialog = showGameOverDialog;
        }

        public void InitializeGame()
        {
            state.Reset();

            state.Speed = SettingsManager.Settings.GetGameSpeed();
            gameTimer.Interval = state.Speed;

            engine.GenerateFood();

            state.GameRunning = true;
            state.GamePaused = false;

            gameTimer.Start();

            updateUI();
        }

        public void RestartGame()
        {
            gameTimer.Stop();

            InitializeGame();

            pausePanel.Visible = false;
            state.GamePaused = false;
        }

        public void GameOver()
        {
            state.GameRunning = false;
            state.GameOver = true;

            gameTimer.Stop();

            // Update best score
            if (state.Score > SettingsManager.Settings.BestScore)
            {
                SettingsManager.Settings.BestScore = state.Score;
                SettingsManager.SaveSettings();
            }

            // Save score to database
            try
            {
                Database.AddScore(
                    SettingsManager.Settings.PlayerName,
                    state.Score,
                    state.Level,
                    DateTime.Now
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save score: {ex.Message}");
            }

            updateUI();

            showGameOverDialog();
        }

        public void TogglePause()
        {
            if (!state.GameRunning) return;

            state.GamePaused = !state.GamePaused;

            pausePanel.Visible = state.GamePaused;

            if (state.GamePaused)
                gameTimer.Stop();
            else
                gameTimer.Start();
        }
    }
}