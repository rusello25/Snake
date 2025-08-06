using System.Text;
using TestSnake.Core.Constants;

namespace TestSnake.Infrastructure.Console
{
    /// <summary>
    /// Defines the contract for console operations in the application.
    /// Provides an abstraction layer over system console operations for better testability.
    /// </summary>
    public interface IConsoleService
    {
        /// <summary>
        /// Asynchronously initializes the console with the specified configuration.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the initialization</param>
        /// <returns>A task representing the asynchronous initialization operation</returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the cursor position in the console.
        /// </summary>
        /// <param name="left">The column position of the cursor</param>
        /// <param name="top">The row position of the cursor</param>
        void SetCursorPosition(int left, int top);

        /// <summary>
        /// Sets the visibility of the cursor.
        /// </summary>
        /// <param name="visible">True to make the cursor visible, false to hide it</param>
        void SetCursorVisible(bool visible);

        /// <summary>
        /// Clears the console screen.
        /// </summary>
        void Clear();

        /// <summary>
        /// Writes text to the console without a newline.
        /// </summary>
        /// <param name="text">The text to write</param>
        void Write(string text);

        /// <summary>
        /// Writes a line of text to the console.
        /// </summary>
        /// <param name="text">The text to write</param>
        void WriteLine(string text = "");

        /// <summary>
        /// Reads a key from the console.
        /// </summary>
        /// <param name="intercept">True to intercept the key (not display it), false otherwise</param>
        /// <returns>Information about the key that was pressed</returns>
        ConsoleKeyInfo ReadKey(bool intercept = true);

        /// <summary>
        /// Gets a value indicating whether a key press is available in the input stream.
        /// </summary>
        bool KeyAvailable { get; }

        /// <summary>
        /// Gets the width of the console window.
        /// </summary>
        int WindowWidth { get; }

        /// <summary>
        /// Gets the height of the console window.
        /// </summary>
        int WindowHeight { get; }

        /// <summary>
        /// Sets the foreground color of the console.
        /// </summary>
        /// <param name="color">The color to set</param>
        void SetForegroundColor(ConsoleColor color);

        /// <summary>
        /// Resets the console colors to their default values.
        /// </summary>
        void ResetColor();
    }

    /// <summary>
    /// Implementation of IConsoleService that wraps the system console.
    /// Provides thread-safe access to console operations with proper error handling.
    /// </summary>
    internal sealed class ConsoleService : IConsoleService
    {
        private readonly object _lock = new();
        private bool _isInitialized;

        /// <inheritdoc />
        public bool KeyAvailable 
        { 
            get 
            { 
                lock (_lock) 
                { 
                    return System.Console.KeyAvailable; 
                } 
            } 
        }

        /// <inheritdoc />
        public int WindowWidth 
        { 
            get 
            { 
                lock (_lock) 
                { 
                    return System.Console.WindowWidth; 
                } 
            } 
        }

        /// <inheritdoc />
        public int WindowHeight 
        { 
            get 
            { 
                lock (_lock) 
                { 
                    return System.Console.WindowHeight; 
                } 
            } 
        }

        /// <inheritdoc />
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            if (_isInitialized)
                return;

            await Task.Run(() =>
            {
                lock (_lock)
                {
                    if (_isInitialized)
                        return;

                    try
                    {
                        System.Console.OutputEncoding = Encoding.UTF8;
                        System.Console.InputEncoding = Encoding.UTF8;
                    }
                    catch (Exception ex)
                    {
                        // Log warning but continue - encoding is not critical
                        System.Console.WriteLine($"Warning: Could not set UTF-8 encoding: {ex.Message}");
                    }

                    SetCursorVisible(false);
                    System.Console.Title = GameConstants.GameTitle;
                    Clear();
                    
                    _isInitialized = true;
                }
            }, cancellationToken);
        }

        /// <inheritdoc />
        public void SetCursorPosition(int left, int top)
        {
            lock (_lock)
            {
                System.Console.SetCursorPosition(left, top);
            }
        }

        /// <inheritdoc />
        public void SetCursorVisible(bool visible)
        {
            lock (_lock)
            {
                System.Console.CursorVisible = visible;
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (_lock)
            {
                System.Console.Clear();
            }
        }

        /// <inheritdoc />
        public void Write(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            lock (_lock)
            {
                System.Console.Write(text);
            }
        }

        /// <inheritdoc />
        public void WriteLine(string text = "")
        {
            lock (_lock)
            {
                System.Console.WriteLine(text);
            }
        }

        /// <inheritdoc />
        public ConsoleKeyInfo ReadKey(bool intercept = true)
        {
            lock (_lock)
            {
                return System.Console.ReadKey(intercept);
            }
        }

        /// <inheritdoc />
        public void SetForegroundColor(ConsoleColor color)
        {
            lock (_lock)
            {
                System.Console.ForegroundColor = color;
            }
        }

        /// <inheritdoc />
        public void ResetColor()
        {
            lock (_lock)
            {
                System.Console.ResetColor();
            }
        }
    }
}