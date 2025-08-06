using Microsoft.Extensions.DependencyInjection;
using TestSnake.Domain.Repositories;
using TestSnake.Infrastructure.Audio.Midi;
using TestSnake.Infrastructure.Audio.Services;
using TestSnake.Infrastructure.Console;
using TestSnake.Infrastructure.IO.Input;
using TestSnake.Infrastructure.Managers.Record;
using TestSnake.Infrastructure.Persistence;

namespace TestSnake.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering infrastructure services in the dependency injection container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all infrastructure services required by the application.
        /// </summary>
        /// <param name="services">The service collection to extend</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Console services
            services.AddSingleton<IConsoleService, ConsoleService>();
            
            // Input handling
            services.AddSingleton<IInputHandler, ConsoleInputHandler>();
            
            // Audio services
            services.AddSingleton<IMidiPlayer, MidiPlayer>();
            services.AddSingleton<ISoundManager, SoundManager>();
            
            // Data persistence
            services.AddSingleton<IRecordRepository, FileRecordRepository>();
            services.AddSingleton<IRecordManager, RecordManager>();

            return services;
        }
    }
}