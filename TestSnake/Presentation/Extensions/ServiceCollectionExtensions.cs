using Microsoft.Extensions.DependencyInjection;
using TestSnake.Presentation.Rendering;

namespace TestSnake.Presentation.Extensions
{
    /// <summary>
    /// Extension methods for registering presentation services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all presentation services required by the application.
        /// </summary>
        /// <param name="services">The service collection to extend</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            // Rendering services
            services.AddSingleton<IGameRenderer>(serviceProvider => 
                new ConsoleGameRenderer(useEmoji: true));

            return services;
        }
    }
}