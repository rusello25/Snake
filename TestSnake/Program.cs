using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestSnake.Application.Extensions;
using TestSnake.Core.Extensions;
using TestSnake.Core.GameEngine;
using TestSnake.Infrastructure.Console;
using TestSnake.Infrastructure.Extensions;
using TestSnake.Presentation.Extensions;

namespace TestSnake
{
    /// <summary>
    /// Main entry point for the Snake Game application.
    /// Implements proper bootstrapping with dependency injection and graceful error handling.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code: 0 for success, 1 for error</returns>
        internal static async Task<int> Main(string[] args)
        {
            using var host = CreateHost(args);
            
            try
            {
                await InitializeApplicationAsync(host);
                await RunGameEngineAsync(host);
                return ExitCodes.Success;
            }
            catch (OperationCanceledException)
            {
                // Expected when the application is cancelled gracefully
                return ExitCodes.Success;
            }
            catch (Exception ex)
            {
                await LogCriticalErrorAsync(host, ex);
                return ExitCodes.Error;
            }
        }

        /// <summary>
        /// Creates and configures the host with all required services.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Configured host instance</returns>
        private static IHost CreateHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(ConfigureLogging)
                .UseConsoleLifetime()
                .Build();
        }

        /// <summary>
        /// Configures all application services using extension methods for better organization.
        /// </summary>
        /// <param name="services">Service collection to configure</param>
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddCoreServices();
            services.AddApplicationServices();
            services.AddInfrastructureServices();
            services.AddPresentationServices();
        }

        /// <summary>
        /// Configures logging for the application.
        /// </summary>
        /// <param name="logging">Logging builder to configure</param>
        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information);
        }

        /// <summary>
        /// Initializes application components that require async initialization.
        /// </summary>
        /// <param name="host">Host instance</param>
        private static async Task InitializeApplicationAsync(IHost host)
        {
            var consoleService = host.Services.GetRequiredService<IConsoleService>();
            await consoleService.InitializeAsync();
        }

        /// <summary>
        /// Runs the main game engine.
        /// </summary>
        /// <param name="host">Host instance</param>
        private static async Task RunGameEngineAsync(IHost host)
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            
            // Setup graceful shutdown
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            var gameEngine = host.Services.GetRequiredService<IGameEngine>();
            await gameEngine.RunAsync(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Logs critical errors that prevent the application from starting or running.
        /// </summary>
        /// <param name="host">Host instance</param>
        /// <param name="exception">Exception that occurred</param>
        private static async Task LogCriticalErrorAsync(IHost host, Exception exception)
        {
            try
            {
                var logger = host.Services.GetService<ILogger<IGameEngine>>();
                logger?.LogCritical(exception, "A critical error occurred that prevented the game from running");
                
                // Show user-friendly error message
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An unexpected error occurred. Please check the logs for details.");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                
                // Use Task.Run to make the ReadKey operation non-blocking and truly async
                await Task.Run(() => Console.ReadKey());
            }
            catch
            {
                // Fallback error handling
                Console.WriteLine($"Critical error: {exception.Message}");
                await Task.Delay(100); // Small delay to ensure console output is flushed
            }
        }

        /// <summary>
        /// Application exit codes.
        /// </summary>
        private static class ExitCodes
        {
            internal const int Success = 0;
            internal const int Error = 1;
        }
    }
}