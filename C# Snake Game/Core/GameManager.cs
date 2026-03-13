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
        private readonly Action showPauseMenu;

        private readonly Timer gameTimer;

        private readonly Action updateUI;
        private readonly Action showGameOverDialog;
        private readonly Panel gamePanel;
        private readonly Action hidePauseMenu;

        public GameManager(
            GameState state,
            GameEngine engine,
            Timer timer,
            Panel gamePanel,
            Action updateUI,
            Action showGameOverDialog,
            Action showPauseMenu,
            Action hidePauseMenu)
        {
            this.state = state;
            this.engine = engine;
            this.gameTimer = timer;
            this.gamePanel = gamePanel;
            this.updateUI = updateUI;
            this.showGameOverDialog = showGameOverDialog;
            this.showPauseMenu = showPauseMenu;
            this.hidePauseMenu = hidePauseMenu;
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
            
            state.GamePaused = false;

            gamePanel.Invalidate();
            gamePanel.Focus();

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

            if (state.GamePaused)
            {
                state.GamePaused = false;
                gameTimer.Start();

                hidePauseMenu?.Invoke();
            }
            else
            {
                state.GamePaused = true;
                gameTimer.Stop();

                showPauseMenu?.Invoke();
            }
        }
    }
}