using System;
using System.Collections.Generic;
using System.Drawing;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Data
{
    public class GameState
    {
        // Snake data
        public List<Point> Snake { get; set; }
        public Point Food { get; set; }

        public List<Point> Powerups { get; set; }
        public List<Point> Obstacles { get; set; }

        public Dictionary<Point, DateTime> PowerupTimers { get; set; }
        public Dictionary<Point, DateTime> ObstacleTimers { get; set; }

        // Game stats
        public int Score { get; set; }
        public int Level { get; set; }
        public int Speed { get; set; }

        // Game status
        public bool GameRunning { get; set; }
        public bool GamePaused { get; set; }

        // Movement
        public Direction Direction { get; set; }
        public Direction NextDirection { get; set; }

        public GameState()
        {
            Snake = new List<Point>();
            Powerups = new List<Point>();
            Obstacles = new List<Point>();

            PowerupTimers = new Dictionary<Point, DateTime>();
            ObstacleTimers = new Dictionary<Point, DateTime>();

            Score = 0;
            Level = 1;
            Speed = 100;

            Direction = Direction.Right;
            NextDirection = Direction.Right;

            GameRunning = false;
            GamePaused = false;
        }
    }
}