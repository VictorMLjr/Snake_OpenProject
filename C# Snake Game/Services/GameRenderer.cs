using System;
using System.Drawing;
using System.Windows.Forms;
using CleanSnakeGame.Data;

namespace CleanSnakeGame.Services
{
    public class GameRenderer
    {
        private readonly GameState state;

        public int CellSize = 20;
        public int GridWidth = 30;
        public int GridHeight = 20;

        public GameRenderer(GameState gameState)
        {
            state = gameState;
        }

        public void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;

            g.Clear(Color.FromArgb(20, 25, 35));

            DrawGrid(g);
            DrawSnake(g);
            DrawFood(g);
            DrawPowerups(g);
            DrawObstacles(g);
        }

        private void DrawGrid(Graphics g)
        {
            if (!SettingsManager.Settings.ShowGrid) return;

            using var pen = new Pen(Color.FromArgb(40, Color.Gray), 1);

            for (int x = 0; x <= GridWidth; x++)
                g.DrawLine(pen, x * CellSize, 0, x * CellSize, GridHeight * CellSize);

            for (int y = 0; y <= GridHeight; y++)
                g.DrawLine(pen, 0, y * CellSize, GridWidth * CellSize, y * CellSize);
        }

        private void DrawSnake(Graphics g)
        {
            if (state.Snake == null || state.Snake.Count == 0) return;

            for (int i = 0; i < state.Snake.Count; i++)
            {
                Point segment = state.Snake[i];

                Rectangle rect = new Rectangle(
                    segment.X * CellSize + 1,
                    segment.Y * CellSize + 1,
                    CellSize - 2,
                    CellSize - 2
                );

                if (i == 0)
                    DrawSnakeHead(g, rect);
                else
                    DrawSnakeBody(g, rect);
            }
        }

        private void DrawSnakeHead(Graphics g, Rectangle rect)
        {
            var headColor = SettingsManager.Settings.GetSnakeColor();

            using (var brush = new SolidBrush(headColor))
                g.FillRectangle(brush, rect);

            using (var pen = new Pen(Color.FromArgb(
                Math.Max(0, headColor.R - 50),
                Math.Max(0, headColor.G - 50),
                Math.Max(0, headColor.B - 50)), 2))
                g.DrawRectangle(pen, rect);

            using (var eyeBrush = new SolidBrush(Color.Black))
            {
                g.FillRectangle(eyeBrush, rect.X + 4, rect.Y + 4, 3, 3);
                g.FillRectangle(eyeBrush, rect.Right - 7, rect.Y + 4, 3, 3);
            }
        }

        private void DrawSnakeBody(Graphics g, Rectangle rect)
        {
            var headColor = SettingsManager.Settings.GetSnakeColor();

            var bodyColor = Color.FromArgb(
                Math.Max(0, headColor.R - 80),
                Math.Max(0, headColor.G - 80),
                Math.Max(0, headColor.B - 80));

            using (var brush = new SolidBrush(bodyColor))
                g.FillRectangle(brush, rect);

            using (var pen = new Pen(Color.FromArgb(
                Math.Max(0, bodyColor.R - 40),
                Math.Max(0, bodyColor.G - 40),
                Math.Max(0, bodyColor.B - 40)), 1))
                g.DrawRectangle(pen, rect);
        }

        private void DrawFood(Graphics g)
        {
            Rectangle foodRect = new Rectangle(
                state.Food.X * CellSize + 2,
                state.Food.Y * CellSize + 2,
                CellSize - 4,
                CellSize - 4
            );

            using (var brush = new SolidBrush(Color.FromArgb(255, 60, 60)))
                g.FillEllipse(brush, foodRect);

            using (var pen = new Pen(Color.FromArgb(200, 40, 40), 2))
                g.DrawEllipse(pen, foodRect);
        }

        private void DrawPowerups(Graphics g)
        {
            if (!SettingsManager.Settings.PowerupsEnabled) return;

            foreach (var powerup in state.Powerups)
            {
                Rectangle rect = new Rectangle(
                    powerup.X * CellSize + 1,
                    powerup.Y * CellSize + 1,
                    CellSize - 2,
                    CellSize - 2
                );

                using (var brush = new SolidBrush(Color.Gold))
                    g.FillEllipse(brush, rect);

                using (var pen = new Pen(Color.Orange, 2))
                    g.DrawEllipse(pen, rect);
            }
        }

        private void DrawObstacles(Graphics g)
        {
            if (!SettingsManager.Settings.ObstaclesEnabled) return;

            foreach (var obstacle in state.Obstacles)
            {
                Rectangle rect = new Rectangle(
                    obstacle.X * CellSize + 1,
                    obstacle.Y * CellSize + 1,
                    CellSize - 2,
                    CellSize - 2
                );

                using (var brush = new SolidBrush(Color.FromArgb(139, 69, 19)))
                    g.FillRectangle(brush, rect);

                using (var pen = new Pen(Color.FromArgb(101, 67, 33), 2))
                    g.DrawRectangle(pen, rect);
            }
        }
    }
}