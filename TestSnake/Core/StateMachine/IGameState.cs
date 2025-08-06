namespace TestSnake.Core.StateMachine
{
    public interface IGameState
    {
        void Update();
        void HandleInput(ConsoleKey key);
        void OnEnter();
        void OnExit();
        bool CanTransitionTo<TState>() where TState : IGameState;
    }
}