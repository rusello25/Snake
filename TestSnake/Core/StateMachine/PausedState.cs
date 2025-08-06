using TestSnake.Application;

namespace TestSnake.Core.StateMachine
{
    public class PausedState(GameLogic game) : IGameState
    {
        private readonly GameLogic _game = game;

        public void Update()
        {
            // No game updates while paused
        }

        public void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.P:
                case ConsoleKey.Spacebar:
                    _game.SetState(new RunningState(_game)); // Resume
                    break;
                case ConsoleKey.Escape:
                    _game.SetState(new GameOverState(_game)); // Quit to menu
                    break;
            }
        }

        public void OnEnter()
        {
            // Game is now paused
        }

        public void OnExit()
        {
            // Game is no longer paused
        }

        public bool CanTransitionTo<TState>() where TState : IGameState
        {
            return typeof(TState) == typeof(RunningState) || typeof(TState) == typeof(GameOverState);
        }
    }
}