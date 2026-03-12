using System;
using System.Collections.Generic;
using System.Drawing;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Core
{
    public class GameState
    {
        // Snake data
        public List<Point> Snake { get; set; }
        public Point Food { get; set; }

        // Dynamic items
        public List<Point> Powerups { get; set; }
        public List<Point> Obstacles { get; set; }

        public Dictionary<Point, DateTime> PowerupTimers { get; set; }
        public Dictionary<Point, DateTime> ObstacleTimers { get; set; }

        // Movement
        public Direction Direction { get; set; }
        public Queue<Direction> DirectionQueue { get; set; }

        // Game stats
        public int Score { get; set; }
        public int Level { get; set; }
        public int Speed { get; set; }

        // Game status
        public bool GameRunning { get; set; }
        public bool GamePaused { get; set; }
        public bool GameOver { get; set; }

        public GameState()
        {
            Snake = new List<Point>();
            Powerups = new List<Point>();
            Obstacles = new List<Point>();

            PowerupTimers = new Dictionary<Point, DateTime>();
            ObstacleTimers = new Dictionary<Point, DateTime>();

            DirectionQueue = new Queue<Direction>();

            Reset();
        }

        public void Reset()
        {
            Snake.Clear();
            Powerups.Clear();
            Obstacles.Clear();
            PowerupTimers.Clear();
            ObstacleTimers.Clear();
            DirectionQueue.Clear();

            Score = 0;
            Level = 1;
            Speed = 100;

            Direction = Direction.Right;

            GameRunning = true;
            GamePaused = false;
            GameOver = false;

            // Start snake in center
            Snake.Add(new Point(20, 15));
            Snake.Add(new Point(19, 15));
            Snake.Add(new Point(18, 15));
        }
    }
}