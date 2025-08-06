using TestSnake.Application;

namespace TestSnake.Core.StateMachine
{
    public class RunningState(GameLogic game) : IGameState
    {
        private readonly GameLogic _game = game;

        public void Update()
        {
            _game.UpdateGameCore();
        }

        public void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:    _game.ChangeDirection(0, -1); break;
                case ConsoleKey.DownArrow:  _game.ChangeDirection(0, 1); break;
                case ConsoleKey.LeftArrow:  _game.ChangeDirection(-1, 0); break;
                case ConsoleKey.RightArrow: _game.ChangeDirection(1, 0); break;
                case ConsoleKey.Escape:     _game.SetState(new GameOverState(_game)); break;
                case ConsoleKey.P:          _game.SetState(new PausedState(_game)); break;
            }
        }

        public void OnEnter()
        {
            // Game is now running
        }

        public void OnExit()
        {
            // Game is no longer running
        }

        public bool CanTransitionTo<TState>() where TState : IGameState
        {
            return typeof(TState) == typeof(GameOverState) || typeof(TState) == typeof(PausedState);
        }
    }
}