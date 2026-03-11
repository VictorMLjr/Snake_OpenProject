using System;
using System.Drawing;
using System.Linq;
using CleanSnakeGame.Data;
using CleanSnakeGame.Services;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Services
{
    public class GameEngine
    {
        private readonly GameState gameState;
        private readonly Random random;

        private const int GridWidth = 40;
        private const int GridHeight = 30;

        public GameEngine(GameState state)
        {
            gameState = state;
            random = new Random();
        }

        public void UpdateGame()
        {
            if (!gameState.GameRunning || gameState.GamePaused || gameState.Snake.Count == 0)
                return;

            gameState.Direction = gameState.NextDirection;

            MoveSnake();
            HandleFood();
            HandlePowerups();
            HandleObstacles();
            ManageDynamicItems();
        }

        private void MoveSnake()
        {
            Point head = gameState.Snake[0];

            Point newHead = gameState.Direction switch
            {
                Direction.Up => new Point(head.X, head.Y - 1),
                Direction.Down => new Point(head.X, head.Y + 1),
                Direction.Left => new Point(head.X - 1, head.Y),
                Direction.Right => new Point(head.X + 1, head.Y),
                _ => head
            };

            // Wall wrapping
            if (newHead.X < 0) newHead.X = GridWidth - 1;
            if (newHead.X >= GridWidth) newHead.X = 0;

            if (newHead.Y < 0) newHead.Y = GridHeight - 1;
            if (newHead.Y >= GridHeight) newHead.Y = 0;

            gameState.Snake.Insert(0, newHead);

            if (CheckSelfCollision())
            {
                gameState.GameRunning = false;
                return;
            }
        }
        private bool CheckSelfCollision()
        {
            if (gameState.Snake.Count <= 1)
                return false;

            Point head = gameState.Snake[0];

            return gameState.Snake
                .Skip(1)
                .Any(segment => segment.Equals(head));
        }
        private void HandleFood()
        {
            Point head = gameState.Snake[0];

            if (head.Equals(gameState.Food))
            {
                gameState.Score += 10;
                GenerateFood();

                if (gameState.Score / 100 + 1 > gameState.Level)
                {
                    gameState.Level++;
                    gameState.Speed = Math.Max(50, gameState.Speed - 10);
                }
            }
            else
            {
                gameState.Snake.RemoveAt(gameState.Snake.Count - 1);
            }
        }
        private void HandlePowerups()
        {
            if (!SettingsManager.Settings.PowerupsEnabled)
                return;

            Point head = gameState.Snake[0];

            if (gameState.Powerups.Contains(head))
            {
                gameState.Powerups.Remove(head);
                gameState.PowerupTimers.Remove(head);

                gameState.Score += 25;
            }
        }
        private void HandleObstacles()
        {
            if (!SettingsManager.Settings.ObstaclesEnabled)
                return;

            Point head = gameState.Snake[0];

            if (gameState.Obstacles.Contains(head))
            {
                gameState.GameRunning = false;
            }
        }
        private void GenerateFood()
        {
            Point newFood;

            do
            {
                newFood = new Point(
                    random.Next(GridWidth),
                    random.Next(GridHeight)
                );
            }
            while (
                gameState.Snake.Contains(newFood) ||
                gameState.Powerups.Contains(newFood) ||
                gameState.Obstacles.Contains(newFood)
            );

            gameState.Food = newFood;
        }
        private void ManageDynamicItems()
        {
            DateTime now = DateTime.Now;

            if (SettingsManager.Settings.PowerupsEnabled)
            {
                var expired = gameState.PowerupTimers
                    .Where(p => (now - p.Value).TotalSeconds > 8)
                    .Select(p => p.Key)
                    .ToList();

                foreach (var p in expired)
                {
                    gameState.Powerups.Remove(p);
                    gameState.PowerupTimers.Remove(p);
                }

                if (gameState.Powerups.Count == 0 && random.Next(1000) < 5)
                {
                    SpawnPowerup(now);
                }
            }

            if (SettingsManager.Settings.ObstaclesEnabled)
            {
                var expired = gameState.ObstacleTimers
                    .Where(o => (now - o.Value).TotalSeconds > 12)
                    .Select(o => o.Key)
                    .ToList();

                foreach (var o in expired)
                {
                    gameState.Obstacles.Remove(o);
                    gameState.ObstacleTimers.Remove(o);
                }

                if (gameState.Score >= 60 && gameState.Obstacles.Count < 2 && random.Next(100) < 2)
                {
                    SpawnObstacle(now);
                }
            }
        }
        private void SpawnPowerup(DateTime time)
        {
            Point p;

            do
            {
                p = new Point(
                    random.Next(GridWidth),
                    random.Next(GridHeight)
                );
            }
            while (
                gameState.Snake.Contains(p) ||
                gameState.Powerups.Contains(p) ||
                gameState.Obstacles.Contains(p) ||
                p.Equals(gameState.Food)
            );

            gameState.Powerups.Add(p);
            gameState.PowerupTimers[p] = time;
        }
        private void SpawnObstacle(DateTime time)
        {
            Point p;
            do
            {
                p = new Point(
                    random.Next(GridWidth),
                    random.Next(GridHeight)
                );
            }
            while (
                gameState.Snake.Contains(p) ||
                gameState.Powerups.Contains(p) ||
                gameState.Obstacles.Contains(p) ||
                p.Equals(gameState.Food)
            );

            gameState.Obstacles.Add(p);
            gameState.ObstacleTimers[p] = time;
        }
    }
}