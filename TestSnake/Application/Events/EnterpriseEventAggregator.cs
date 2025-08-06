using System.Collections.Concurrent;
using TestSnake.Core.Validation;
using TestSnake.Domain.Events;

namespace TestSnake.Application.Events
{
    /// <summary>
    /// Advanced event handler interface with priority and filtering support.
    /// </summary>
    /// <typeparam name="TEvent">Type of event to handle</typeparam>
    public interface IEventHandler<in TEvent> where TEvent : IGameEvent
    {
        /// <summary>
        /// Handles the specified event.
        /// </summary>
        /// <param name="gameEvent">Event to handle</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the handling operation</returns>
        Task HandleAsync(TEvent gameEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the priority of this handler (lower numbers = higher priority).
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Determines if this handler should process the given event.
        /// </summary>
        /// <param name="gameEvent">Event to check</param>
        /// <returns>True if the handler should process the event, false otherwise</returns>
        bool CanHandle(TEvent gameEvent);
    }

    /// <summary>
    /// Base implementation for event handlers with common functionality.
    /// </summary>
    /// <typeparam name="TEvent">Type of event to handle</typeparam>
    public abstract class EventHandlerBase<TEvent> : IEventHandler<TEvent> where TEvent : IGameEvent
    {
        /// <inheritdoc />
        public abstract Task HandleAsync(TEvent gameEvent, CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public virtual int Priority => 0;

        /// <inheritdoc />
        public virtual bool CanHandle(TEvent gameEvent) => true;
    }

    /// <summary>
    /// Event subscription information.
    /// </summary>
    internal sealed class EventSubscription
    {
        public required Type EventType { get; init; }
        public required Type HandlerType { get; init; }
        public required object Handler { get; init; }
        public required int Priority { get; init; }
        public required Func<IGameEvent, bool> CanHandle { get; init; }
        public required Func<IGameEvent, CancellationToken, Task> HandleAsync { get; init; }
    }

    /// <summary>
    /// Simple action-based subscription for compatibility.
    /// </summary>
    internal sealed class ActionSubscription
    {
        public required Type EventType { get; init; }
        public required object Action { get; init; }
    }

    /// <summary>
    /// Enterprise-grade event aggregator with advanced features.
    /// Implements Observer pattern with priority handling, filtering, and async support.
    /// </summary>
    public sealed class EnterpriseEventAggregator : IEventAggregator, IDisposable
    {
        private readonly ConcurrentDictionary<Type, List<EventSubscription>> _subscriptions = [];
        private readonly ConcurrentDictionary<Type, List<ActionSubscription>> _actionSubscriptions = [];
        private readonly SemaphoreSlim _subscriptionLock = new(1, 1);
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _disposed;

        /// <summary>
        /// Subscribes an event handler to events of the specified type.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to subscribe to</typeparam>
        /// <param name="handler">Handler to subscribe</param>
        public async Task SubscribeAsync<TEvent>(IEventHandler<TEvent> handler) where TEvent : IGameEvent
        {
            Guard.NotNull(handler);
            ThrowIfDisposed();

            await _subscriptionLock.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                var eventType = typeof(TEvent);
                var subscription = new EventSubscription
                {
                    EventType = eventType,
                    HandlerType = handler.GetType(),
                    Handler = handler,
                    Priority = handler.Priority,
                    CanHandle = evt => evt is TEvent typedEvent && handler.CanHandle(typedEvent),
                    HandleAsync = (evt, ct) => handler.HandleAsync((TEvent)evt, ct)
                };

                var subscriptionList = _subscriptions.GetOrAdd(eventType, _ => []);
                subscriptionList.Add(subscription);

                // Sort by priority (lower numbers = higher priority)
                subscriptionList.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

        /// <inheritdoc />
        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            Guard.NotNull(handler);
            ThrowIfDisposed();

            var eventType = typeof(T);
            var subscription = new ActionSubscription
            {
                EventType = eventType,
                Action = handler
            };

            var subscriptionList = _actionSubscriptions.GetOrAdd(eventType, _ => []);
            subscriptionList.Add(subscription);
        }

        /// <inheritdoc />
        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            Guard.NotNull(handler);
            ThrowIfDisposed();

            var eventType = typeof(T);
            if (_actionSubscriptions.TryGetValue(eventType, out var subscriptionList))
            {
                subscriptionList.RemoveAll(s => ReferenceEquals(s.Action, handler));
                
                if (subscriptionList.Count == 0)
                {
                    _actionSubscriptions.TryRemove(eventType, out _);
                }
            }
        }

        /// <summary>
        /// Unsubscribes an event handler from events.
        /// </summary>
        /// <typeparam name="TEvent">Type of event to unsubscribe from</typeparam>
        /// <param name="handler">Handler to unsubscribe</param>
        public async Task UnsubscribeAsync<TEvent>(IEventHandler<TEvent> handler) where TEvent : IGameEvent
        {
            Guard.NotNull(handler);
            ThrowIfDisposed();

            await _subscriptionLock.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                var eventType = typeof(TEvent);
                if (_subscriptions.TryGetValue(eventType, out var subscriptionList))
                {
                    subscriptionList.RemoveAll(s => ReferenceEquals(s.Handler, handler));
                    
                    if (subscriptionList.Count == 0)
                    {
                        _subscriptions.TryRemove(eventType, out _);
                    }
                }
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

        /// <inheritdoc />
        public void Publish<T>(T gameEvent) where T : IGameEvent
        {
            if (gameEvent == null) return;
            ThrowIfDisposed();

            // Handle action-based subscriptions (synchronous, for compatibility)
            var eventType = typeof(T);
            if (_actionSubscriptions.TryGetValue(eventType, out var actionSubscriptions))
            {
                foreach (var subscription in actionSubscriptions.ToList())
                {
                    try
                    {
                        if (subscription.Action is Action<T> action)
                        {
                            action(gameEvent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in action subscription: {ex.Message}");
                    }
                }
            }

            // Fire and forget async operation for advanced handlers
            _ = PublishAsync(gameEvent, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Publishes an event asynchronously to all subscribed handlers.
        /// </summary>
        /// <typeparam name="T">Type of event to publish</typeparam>
        /// <param name="gameEvent">Event to publish</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task PublishAsync<T>(T gameEvent, CancellationToken cancellationToken = default) where T : IGameEvent
        {
            if (gameEvent == null) return;
            ThrowIfDisposed();

            var eventType = typeof(T);
            if (!_subscriptions.TryGetValue(eventType, out var subscriptionList))
                return;

            // Create a snapshot of subscriptions to avoid modification during iteration
            List<EventSubscription> subscriptionsSnapshot;
            await _subscriptionLock.WaitAsync(cancellationToken);
            try
            {
                subscriptionsSnapshot = [.. subscriptionList];
            }
            finally
            {
                _subscriptionLock.Release();
            }

            // Execute handlers in parallel but maintain priority order within each priority group
            var handlerTasks = new List<Task>();
            
            foreach (var subscription in subscriptionsSnapshot)
            {
                if (subscription.CanHandle(gameEvent))
                {
                    handlerTasks.Add(ExecuteHandlerSafelyAsync(subscription, gameEvent, cancellationToken));
                }
            }
            
            // Wait for all handlers to complete
            if (handlerTasks.Count > 0)
            {
                await Task.WhenAll(handlerTasks);
            }
        }

        /// <summary>
        /// Gets the number of subscribed handlers for the specified event type.
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <returns>Number of subscribed handlers</returns>
        public int GetSubscriberCount<T>() where T : IGameEvent
        {
            var eventType = typeof(T);
            var advancedCount = _subscriptions.TryGetValue(eventType, out var subscriptionList) ? subscriptionList.Count : 0;
            var actionCount = _actionSubscriptions.TryGetValue(eventType, out var actionList) ? actionList.Count : 0;
            return advancedCount + actionCount;
        }

        /// <summary>
        /// Clears all event subscriptions.
        /// </summary>
        public async Task ClearAllSubscriptionsAsync()
        {
            ThrowIfDisposed();

            await _subscriptionLock.WaitAsync(_cancellationTokenSource.Token);
            try
            {
                _subscriptions.Clear();
                _actionSubscriptions.Clear();
            }
            finally
            {
                _subscriptionLock.Release();
            }
        }

        /// <summary>
        /// Executes an event handler safely with error handling.
        /// </summary>
        /// <param name="subscription">Subscription to execute</param>
        /// <param name="gameEvent">Event to handle</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private static async Task ExecuteHandlerSafelyAsync(
            EventSubscription subscription,
            IGameEvent gameEvent,
            CancellationToken cancellationToken)
        {
            try
            {
                await subscription.HandleAsync(gameEvent, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
                throw;
            }
            catch (Exception ex)
            {
                // Log error but don't let one handler failure affect others
                Console.WriteLine($"Error in event handler {subscription.HandlerType.Name}: {ex.Message}");
            }
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _subscriptionLock.Dispose();
                _subscriptions.Clear();
                _actionSubscriptions.Clear();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Example event handlers for demonstration.
    /// </summary>
    namespace Handlers
    {
        /// <summary>
        /// Handler for score-related events with high priority.
        /// </summary>
        public sealed class ScoreEventHandler : EventHandlerBase<ScoreChangedEvent>
        {
            public override int Priority => -100; // High priority

            public override async Task HandleAsync(ScoreChangedEvent gameEvent, CancellationToken cancellationToken = default)
            {
                // Handle score change logic
                await Task.Delay(1, cancellationToken); // Simulate async work
                Console.WriteLine($"Score updated to: {gameEvent.Score}");
            }
        }

        /// <summary>
        /// Handler for game over events with standard priority.
        /// </summary>
        public sealed class GameOverEventHandler : EventHandlerBase<GameOverEvent>
        {
            public override async Task HandleAsync(GameOverEvent gameEvent, CancellationToken cancellationToken = default)
            {
                // Handle game over logic
                await Task.Delay(1, cancellationToken); // Simulate async work
                Console.WriteLine("Game Over - Cleaning up resources");
            }
        }

        /// <summary>
        /// Conditional handler that only processes certain food events.
        /// </summary>
        public sealed class ConditionalFoodEventHandler : EventHandlerBase<FoodEatenEvent>
        {
            public override bool CanHandle(FoodEatenEvent gameEvent)
            {
                // Only handle food events where score is a multiple of 10
                return gameEvent.NewScore % 10 == 0;
            }

            public override async Task HandleAsync(FoodEatenEvent gameEvent, CancellationToken cancellationToken = default)
            {
                await Task.Delay(1, cancellationToken); // Simulate async work
                Console.WriteLine($"Milestone reached! Score: {gameEvent.NewScore}");
            }
        }
    }
}