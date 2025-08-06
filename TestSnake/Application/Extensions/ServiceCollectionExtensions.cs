using Microsoft.Extensions.DependencyInjection;
using TestSnake.Application.Events;
using TestSnake.Application.Factories;
using TestSnake.Application.Interfaces;
using TestSnake.Application.Services;
using TestSnake.Core.Constants;

namespace TestSnake.Application.Extensions
{
    /// <summary>
    /// Extension methods for registering application services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all application services required by the application.
        /// </summary>
        /// <param name="services">The service collection to extend</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Event handling - can switch between implementations
            services.AddSingleton<IEventAggregator, EventAggregator>(); // Use simple implementation
            // services.AddSingleton<IEventAggregator, EnterpriseEventAggregator>(); // Use advanced implementation
            
            // Application services - using scoped lifetime for services that manage state
            services.AddScoped<IScoreService, ScoreService>();
            services.AddScoped<ILevelService>(serviceProvider => 
                new LevelService(GameConstants.PointsPerLevel, serviceProvider.GetRequiredService<IEventAggregator>()));
            
            // Factories - using transient for factory that creates scoped services
            services.AddTransient<IGameFactory, GameFactory>();
            
            // Session management
            services.AddSingleton<IGameSessionManager, GameSessionManager>();

            return services;
        }
    }
}