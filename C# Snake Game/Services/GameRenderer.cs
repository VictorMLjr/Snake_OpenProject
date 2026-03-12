using System;
using System.Drawing;
using System.Windows.Forms;
using CleanSnakeGame.Services;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Core
{
    public class GameRenderer
    {
        private readonly GameState state;
        private readonly int gridWidth;
        private readonly int gridHeight;
        private readonly int cellSize;

        public GameRenderer(GameState state, int gridWidth, int gridHeight, int cellSize)
        {
            this.state = state;
            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
            this.cellSize = cellSize;
        }

        public void Render(object sender, PaintEventArgs e)
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

            for (int x = 0; x <= gridWidth; x++)
                g.DrawLine(pen, x * cellSize, 0, x * cellSize, gridHeight * cellSize);

            for (int y = 0; y <= gridHeight; y++)
                g.DrawLine(pen, 0, y * cellSize, gridWidth * cellSize, y * cellSize);
        }

        private void DrawSnake(Graphics g)
        {
            if (state.Snake == null || state.Snake.Count == 0) return;

            var headColor = SettingsManager.Settings.GetSnakeColor();

            for (int i = 0; i < state.Snake.Count; i++)
            {
                var segment = state.Snake[i];

                Rectangle rect = new Rectangle(
                    segment.X * cellSize + 1,
                    segment.Y * cellSize + 1,
                    cellSize - 2,
                    cellSize - 2
                );

                if (i == 0)
                {
                    using var brush = new SolidBrush(headColor);
                    g.FillRectangle(brush, rect);

                    using var pen = new Pen(Color.FromArgb(
                        Math.Max(0, headColor.R - 50),
                        Math.Max(0, headColor.G - 50),
                        Math.Max(0, headColor.B - 50)), 2);

                    g.DrawRectangle(pen, rect);
                }
                else
                {
                    var bodyColor = Color.FromArgb(
                        Math.Max(0, headColor.R - 80),
                        Math.Max(0, headColor.G - 80),
                        Math.Max(0, headColor.B - 80)
                    );

                    using var brush = new SolidBrush(bodyColor);
                    g.FillRectangle(brush, rect);

                    using var pen = new Pen(Color.FromArgb(
                        Math.Max(0, bodyColor.R - 40),
                        Math.Max(0, bodyColor.G - 40),
                        Math.Max(0, bodyColor.B - 40)), 1);

                    g.DrawRectangle(pen, rect);
                }
            }
        }

        private void DrawFood(Graphics g)
        {
            Rectangle foodRect = new Rectangle(
                state.Food.X * cellSize + 2,
                state.Food.Y * cellSize + 2,
                cellSize - 4,
                cellSize - 4
            );

            using var brush = new SolidBrush(Color.FromArgb(255, 60, 60));
            g.FillEllipse(brush, foodRect);

            using var pen = new Pen(Color.FromArgb(200, 40, 40), 2);
            g.DrawEllipse(pen, foodRect);
        }

        private void DrawPowerups(Graphics g)
        {
            if (!SettingsManager.Settings.PowerupsEnabled) return;

            foreach (var powerup in state.Powerups)
            {
                Rectangle rect = new Rectangle(
                    powerup.X * cellSize + 1,
                    powerup.Y * cellSize + 1,
                    cellSize - 2,
                    cellSize - 2
                );

                using var brush = new SolidBrush(Color.Gold);
                g.FillEllipse(brush, rect);

                using var pen = new Pen(Color.Orange, 2);
                g.DrawEllipse(pen, rect);
            }
        }

        private void DrawObstacles(Graphics g)
        {
            if (!SettingsManager.Settings.ObstaclesEnabled) return;

            foreach (var obstacle in state.Obstacles)
            {
                Rectangle rect = new Rectangle(
                    obstacle.X * cellSize + 1,
                    obstacle.Y * cellSize + 1,
                    cellSize - 2,
                    cellSize - 2
                );

                using var brush = new SolidBrush(Color.SaddleBrown);
                g.FillRectangle(brush, rect);

                using var pen = new Pen(Color.FromArgb(101, 67, 33), 2);
                g.DrawRectangle(pen, rect);
            }
        }
    }
}