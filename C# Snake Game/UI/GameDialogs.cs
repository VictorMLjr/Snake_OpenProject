using System;
using System.Drawing;
using System.Windows.Forms;
using CleanSnakeGame.Data;
using CleanSnakeGame.Services;

namespace CleanSnakeGame.UI
{
    public class GameDialogs
    {
        private readonly Form form;
        private readonly GameState state;
        private readonly GameManager manager;

        public GameDialogs(Form targetForm, GameState gameState, GameManager gameManager)
        {
            form = targetForm;
            state = gameState;
            manager = gameManager;
        }

        public void ShowPauseMenu()
        {
            if (!state.GameRunning) return;

            state.GamePaused = true;

            Panel pauseMenuPanel = new Panel
            {
                Size = new Size(350, 400),
                BackColor = Color.FromArgb(40, 45, 55),
                Location = new Point(
                    (form.Width - 350) / 2,
                    (form.Height - 400) / 2
                )
            };

            Label title = new Label
            {
                Text = "GAME PAUSED",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 255),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(330, 40),
                Location = new Point(10, 20)
            };

            Button resumeButton = CreateButton("RESUME", 80, Color.FromArgb(50, 200, 50));
            resumeButton.Click += (s, e) =>
            {
                form.Controls.Remove(pauseMenuPanel);
                manager.TogglePause();
            };

            Button settingsButton = CreateButton("SETTINGS", 150, Color.FromArgb(70, 130, 255));
            settingsButton.Click += (s, e) =>
            {
                SettingsForm settingsForm = new SettingsForm();
                settingsForm.ShowDialog();
            };

            Button mainMenuButton = CreateButton("MAIN MENU", 220, Color.Orange);
            mainMenuButton.Click += (s, e) =>
            {
                form.Controls.Remove(pauseMenuPanel);
                form.Close();
            };

            Button quitButton = CreateButton("QUIT GAME", 290, Color.Red);
            quitButton.Click += (s, e) =>
            {
                Application.Exit();
            };

            pauseMenuPanel.Controls.Add(title);
            pauseMenuPanel.Controls.Add(resumeButton);
            pauseMenuPanel.Controls.Add(settingsButton);
            pauseMenuPanel.Controls.Add(mainMenuButton);
            pauseMenuPanel.Controls.Add(quitButton);

            form.Controls.Add(pauseMenuPanel);
            pauseMenuPanel.BringToFront();
        }

        public void ShowGameOverDialog()
        {
            Panel gameOverPanel = new Panel
            {
                Size = new Size(400, 300),
                BackColor = Color.FromArgb(40, 45, 55),
                Location = new Point(
                    (form.Width - 400) / 2,
                    (form.Height - 300) / 2
                )
            };

            Label title = new Label
            {
                Text = "GAME OVER",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 100, 100),
                Size = new Size(380, 50),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label scoreLabel = new Label
            {
                Text = $"Score: {state.Score}\nBest: {SettingsManager.Settings.BestScore}\nLevel: {state.Level}",
                Font = new Font("Arial", 14),
                ForeColor = Color.White,
                Size = new Size(380, 80),
                Location = new Point(10, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button yesButton = CreateButton("YES", 200, Color.FromArgb(50, 200, 50));
            yesButton.Location = new Point(80, 220);
            yesButton.Click += (s, e) =>
            {
                form.Controls.Remove(gameOverPanel);
                manager.RestartGame();
            };

            Button noButton = CreateButton("NO", 200, Color.FromArgb(200, 50, 50));
            noButton.Location = new Point(220, 220);
            noButton.Click += (s, e) =>
            {
                form.Close();
            };

            gameOverPanel.Controls.Add(title);
            gameOverPanel.Controls.Add(scoreLabel);
            gameOverPanel.Controls.Add(yesButton);
            gameOverPanel.Controls.Add(noButton);

            form.Controls.Add(gameOverPanel);
            gameOverPanel.BringToFront();
        }

        private Button CreateButton(string text, int y, Color color)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Size = new Size(250, 50),
                Location = new Point(50, y),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }
    }
}