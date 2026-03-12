using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using CleanSnakeGame.Services;

namespace CleanSnakeGame.UI
{
    public partial class SettingsForm : Form
    {
        private Label lblTitle;
        private Button btnDifficulty;
        private Button btnSnakeColor;
        private Button btnPlayerName;
        private Button btnPowerups;
        private Button btnObstacles;
        private Button btnSound;
        private Button btnShowGrid;
        private Button btnFullscreen;
        private Button btnBack;
        private Button btnEnableCollision;
        private Panel pnlSnakeColorPreview;

        // Why did the original creator have buttons declared BUT NOT THE LABELS???
        private Label difficultyLabel;
        private Label diffDescLabel;
        private Label colorLabel;
        private Label nameLabel;
        private Label powerUpsLabel;
        private Label obstaclesLabel;
        private Label soundLabel;
        private Label gridLabel;
        private Label boundaryLabel;
        private Label fullscreenLabel;

        private readonly Color[] snakeColors = { Color.Lime, Color.Red, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan };

        public SettingsForm()
        {
            InitializeComponent();
            SetupForm();
            CreateControls();
            UpdateStatusLabels();
            ApplyFullscreenToCurrentForm();
        }

        private void SetupForm()
        {
            Text = "Settings - Ultimate Snake Game";
            Size = new Size(1024, 800); // Increased height to prevent overlap
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(30, 35, 45);
            MaximizeBox = false;
            KeyPreview = true;
            KeyDown += SettingsForm_KeyDown;
        }

        private void CreateControls()
        {
            // Title
            lblTitle = new Label
            {
                Text = "🐍 SETTINGS",
                Font = new Font("Arial", 48, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(350, 50)
            };
            Controls.Add(lblTitle);

            int yPos = 150;
            int spacing = 65;

            // Difficulty
            CreateSettingButton(ref btnDifficulty, "Difficulty", yPos, Color.FromArgb(33, 150, 243));
            btnDifficulty.Click += BtnDifficulty_Click;
            difficultyLabel = CreateStatusLabel(yPos);
            diffDescLabel = CreateSubtextLabel(yPos + 20);
            diffDescLabel.UseMnemonic = false;
            yPos += spacing;

            // Snake Color with preview
            CreateSettingButton(ref btnSnakeColor, "Snake Color", yPos, Color.FromArgb(76, 175, 80));
            btnSnakeColor.Click += BtnSnakeColor_Click;
            colorLabel = CreateStatusLabel(yPos);

            pnlSnakeColorPreview = new Panel
            {
                Size = new Size(60, 20),
                Location = new Point(550, yPos + 15),
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlSnakeColorPreview.Paint += PnlSnakeColorPreview_Paint;
            Controls.Add(pnlSnakeColorPreview);
            yPos += spacing;

            // Player Name
            CreateSettingButton(ref btnPlayerName, "Player Name", yPos, Color.FromArgb(156, 39, 176));
            btnPlayerName.Click += BtnPlayerName_Click;
            nameLabel = CreateStatusLabel(yPos);
            yPos += spacing;

            // Powerups
            CreateSettingButton(ref btnPowerups, "Powerups", yPos, Color.FromArgb(255, 152, 0));
            btnPowerups.Click += BtnPowerups_Click;
            powerUpsLabel = CreateStatusLabel(yPos);
            yPos += spacing;

            // Obstacles
            CreateSettingButton(ref btnObstacles, "Obstacles", yPos, Color.FromArgb(244, 67, 54));
            btnObstacles.Click += BtnObstacles_Click;
            obstaclesLabel = CreateStatusLabel(yPos);
            yPos += spacing;

            // Sound
            CreateSettingButton(ref btnSound, "Sound", yPos, Color.FromArgb(255, 235, 59));
            btnSound.Click += BtnSound_Click;
            soundLabel = CreateStatusLabel(yPos);
            yPos += spacing;

            // Show Grid
            CreateSettingButton(ref btnShowGrid, "Show Grid", yPos, Color.FromArgb(33, 150, 243));
            btnShowGrid.Click += BtnShowGrid_Click;
            gridLabel = CreateStatusLabel(yPos);
            yPos += spacing;

            // Enable Collision (Boundary Walls)
            CreateSettingButton(ref btnEnableCollision, "Boundary Walls", yPos, Color.FromArgb(156, 39, 176));
            btnEnableCollision.Click += BtnEnableCollision_Click;
            boundaryLabel = CreateStatusLabel(yPos);
            yPos += spacing;

            // Fullscreen
            CreateSettingButton(ref btnFullscreen, "Fullscreen", yPos, Color.FromArgb(76, 175, 80));
            btnFullscreen.Click += BtnFullscreen_Click;
            fullscreenLabel = CreateStatusLabel(yPos);
            yPos += spacing; // Add spacing after fullscreen button

            // Back Button - positioned on right side for balanced layout
            btnBack = new Button
            {
                Text = "BACK",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Size = new Size(100, 40),
                Location = new Point(450, 730), // Centered at bottom
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += BtnBack_Click;
            Controls.Add(btnBack);

            // Instructions - positioned at bottom
            var lblInstructions = new Label
            {
                Text = "Click buttons to change settings • Use F11 for fullscreen • All settings are saved automatically",
                Font = new Font("Arial", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(150, 150, 150),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(900, 40),
                Location = new Point(50, 720) // Bottom center
            };
            Controls.Add(lblInstructions);
        }

        private void CreateSettingButton(ref Button button, string text, int yPos, Color buttonColor)
        {
            button = new Button
            {
                Text = text,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new Point(50, yPos),
                BackColor = buttonColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            button.FlatAppearance.BorderSize = 0;
            Controls.Add(button);
        }

        private void UpdateStatusLabels()
        {
            SetStatus(difficultyLabel, SettingsManager.Settings.Difficulty, true);
            diffDescLabel.Text = GetDifficultyDescription();
            SetStatus(colorLabel, GetColorName(snakeColors[SettingsManager.Settings.SnakeColorIndex]), true);
            SetStatus(nameLabel, $"Player Name: {SettingsManager.Settings.PlayerName}", true);
            SetStatus(powerUpsLabel, SettingsManager.Settings.PowerupsEnabled ? "ON" : "OFF", SettingsManager.Settings.PowerupsEnabled);
            SetStatus(obstaclesLabel, SettingsManager.Settings.ObstaclesEnabled ? "ON" : "OFF", SettingsManager.Settings.ObstaclesEnabled);
            SetStatus(soundLabel, SettingsManager.Settings.SoundEnabled ? "ON" : "OFF", SettingsManager.Settings.SoundEnabled);
            SetStatus(gridLabel, SettingsManager.Settings.ShowGrid ? "ON" : "OFF", SettingsManager.Settings.ShowGrid);
            SetStatus(boundaryLabel, SettingsManager.Settings.boundaryWalls ? "ON" : "OFF", SettingsManager.Settings.boundaryWalls);
            SetStatus(fullscreenLabel, SettingsManager.Settings.Fullscreen ? "ON" : "OFF", SettingsManager.Settings.Fullscreen);

            pnlSnakeColorPreview?.Invalidate();
        }

        private void SetStatus(Label lbl, string text, bool isPositive)
        {
            lbl.Text = text;
            lbl.ForeColor = isPositive
                ? Color.FromArgb(76, 175, 80)
                : Color.FromArgb(244, 67, 54);
        }

        private string GetDifficultyDescription()
        {
            return SettingsManager.Settings.Difficulty switch
            {
                "Easy" => "Relaxed pace",
                "Medium" => "Balanced challenge",
                "Hard" => "Fast & furious",
                _ => "Balanced challenge"
            };
        }

        private Label CreateStatusLabel(int yPos)
        {
            var label = new Label
            {
                Font = new Font("Arial", 12, FontStyle.Regular),
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(280, yPos + 10)
            };

            Controls.Add(label);
            return label;
        }

        private Label CreateSubtextLabel(int yPos)
        {
            var label = new Label
            {
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(280, yPos + 10)
            };

            Controls.Add(label);
            return label;
        }

        private string GetColorName(Color color)
        {
            if (color == Color.Lime) return "Emerald Green";
            if (color == Color.Red) return "Red";
            if (color == Color.Blue) return "Blue";
            if (color == Color.Yellow) return "Yellow";
            if (color == Color.Magenta) return "Magenta";
            if (color == Color.Cyan) return "Cyan";
            return "Custom";
        }

        private void PnlSnakeColorPreview_Paint(object sender, PaintEventArgs e)
        {
            int segmentWidth = pnlSnakeColorPreview.Width / 3;

            for (int i = 0; i < 3; i++)
            {
                Rectangle rect = new Rectangle(i * segmentWidth, 0, segmentWidth - 1, pnlSnakeColorPreview.Height - 1);
                using (var brush = new SolidBrush(snakeColors[SettingsManager.Settings.SnakeColorIndex]))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
                using (var pen = new Pen(Color.White))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        private void BtnDifficulty_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.Difficulty = SettingsManager.Settings.Difficulty switch
            {
                "Easy" => "Medium",
                "Medium" => "Hard",
                "Hard" => "Easy",
                _ => "Medium"
            };
            SettingsManager.SaveSettings();
            UpdateStatusLabels();
        }

        private void BtnSnakeColor_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.SnakeColorIndex = (SettingsManager.Settings.SnakeColorIndex + 1) % snakeColors.Length;
            SettingsManager.SaveSettings();
            pnlSnakeColorPreview.Invalidate();
            UpdateStatusLabels();
        }

        private void BtnPlayerName_Click(object sender, EventArgs e)
        {
            string newName = ShowInputDialog("Enter your player name:", "Player Name", SettingsManager.Settings.PlayerName);

            if (!string.IsNullOrWhiteSpace(newName) && newName != SettingsManager.Settings.PlayerName)
            {
                SettingsManager.Settings.PlayerName = newName;
                SettingsManager.SaveSettings();
                UpdateStatusLabels();
            }
        }

        private string ShowInputDialog(string text, string caption, string defaultValue)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(30, 35, 45)
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Width = 350, Text = text, ForeColor = Color.White };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 350, Text = defaultValue, BackColor = Color.DarkGray, ForeColor = Color.White };
            Button confirmation = new Button() { Text = "OK", Left = 250, Width = 100, Top = 90, DialogResult = DialogResult.OK, BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            Button cancel = new Button() { Text = "Cancel", Left = 130, Width = 100, Top = 90, DialogResult = DialogResult.Cancel, BackColor = Color.FromArgb(244, 67, 54), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            confirmation.FlatAppearance.BorderSize = 0;
            cancel.FlatAppearance.BorderSize = 0;

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : defaultValue;
        }

        private void BtnPowerups_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.PowerupsEnabled = !SettingsManager.Settings.PowerupsEnabled;
            SettingsManager.SaveSettings();
            UpdateStatusLabels();
        }

        private void BtnObstacles_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.ObstaclesEnabled = !SettingsManager.Settings.ObstaclesEnabled;
            SettingsManager.SaveSettings();
            UpdateStatusLabels();
        }

        private void BtnSound_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.SoundEnabled = !SettingsManager.Settings.SoundEnabled;
            SettingsManager.SaveSettings();
            UpdateStatusLabels();
        }

        private void BtnShowGrid_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.ShowGrid = !SettingsManager.Settings.ShowGrid;
            SettingsManager.SaveSettings();
            UpdateStatusLabels();
        }

        private void BtnEnableCollision_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.boundaryWalls = !SettingsManager.Settings.boundaryWalls;
            SettingsManager.SaveSettings();
            UpdateStatusLabels();
        }
        private void BtnFullscreen_Click(object sender, EventArgs e)
        {
            SettingsManager.Settings.Fullscreen = !SettingsManager.Settings.Fullscreen;
            SettingsManager.SaveSettings();
            UpdateStatusLabels();

            // Apply fullscreen to this form immediately
            ApplyFullscreenToCurrentForm();
        }

        private void ApplyFullscreenToCurrentForm()
        {
            if (SettingsManager.Settings.Fullscreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                TopMost = true;
            }
            else
            {
                TopMost = false;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.FixedSingle;
                Size = new Size(1024, 800);
                CenterToScreen();
            }

            Refresh();
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                BtnBack_Click(sender, e);
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 768);
            Name = "SettingsForm";
            Text = "Settings - Ultimate Snake Game";
            ResumeLayout(false);
        }
    }
}
