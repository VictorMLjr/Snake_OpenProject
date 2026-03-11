using System.Windows.Forms;
using CleanSnakeGame.Data;
using CleanSnakeGame.Services;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Input
{
    public class InputHandler
    {
        private readonly GameState state;
        private readonly GameManager manager;
        private readonly WindowService windowService;

        public InputHandler(GameState gameState, GameManager gameManager, WindowService windowSvc)
        {
            state = gameState;
            manager = gameManager;
            windowService = windowSvc;
        }

        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
            // Fullscreen toggle works anytime
            if (e.KeyCode == Keys.F11)
            {
                windowService.ToggleFullscreen();
                return;
            }

            if (!state.GameRunning || state.GamePaused)
            {
                if (e.KeyCode == Keys.Escape && state.GamePaused)
                {
                    manager.TogglePause();
                }
                return;
            }

            Direction newDirection = state.Direction;

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    if (state.Direction != Direction.Down)
                        newDirection = Direction.Up;
                    break;

                case Keys.S:
                case Keys.Down:
                    if (state.Direction != Direction.Up)
                        newDirection = Direction.Down;
                    break;

                case Keys.A:
                case Keys.Left:
                    if (state.Direction != Direction.Right)
                        newDirection = Direction.Left;
                    break;

                case Keys.D:
                case Keys.Right:
                    if (state.Direction != Direction.Left)
                        newDirection = Direction.Right;
                    break;
                case Keys.Escape:
                    manager.TogglePause();
                    return;
            }

            state.NextDirection = newDirection;
        }
    }
}