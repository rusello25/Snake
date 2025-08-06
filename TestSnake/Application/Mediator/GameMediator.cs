using TestSnake.Core.Validation;

namespace TestSnake.Application.Mediator
{
    /// <summary>
    /// Base interface for all requests processed by the mediator.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Gets the unique identifier for this request.
        /// </summary>
        Guid RequestId { get; }

        /// <summary>
        /// Gets the timestamp when this request was created.
        /// </summary>
        DateTime CreatedAt { get; }
    }

    /// <summary>
    /// Interface for requests that return a response.
    /// </summary>
    /// <typeparam name="TResponse">Type of response</typeparam>
    public interface IRequest<out TResponse> : IRequest
    {
    }

    /// <summary>
    /// Interface for handling requests.
    /// </summary>
    /// <typeparam name="TRequest">Type of request to handle</typeparam>
    /// <typeparam name="TResponse">Type of response to return</typeparam>
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">Request to handle</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response</returns>
        Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface for handling requests without responses (commands).
    /// </summary>
    /// <typeparam name="TRequest">Type of request to handle</typeparam>
    public interface IRequestHandler<in TRequest>
        where TRequest : IRequest
    {
        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">Request to handle</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task HandleAsync(TRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Mediator interface for decoupling request/response communication.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Sends a request and returns a response.
        /// </summary>
        /// <typeparam name="TResponse">Type of response</typeparam>
        /// <param name="request">Request to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response</returns>
        Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a command without expecting a response.
        /// </summary>
        /// <param name="request">Request to send</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task SendAsync(IRequest request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Base implementation for requests.
    /// </summary>
    public abstract record RequestBase : IRequest
    {
        /// <inheritdoc />
        public Guid RequestId { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Simple mediator implementation using service provider for handler resolution.
    /// </summary>
    public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
    {
        private readonly IServiceProvider _serviceProvider = Guard.NotNull(serviceProvider);

        /// <inheritdoc />
        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            Guard.NotNull(request);

            var requestType = request.GetType();
            var responseType = typeof(TResponse);
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var handler = _serviceProvider.GetService(handlerType) ??
                throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");

            var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync)) ??
                throw new InvalidOperationException($"HandleAsync method not found on handler for {requestType.Name}");

            var result = method.Invoke(handler, [request, cancellationToken]);
            if (result is Task<TResponse> task)
                return await task;

            throw new InvalidOperationException($"Handler for {requestType.Name} did not return expected Task<{responseType.Name}>");
        }

        /// <inheritdoc />
        public async Task SendAsync(IRequest request, CancellationToken cancellationToken = default)
        {
            Guard.NotNull(request);

            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);

            var handler = _serviceProvider.GetService(handlerType) ??
                throw new InvalidOperationException($"No handler registered for request type {requestType.Name}");

            var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest>.HandleAsync)) ??
                throw new InvalidOperationException($"HandleAsync method not found on handler for {requestType.Name}");

            var result = method.Invoke(handler, [request, cancellationToken]);
            if (result is Task task)
                await task;
            else
                throw new InvalidOperationException($"Handler for {requestType.Name} did not return expected Task");
        }
    }

    /// <summary>
    /// Game-specific requests and responses.
    /// </summary>
    namespace Requests
    {
        /// <summary>
        /// Request to start a new game.
        /// </summary>
        public sealed record StartGameRequest(int Width, int Height) : RequestBase, IRequest<StartGameResponse>;

        /// <summary>
        /// Response for starting a new game.
        /// </summary>
        public sealed record StartGameResponse(bool Success, string? ErrorMessage, Guid GameId);

        /// <summary>
        /// Request to move the snake.
        /// </summary>
        public sealed record MoveSnakeRequest(Guid GameId, int DeltaX, int DeltaY) : RequestBase, IRequest<MoveSnakeResponse>;

        /// <summary>
        /// Response for moving the snake.
        /// </summary>
        public sealed record MoveSnakeResponse(bool Success, bool GameOver, int NewScore);

        /// <summary>
        /// Request to pause the game.
        /// </summary>
        public sealed record PauseGameRequest(Guid GameId) : RequestBase, IRequest;

        /// <summary>
        /// Request to get the current game state.
        /// </summary>
        public sealed record GetGameStateRequest(Guid GameId) : RequestBase, IRequest<GameStateResponse>;

        /// <summary>
        /// Response containing the current game state.
        /// </summary>
        public sealed record GameStateResponse(
            bool IsActive,
            int Score,
            int Level,
            IReadOnlyList<(int X, int Y)> SnakePositions,
            (int X, int Y) FoodPosition,
            IReadOnlyList<(int X, int Y)> Obstacles);

        /// <summary>
        /// Request to save the current game.
        /// </summary>
        public sealed record SaveGameRequest(Guid GameId, string SaveSlotName) : RequestBase, IRequest<SaveGameResponse>;

        /// <summary>
        /// Response for saving a game.
        /// </summary>
        public sealed record SaveGameResponse(bool Success, string? SaveSlotId, string? ErrorMessage);

        /// <summary>
        /// Request to load a saved game.
        /// </summary>
        public sealed record LoadGameRequest(string SaveSlotId) : RequestBase, IRequest<LoadGameResponse>;

        /// <summary>
        /// Response for loading a game.
        /// </summary>
        public sealed record LoadGameResponse(bool Success, Guid? GameId, string? ErrorMessage);
    }

    /// <summary>
    /// Example handlers for game requests.
    /// </summary>
    namespace Handlers
    {
        using TestSnake.Application.Interfaces;
        using TestSnake.Application.Mediator.Requests;

        /// <summary>
        /// Handler for starting new games.
        /// </summary>
        public sealed class StartGameHandler(IGameFactory gameFactory) : IRequestHandler<StartGameRequest, StartGameResponse>
        {
            private readonly IGameFactory _gameFactory = Guard.NotNull(gameFactory);

            public async Task<StartGameResponse> HandleAsync(StartGameRequest request, CancellationToken cancellationToken = default)
            {
                try
                {
                    // Validate request
                    if (request.Width <= 0 || request.Height <= 0)
                    {
                        return new StartGameResponse(false, "Invalid game dimensions", Guid.Empty);
                    }

                    // Create new game
                    var game = _gameFactory.CreateGame();
                    var gameId = Guid.NewGuid();

                    // In a real implementation, you would store the game instance
                    // GameManager.AddGame(gameId, game);

                    await Task.Delay(1, cancellationToken); // Simulate async work

                    return new StartGameResponse(true, null, gameId);
                }
                catch (Exception ex)
                {
                    return new StartGameResponse(false, ex.Message, Guid.Empty);
                }
            }
        }

        /// <summary>
        /// Handler for moving the snake.
        /// </summary>
        public sealed class MoveSnakeHandler : IRequestHandler<MoveSnakeRequest, MoveSnakeResponse>
        {
            public async Task<MoveSnakeResponse> HandleAsync(MoveSnakeRequest request, CancellationToken cancellationToken = default)
            {
                try
                {
                    // In a real implementation, you would:
                    // 1. Get the game instance by GameId
                    // 2. Validate the move
                    // 3. Execute the move
                    // 4. Check for game over conditions

                    await Task.Delay(1, cancellationToken); // Simulate async work

                    // Mock response
                    return new MoveSnakeResponse(true, false, 42);
                }
                catch (Exception)
                {
                    return new MoveSnakeResponse(false, true, 0);
                }
            }
        }

        /// <summary>
        /// Handler for pausing games.
        /// </summary>
        public sealed class PauseGameHandler : IRequestHandler<PauseGameRequest>
        {
            public async Task HandleAsync(PauseGameRequest request, CancellationToken cancellationToken = default)
            {
                // In a real implementation, you would pause the specified game
                await Task.Delay(1, cancellationToken); // Simulate async work
            }
        }
    }
}