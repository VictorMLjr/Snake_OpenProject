using System.Linq;
using System.Windows.Forms;
using CleanSnakeGame.Core;
using CleanSnakeGame.Services;
using CleanSnakeGame.UI;

namespace CleanSnakeGame.Input
{
    public class InputHandler
    {
        private readonly GameState state;
        private readonly GameManager manager;
        private readonly WindowService windowService;

        private const int MAX_BUFFER = 3;

        public InputHandler(GameState state, GameManager manager, WindowService windowService)
        {
            this.state = state;
            this.manager = manager;
            this.windowService = windowService;
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
                    manager.TogglePause();

                return;
            }

            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    EnqueueDirection(Direction.Up, Direction.Down);
                    break;

                case Keys.S:
                case Keys.Down:
                    EnqueueDirection(Direction.Down, Direction.Up);
                    break;

                case Keys.A:
                case Keys.Left:
                    EnqueueDirection(Direction.Left, Direction.Right);
                    break;

                case Keys.D:
                case Keys.Right:
                    EnqueueDirection(Direction.Right, Direction.Left);
                    break;

                case Keys.Escape:
                    manager.TogglePause();
                    break;
            }
        }

        private void EnqueueDirection(Direction desired, Direction opposite)
        {
            Direction last = state.DirectionQueue.Count > 0
                ? state.DirectionQueue.Last()
                : state.Direction;

            if (last != opposite && state.DirectionQueue.Count < MAX_BUFFER)
            {
                state.DirectionQueue.Enqueue(desired);
            }
        }
    }
}