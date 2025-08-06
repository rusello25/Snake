using TestSnake.Application;

namespace TestSnake.Core.StateMachine
{
    public class GameOverState(GameLogic game) : IGameState
    {
        private readonly GameLogic _game = game;

        public void Update()
        {
            // No updates in game over state
        }

        public void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.R:
                    _game.InitializeGame(); // Restart game
                    break;
                case ConsoleKey.Escape:
                    // Exit handled by game engine
                    break;
            }
        }

        public void OnEnter()
        {
            _game.EndGame();
        }

        public void OnExit()
        {
            // Cleanup if needed
        }

        public bool CanTransitionTo<TState>() where TState : IGameState
        {
            return typeof(TState) == typeof(RunningState);
        }
    }
}