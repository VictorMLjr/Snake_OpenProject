using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using CleanSnakeGame.Data;
using CleanSnakeGame.Input;
using CleanSnakeGame.Services;

namespace CleanSnakeGame.UI
{
    public partial class GameForm : Form
    {
        private const int CellSize = 20;
        private const int GridWidth = 40;
        private const int GridHeight = 30;

        // Game systems
        private GameState gameState;
        private GameEngine gameEngine;
        private GameRenderer renderer;
        private GameManager gameManager;
        private WindowService windowService;
        private InputHandler inputHandler;

        private Timer gameTimer;

        // UI
        private Panel gamePanel;
        private Panel pausePanel;

        private Label lblScore;
        private Label lblBest;
        private Label lblLevel;
        private Label lblSpeed;
        private Label lblPlayer;
        private Label lblControls;

        private Button btnResume;
        private Button btnRestart;
        private Button btnMainMenu;

        public GameForm()
        {
            // Create core systems
            gameState = new GameState();
            gameEngine = new GameEngine(gameState);
            gameManager = new GameManager(gameState);
            renderer = new GameRenderer(gameState);
            windowService = new WindowService(this);
            inputHandler = new InputHandler(gameState, gameManager, windowService);

            gameTimer = new Timer();

            SetupForm();
            CreateControls();
            InitializeGame();
        }

        private void SetupForm()
        {
            Text = "Ultimate Snake Game";
            Size = new Size(1024, 768);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 35, 45);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            KeyPreview = true;
            KeyDown += inputHandler.HandleKeyDown;

            windowService.ApplyFullscreenSetting();

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer, true);

            UpdateStyles();
        }

        private void InitializeGame()
        {
            gameManager.InitializeGame();

            gameTimer.Interval = gameState.Speed;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            UpdateUI();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameEngine.UpdateGame();

            UpdateUI();
            gamePanel.Invalidate();
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            renderer.GamePanel_Paint(sender, e);
        }

        private void CreateControls()
        {
            int statusY = 15;

            lblScore = CreateStatusLabel("Score: 0", 20, statusY, Color.White);
            lblBest = CreateStatusLabel("Best: 0", 120, statusY, Color.Green);
            lblLevel = CreateStatusLabel("Level: 1", 400, statusY, Color.Blue);
            lblSpeed = CreateStatusLabel("Speed: 10", 500, statusY, Color.Gray);
            lblPlayer = CreateStatusLabel("Player: Player", 750, statusY, Color.White);
            lblControls = CreateStatusLabel("WASD/Arrows: Move | ESC: Menu", 750, statusY + 20, Color.Gray);

            gamePanel = new Panel
            {
                Size = new Size(GridWidth * CellSize, GridHeight * CellSize),
                Location = new Point((ClientSize.Width - GridWidth * CellSize) / 2, 70),
                BackColor = Color.FromArgb(20, 25, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                gamePanel,
                new object[] { true });

            gamePanel.Paint += GamePanel_Paint;

            Controls.Add(gamePanel);

            CreatePausePanel();
        }

        private Label CreateStatusLabel(string text, int x, int y, Color color)
        {
            Label label = new Label
            {
                Text = text,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(x, y)
            };

            Controls.Add(label);
            return label;
        }

        private void CreatePausePanel()
        {
            pausePanel = new Panel
            {
                Size = new Size(300, 200),
                BackColor = Color.FromArgb(200, Color.Black),
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            var lblPaused = new Label
            {
                Text = "GAME PAUSED",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(80, 20)
            };

            pausePanel.Controls.Add(lblPaused);

            btnResume = CreatePauseButton("RESUME", 50, Color.Green);
            btnResume.Click += BtnResume_Click;
            pausePanel.Controls.Add(btnResume);

            btnRestart = CreatePauseButton("RESTART", 90, Color.Orange);
            btnRestart.Click += BtnRestart_Click;
            pausePanel.Controls.Add(btnRestart);

            btnMainMenu = CreatePauseButton("MAIN MENU", 130, Color.Red);
            btnMainMenu.Click += BtnMainMenu_Click;
            pausePanel.Controls.Add(btnMainMenu);

            Controls.Add(pausePanel);

            CenterPausePanel();
        }

        private Button CreatePauseButton(string text, int y, Color color)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(200, 30),
                Location = new Point(50, y),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        private void CenterPausePanel()
        {
            pausePanel.Location = new Point(
                (ClientSize.Width - pausePanel.Width) / 2,
                (ClientSize.Height - pausePanel.Height) / 2
            );
        }

        private void UpdateUI()
        {
            lblScore.Text = $"Score: {gameState.Score}";
            lblBest.Text = $"Best: {SettingsManager.Settings.BestScore}";
            lblLevel.Text = $"Level: {gameState.Level}";
            lblSpeed.Text = $"Speed: {gameState.Speed}";
            lblPlayer.Text = $"Player: {SettingsManager.Settings.PlayerName}";
        }

        private void BtnResume_Click(object sender, EventArgs e)
        {
            gameManager.TogglePause();
            pausePanel.Visible = gameState.GamePaused;
        }

        private void BtnRestart_Click(object sender, EventArgs e)
        {
            gameManager.RestartGame();
        }

        private void BtnMainMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            gameTimer?.Stop();
            gameTimer?.Dispose();

            base.OnFormClosed(e);
        }
    }
}
public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }