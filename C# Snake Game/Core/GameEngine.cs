using System;
using System.Drawing;
using System.Linq;
using System.Media;
using CleanSnakeGame.Data;
using CleanSnakeGame.Services;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Core
{
    public class GameEngine
    {
        private readonly GameState state;
        private readonly Random random = new Random();

        private const int GridWidth = 40;
        private const int GridHeight = 30;

        SoundPlayer nom = new SoundPlayer(Properties.Resources.munch);
        SoundPlayer ouch = new SoundPlayer(Properties.Resources.ouch);

        public GameEngine(GameState gameState)
        {
            state = gameState;
        }

        public void Update()
        {
            if (!state.GameRunning || state.GamePaused) return;

            if (state.Snake.Count == 0) return;

            // Apply buffered direction
            if (state.DirectionQueue.Count > 0)
                state.Direction = state.DirectionQueue.Dequeue();

            Point head = state.Snake[0];

            Point newHead = state.Direction switch
            {
                Direction.Up => new Point(head.X, head.Y - 1),
                Direction.Down => new Point(head.X, head.Y + 1),
                Direction.Left => new Point(head.X - 1, head.Y),
                Direction.Right => new Point(head.X + 1, head.Y),
                _ => head
            };

            HandleWallCollision(ref newHead);

            state.Snake.Insert(0, newHead);

            if (CheckSelfCollision())
            {
                if (SettingsManager.Settings.SoundEnabled) ouch.Play();

                state.GameOver = true;
                return;
            }

            HandleFoodAndItems(newHead);
            ManageDynamicItems();
        }

        private void HandleWallCollision(ref Point newHead)
        {
            if (SettingsManager.Settings.boundaryWalls)
            {
                if (newHead.X < 0 || newHead.X >= GridWidth ||
                    newHead.Y < 0 || newHead.Y >= GridHeight)
                {
                    if (SettingsManager.Settings.SoundEnabled) ouch.Play();

                    state.GameOver = true;
                }
            }
            else
            {
                if (newHead.X < 0) newHead.X = GridWidth - 1;
                if (newHead.X >= GridWidth) newHead.X = 0;
                if (newHead.Y < 0) newHead.Y = GridHeight - 1;
                if (newHead.Y >= GridHeight) newHead.Y = 0;
            }
        }

        private void HandleFoodAndItems(Point newHead)
        {
            if (newHead.Equals(state.Food))
            {
                if (SettingsManager.Settings.SoundEnabled) nom.Play();

                state.Score += 10;
                GenerateFood();

                if (state.Score / 100 + 1 > state.Level)
                {
                    state.Level++;
                    state.Speed = Math.Max(50, state.Speed - 10);
                }
            }
            else if (SettingsManager.Settings.PowerupsEnabled && state.Powerups.Contains(newHead))
            {
                if (SettingsManager.Settings.SoundEnabled) nom.Play();

                state.Powerups.Remove(newHead);
                state.PowerupTimers.Remove(newHead);
                state.Score += 25;
            }
            else if (SettingsManager.Settings.ObstaclesEnabled && state.Obstacles.Contains(newHead))
            {
                if (SettingsManager.Settings.SoundEnabled) ouch.Play();

                state.GameOver = true;
            }
            else
            {
                state.Snake.RemoveAt(state.Snake.Count - 1);
            }
        }

        public bool CheckSelfCollision()
        {
            Point head = state.Snake[0];
            return state.Snake.Skip(1).Any(s => s.Equals(head));
        }

        public void GenerateFood()
        {
            Point food;

            do
            {
                food = new Point(random.Next(GridWidth), random.Next(GridHeight));
            }
            while (state.Snake.Contains(food) ||
                   state.Powerups.Contains(food) ||
                   state.Obstacles.Contains(food));

            state.Food = food;
        }

        private void ManageDynamicItems()
        {
            DateTime now = DateTime.Now;

            if (SettingsManager.Settings.PowerupsEnabled)
            {
                var expired = state.PowerupTimers
                    .Where(x => (now - x.Value).TotalSeconds > 8)
                    .Select(x => x.Key)
                    .ToList();

                foreach (var p in expired)
                {
                    state.Powerups.Remove(p);
                    state.PowerupTimers.Remove(p);
                }

                if (state.Powerups.Count == 0 && random.Next(1000) < 5)
                {
                    SpawnPowerup(now);
                }
            }

            if (SettingsManager.Settings.ObstaclesEnabled)
            {
                var expired = state.ObstacleTimers
                    .Where(x => (now - x.Value).TotalSeconds > 12)
                    .Select(x => x.Key)
                    .ToList();

                foreach (var o in expired)
                {
                    state.Obstacles.Remove(o);
                    state.ObstacleTimers.Remove(o);
                }

                if (state.Score >= 60 && state.Obstacles.Count < 2 && random.Next(100) < 2)
                {
                    SpawnObstacle(now);
                }
            }
        }

        private void SpawnPowerup(DateTime time)
        {
            Point p;
            int attempts = 0;

            do
            {
                p = new Point(random.Next(GridWidth), random.Next(GridHeight));
                attempts++;
            }
            while ((state.Snake.Contains(p) ||
                   state.Powerups.Contains(p) ||
                   state.Obstacles.Contains(p) ||
                   p.Equals(state.Food)) && attempts < 50);

            if (attempts < 50)
            {
                state.Powerups.Add(p);
                state.PowerupTimers[p] = time;
            }
        }

        private void SpawnObstacle(DateTime time)
        {
            Point o;
            int attempts = 0;

            do
            {
                o = new Point(random.Next(GridWidth), random.Next(GridHeight));
                attempts++;
            }
            while ((state.Snake.Contains(o) ||
                   state.Powerups.Contains(o) ||
                   state.Obstacles.Contains(o) ||
                   o.Equals(state.Food)) && attempts < 50);

            if (attempts < 50)
            {
                state.Obstacles.Add(o);
                state.ObstacleTimers[o] = time;
            }
        }
    }
}