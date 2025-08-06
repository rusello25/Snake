using Microsoft.Extensions.DependencyInjection;
using TestSnake.Core.Configuration;
using TestSnake.Core.Constants;
using TestSnake.Core.GameElements.Food;
using TestSnake.Core.GameElements.Obstacles;
using TestSnake.Core.GameEngine;
using TestSnake.Core.Physics;
using TestSnake.Domain.Services;

namespace TestSnake.Core.Extensions
{
    /// <summary>
    /// Extension methods for registering core services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all core services required by the application.
        /// </summary>
        /// <param name="services">The service collection to extend</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // Configuration
            services.AddSingleton<GameConfig>(CreateGameConfiguration);
            
            // Core game components
            services.AddSingleton<IGameEngine, SnakeGameEngine>();
            services.AddSingleton<ICollisionDetector, CollisionDetector>();
            services.AddSingleton<IFoodGenerator, FoodGenerator>();
            services.AddSingleton<IObstacleManager, ObstacleManager>();
            
            // Domain services
            services.AddSingleton<IGameProgressionService, GameProgressionService>();

            return services;
        }

        /// <summary>
        /// Creates and configures the game configuration.
        /// </summary>
        /// <param name="serviceProvider">Service provider for dependency resolution</param>
        /// <returns>Configured game configuration instance</returns>
        private static GameConfig CreateGameConfiguration(IServiceProvider serviceProvider)
        {
            var config = new GameConfig
            {
                Width = GameConstants.DefaultWidth,
                Height = GameConstants.DefaultHeight
            };
            
            config.AdaptToFieldSize();
            return config;
        }
    }
}