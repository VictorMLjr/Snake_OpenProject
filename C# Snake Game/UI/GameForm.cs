using System;
using System.Drawing;
using System.Windows.Forms;
using CleanSnakeGame.Core;
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

        // Core systems
        private GameState gameState;
        private GameEngine gameEngine;
        private GameRenderer renderer;
        private GameManager gameManager;

        // Services
        private WindowService windowService;
        private InputHandler inputHandler;
        private GameDialogs gameDialogs;

        // Timer
        private Timer gameTimer;

        // UI
        private Panel gamePanel;
        private Label lblScore;
        private Label lblBest;
        private Label lblLevel;
        private Label lblSpeed;
        private Label lblPlayer;

        public GameForm()
        {
            InitializeComponent();

            SetupForm();
            CreateControls();
            CreateSystems();
            StartGame();
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

            // Enable double buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer, true);

            UpdateStyles();
        }

        private void CreateSystems()
        {
            gameTimer = new Timer();
            gameTimer.Interval = 100;
            gameTimer.Tick += GameLoop;

            gameState = new GameState();

            gameEngine = new GameEngine(gameState);
            renderer = new GameRenderer(gameState, GridWidth, GridHeight, CellSize);

            windowService = new WindowService(this);

            gameManager = new GameManager(
                gameState,
                gameEngine,
                gameTimer,
                gamePanel,
                UpdateUI,
                () => gameDialogs.ShowGameOverDialog(),
                () => gameDialogs.ShowPauseMenu()
            );

            gameDialogs = new GameDialogs(this, gameState, gameManager);

            inputHandler = new InputHandler(gameState, gameManager, windowService);
            KeyDown += inputHandler.HandleKeyDown;
        }

        private void CreateControls()
        {
            int statusY = 15;

            lblScore = CreateLabel("Score: 0", 20, statusY);
            lblBest = CreateLabel("Best: 0", 120, statusY);
            lblLevel = CreateLabel("Level: 1", 400, statusY);
            lblSpeed = CreateLabel("Speed: 10", 500, statusY);
            lblPlayer = CreateLabel("Player: Player", 750, statusY);

            gamePanel = new Panel
            {
                Size = new Size(GridWidth * CellSize, GridHeight * CellSize),
                Location = new Point(
                    (ClientSize.Width - GridWidth * CellSize) / 2,
                    70
                ),
                BackColor = Color.FromArgb(20, 25, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                gamePanel,
                new object[] { true });

            gamePanel.Paint += GamePanel_Paint;

            Controls.Add(gamePanel);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(x, y)
            };

            Controls.Add(label);
            return label;
        }

        private void StartGame()
        {
            gameManager.InitializeGame();
            gameTimer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            gameEngine.Update();

            if (gameState.GameOver)
            {
                gameTimer.Stop();
                gameDialogs.ShowGameOverDialog();
                return;
            }

            UpdateUI();
            gamePanel.Invalidate();
        }

        private void UpdateUI()
        {
            lblScore.Text = $"Score: {gameState.Score}";
            lblBest.Text = $"Best: {SettingsManager.Settings.BestScore}";
            lblLevel.Text = $"Level: {gameState.Level}";
            lblSpeed.Text = $"Speed: {gameState.Speed}";
            lblPlayer.Text = $"Player: {SettingsManager.Settings.PlayerName}";
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            renderer.Render(e.Graphics, e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            gameTimer?.Stop();
            gameTimer?.Dispose();
            base.OnFormClosed(e);
        }
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // GameForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 1200);
            Margin = new Padding(4, 5, 4, 5);
            Name = "GameForm";
            Text = "Ultimate Snake Game - Playing";
            Load += GameForm_Load;
            ResumeLayout(false);
        }

        private void GameForm_Load(object sender, EventArgs e)
        {

        }
    }
    public enum Direction { Up, Down, Left, Right }
}