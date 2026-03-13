using System;
using System.Drawing;
using System.Windows.Forms;
using CleanSnakeGame.Core;
using CleanSnakeGame.Data;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Services
{
    public class GameDialogs
    {
        private readonly Form form;
        private readonly GameState state;
        private readonly GameManager manager;
        private Panel pausePanel;

        public GameDialogs(Form form, GameState state, GameManager manager)
        {
            this.form = form;
            this.state = state;
            this.manager = manager;
        }

        public void ShowPauseMenu()
        {
            if (!state.GameRunning) return;

            pausePanel = new Panel
            {
                Size = new Size(350, 300),
                BackColor = Color.FromArgb(40, 45, 55),
                Location = new Point((form.Width - 350) / 2, (form.Height - 300) / 2)
            };

            var title = new Label
            {
                Text = "GAME PAUSED",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(70, 130, 255),
                Size = new Size(330, 40),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var resume = CreateButton("RESUME", 80, Color.FromArgb(50, 200, 50));
            resume.Click += (s, e) =>
            {
                form.Controls.Remove(pausePanel);
                manager.TogglePause();
                form.Focus();
            };

            var menu = CreateButton("MAIN MENU", 150, Color.FromArgb(255, 165, 0));
            menu.Click += (s, e) =>
            {
                form.Controls.Remove(pausePanel);
                ReturnToMainMenu();
            };

            var quit = CreateButton("QUIT GAME", 220, Color.FromArgb(200, 50, 50));
            quit.Click += (s, e) => Application.Exit();

            pausePanel.Controls.Add(title);
            pausePanel.Controls.Add(resume);
            pausePanel.Controls.Add(menu);
            pausePanel.Controls.Add(quit);

            form.Controls.Add(pausePanel);
            pausePanel.BringToFront();
        }
        public void HidePauseMenu()
        {
            if (pausePanel != null)
            {
                form.Controls.Remove(pausePanel);
                pausePanel.Dispose();
                pausePanel = null;
            }
        }

        public void ShowGameOverDialog()
        {
            var panel = new Panel
            {
                Size = new Size(400, 300),
                BackColor = Color.FromArgb(40, 45, 55),
                Location = new Point((form.Width - 400) / 2, (form.Height - 300) / 2)
            };

            var title = new Label
            {
                Text = "GAME OVER",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 100, 100),
                Size = new Size(380, 50),
                Location = new Point(10, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var score = new Label
            {
                Text = $"Score: {state.Score}\nBest Score: {SettingsManager.Settings.BestScore}\nLevel: {state.Level}",
                Font = new Font("Arial", 14),
                ForeColor = Color.White,
                Size = new Size(380, 80),
                Location = new Point(10, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var yes = CreateButton("YES", 220, Color.FromArgb(50, 200, 50));
            yes.Location = new Point(80, 220);
            yes.Click += (s, e) =>
            {
                form.Controls.Remove(panel);
                manager.RestartGame();
            };

            var no = CreateButton("NO", 220, Color.FromArgb(200, 50, 50));
            no.Location = new Point(220, 220);
            no.Click += (s, e) =>
            {
                form.Controls.Remove(panel);
                ReturnToMainMenu();
            };

            panel.Controls.Add(title);
            panel.Controls.Add(score);
            panel.Controls.Add(yes);
            panel.Controls.Add(no);

            form.Controls.Add(panel);
            panel.BringToFront();
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

        private void ReturnToMainMenu()
        {
            form.Hide();
            var menu = new MainMenuForm();
            menu.Show();
        }
    }
}