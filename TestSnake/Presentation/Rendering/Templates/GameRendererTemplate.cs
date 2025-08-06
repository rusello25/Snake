using TestSnake.Core.Validation;
using TestSnake.Domain.ValueObjects;

namespace TestSnake.Presentation.Rendering.Templates
{
    /// <summary>
    /// Template Method pattern for game rendering with customizable steps.
    /// Defines the skeleton of the rendering algorithm while allowing subclasses to override specific steps.
    /// </summary>
    public abstract class GameRendererTemplate : IGameRenderer
    {
        /// <summary>
        /// Template method that defines the rendering algorithm.
        /// This method should not be overridden by subclasses.
        /// </summary>
        /// <param name="width">Game field width</param>
        /// <param name="height">Game field height</param>
        /// <param name="snake">Snake segments</param>
        /// <param name="food">Food position</param>
        /// <param name="obstacles">Obstacle positions</param>
        /// <param name="score">Current score</param>
        /// <param name="level">Current level</param>
        /// <param name="record">High score record</param>
        public void DrawGame(int width, int height, List<Position> snake, Position food, List<Position> obstacles, int score, int level, int record)
        {
            Guard.Positive(width);
            Guard.Positive(height);
            Guard.NotNull(snake);
            Guard.NotNull(obstacles);

            // Template method - defines the rendering steps
            BeginRendering();
            
            RenderGameField(width, height);
            RenderSnake(snake);
            RenderFood(food);
            RenderObstacles(obstacles);
            RenderUI(score, level, record);
            
            EndRendering();
        }

        /// <summary>
        /// Shows the start screen.
        /// </summary>
        /// <param name="record">Current high score record</param>
        public virtual void ShowStartScreen(int record)
        {
            BeginRendering();
            RenderStartScreen(record);
            EndRendering();
        }

        /// <summary>
        /// Shows the game over menu.
        /// </summary>
        /// <param name="score">Final score</param>
        /// <param name="record">High score record</param>
        public virtual void ShowGameOverMenu(int score, int record)
        {
            BeginRendering();
            RenderGameOverScreen(score, record);
            EndRendering();
        }

        /// <summary>
        /// Clears the screen.
        /// </summary>
        public abstract void ClearScreen();

        // Template method steps - can be overridden by subclasses

        /// <summary>
        /// Called at the beginning of rendering. Override to perform initialization.
        /// </summary>
        protected virtual void BeginRendering()
        {
            ClearScreen();
        }

        /// <summary>
        /// Renders the game field boundaries and background.
        /// </summary>
        /// <param name="width">Field width</param>
        /// <param name="height">Field height</param>
        protected abstract void RenderGameField(int width, int height);

        /// <summary>
        /// Renders the snake.
        /// </summary>
        /// <param name="snake">Snake segments</param>
        protected abstract void RenderSnake(List<Position> snake);

        /// <summary>
        /// Renders the food.
        /// </summary>
        /// <param name="food">Food position</param>
        protected abstract void RenderFood(Position food);

        /// <summary>
        /// Renders the obstacles.
        /// </summary>
        /// <param name="obstacles">Obstacle positions</param>
        protected abstract void RenderObstacles(List<Position> obstacles);

        /// <summary>
        /// Renders the UI elements (score, level, etc.).
        /// </summary>
        /// <param name="score">Current score</param>
        /// <param name="level">Current level</param>
        /// <param name="record">High score record</param>
        protected abstract void RenderUI(int score, int level, int record);

        /// <summary>
        /// Renders the start screen.
        /// </summary>
        /// <param name="record">High score record</param>
        protected abstract void RenderStartScreen(int record);

        /// <summary>
        /// Renders the game over screen.
        /// </summary>
        /// <param name="score">Final score</param>
        /// <param name="record">High score record</param>
        protected abstract void RenderGameOverScreen(int score, int record);

        /// <summary>
        /// Called at the end of rendering. Override to perform cleanup.
        /// </summary>
        protected virtual void EndRendering()
        {
            // Default implementation does nothing
        }

        // Hook methods - provide additional extension points

        /// <summary>
        /// Hook method called before rendering the snake. Override for custom behavior.
        /// </summary>
        /// <param name="snake">Snake segments</param>
        protected virtual void OnBeforeRenderSnake(List<Position> snake)
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Hook method called after rendering the snake. Override for custom behavior.
        /// </summary>
        /// <param name="snake">Snake segments</param>
        protected virtual void OnAfterRenderSnake(List<Position> snake)
        {
            // Default implementation does nothing
        }

        /// <summary>
        /// Hook method called when rendering special effects. Override for custom behavior.
        /// </summary>
        protected virtual void RenderSpecialEffects()
        {
            // Default implementation does nothing
        }
    }

    /// <summary>
    /// ASCII-based console renderer implementation.
    /// </summary>
    public sealed class AsciiConsoleRenderer : GameRendererTemplate
    {
        private const char WallChar = '#';
        private const char SnakeHeadChar = 'O';
        private const char SnakeBodyChar = 'o';
        private const char FoodChar = '*';
        private const char ObstacleChar = 'X';
        private const char EmptyChar = ' ';

        /// <inheritdoc />
        public override void ClearScreen()
        {
            System.Console.Clear();
        }

        /// <inheritdoc />
        protected override void RenderGameField(int width, int height)
        {
            // Render top border
            System.Console.WriteLine(new string(WallChar, width + 2));

            // Render empty field with side borders
            for (int y = 0; y < height; y++)
            {
                System.Console.Write(WallChar);
                System.Console.Write(new string(EmptyChar, width));
                System.Console.WriteLine(WallChar);
            }

            // Render bottom border
            System.Console.WriteLine(new string(WallChar, width + 2));
        }

        /// <inheritdoc />
        protected override void RenderSnake(List<Position> snake)
        {
            OnBeforeRenderSnake(snake);

            for (int i = 0; i < snake.Count; i++)
            {
                var segment = snake[i];
                var character = i == 0 ? SnakeHeadChar : SnakeBodyChar;
                
                System.Console.SetCursorPosition(segment.X + 1, segment.Y + 1);
                System.Console.Write(character);
            }

            OnAfterRenderSnake(snake);
        }

        /// <inheritdoc />
        protected override void RenderFood(Position food)
        {
            System.Console.SetCursorPosition(food.X + 1, food.Y + 1);
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.Write(FoodChar);
            System.Console.ResetColor();
        }

        /// <inheritdoc />
        protected override void RenderObstacles(List<Position> obstacles)
        {
            System.Console.ForegroundColor = ConsoleColor.DarkRed;
            foreach (var obstacle in obstacles)
            {
                System.Console.SetCursorPosition(obstacle.X + 1, obstacle.Y + 1);
                System.Console.Write(ObstacleChar);
            }
            System.Console.ResetColor();
        }

        /// <inheritdoc />
        protected override void RenderUI(int score, int level, int record)
        {
            // Position cursor below the game field
            System.Console.SetCursorPosition(0, System.Console.WindowHeight - 3);
            
            System.Console.WriteLine($"Score: {score} | Level: {level} | Record: {record}");
            System.Console.WriteLine("Use arrow keys to move, ESC to quit");
        }

        /// <inheritdoc />
        protected override void RenderStartScreen(int record)
        {
            System.Console.WriteLine("=== SNAKE GAME ===");
            System.Console.WriteLine();
            System.Console.WriteLine($"High Score: {record}");
            System.Console.WriteLine();
            System.Console.WriteLine("Controls:");
            System.Console.WriteLine("- Arrow keys to move");
            System.Console.WriteLine("- ESC to quit");
            System.Console.WriteLine();
            System.Console.WriteLine("Press any key to start...");
        }

        /// <inheritdoc />
        protected override void RenderGameOverScreen(int score, int record)
        {
            System.Console.WriteLine("=== GAME OVER ===");
            System.Console.WriteLine();
            System.Console.WriteLine($"Final Score: {score}");
            System.Console.WriteLine($"High Score: {record}");
            System.Console.WriteLine();
            System.Console.WriteLine("R - Restart");
            System.Console.WriteLine("E - Exit");
        }
    }

    /// <summary>
    /// Emoji-based console renderer implementation.
    /// </summary>
    public sealed class EmojiConsoleRenderer : GameRendererTemplate
    {
        private const string WallEmoji = "🧱";
        private const string SnakeHeadEmoji = "🐍";
        private const string SnakeBodyEmoji = "🟢";
        private const string FoodEmoji = "🍎";
        private const string ObstacleEmoji = "🌵";
        private const string EmptySpace = "  ";

        /// <inheritdoc />
        public override void ClearScreen()
        {
            System.Console.Clear();
        }

        /// <inheritdoc />
        protected override void RenderGameField(int width, int height)
        {
            // Render top border
            System.Console.Write(WallEmoji);
            for (int x = 0; x < width; x++)
                System.Console.Write(WallEmoji);
            System.Console.WriteLine(WallEmoji);

            // Render side borders with empty space
            for (int y = 0; y < height; y++)
            {
                System.Console.Write(WallEmoji);
                for (int x = 0; x < width; x++)
                    System.Console.Write(EmptySpace);
                System.Console.WriteLine(WallEmoji);
            }

            // Render bottom border
            System.Console.Write(WallEmoji);
            for (int x = 0; x < width; x++)
                System.Console.Write(WallEmoji);
            System.Console.WriteLine(WallEmoji);
        }

        /// <inheritdoc />
        protected override void RenderSnake(List<Position> snake)
        {
            OnBeforeRenderSnake(snake);

            for (int i = 0; i < snake.Count; i++)
            {
                var segment = snake[i];
                var emoji = i == 0 ? SnakeHeadEmoji : SnakeBodyEmoji;
                
                System.Console.SetCursorPosition((segment.X + 1) * 2, segment.Y + 1);
                System.Console.Write(emoji);
            }

            OnAfterRenderSnake(snake);
            RenderSpecialEffects();
        }

        /// <inheritdoc />
        protected override void RenderFood(Position food)
        {
            System.Console.SetCursorPosition((food.X + 1) * 2, food.Y + 1);
            System.Console.Write(FoodEmoji);
        }

        /// <inheritdoc />
        protected override void RenderObstacles(List<Position> obstacles)
        {
            foreach (var obstacle in obstacles)
            {
                System.Console.SetCursorPosition((obstacle.X + 1) * 2, obstacle.Y + 1);
                System.Console.Write(ObstacleEmoji);
            }
        }

        /// <inheritdoc />
        protected override void RenderUI(int score, int level, int record)
        {
            System.Console.SetCursorPosition(0, System.Console.WindowHeight - 4);
            
            if (score >= record && score > 0)
            {
                System.Console.WriteLine($"🏆 NEW RECORD: {score} 🏆");
            }
            else
            {
                System.Console.WriteLine($"🎮 Score: {score} | 🚩 Level: {level} | 🏆 Record: {record}");
            }
            
            System.Console.WriteLine("🎮 Use arrows to move, ESC to quit");
        }

        /// <inheritdoc />
        protected override void RenderStartScreen(int record)
        {
            System.Console.WriteLine("🐍 === SNAKE GAME === 🐍");
            System.Console.WriteLine();
            System.Console.WriteLine($"🏆 High Score: {record}");
            System.Console.WriteLine();
            System.Console.WriteLine("🎮 Controls:");
            System.Console.WriteLine("  ⬆️⬇️⬅️➡️ Arrow keys to move");
            System.Console.WriteLine("  🚪 ESC to quit");
            System.Console.WriteLine();
            System.Console.WriteLine("🍎 Eat apples to grow");
            System.Console.WriteLine("🌵 Avoid obstacles");
            System.Console.WriteLine();
            System.Console.WriteLine("Press any key to start... 🚀");
        }

        /// <inheritdoc />
        protected override void RenderGameOverScreen(int score, int record)
        {
            System.Console.WriteLine("💀 === GAME OVER === 💀");
            System.Console.WriteLine();
            
            if (score >= record && score > 0)
            {
                System.Console.WriteLine($"🎉 NEW RECORD: {score} 🎉");
            }
            else
            {
                System.Console.WriteLine($"💯 Final Score: {score}");
                System.Console.WriteLine($"🏆 High Score: {record}");
            }
            
            System.Console.WriteLine();
            System.Console.WriteLine("🔄 R - Restart");
            System.Console.WriteLine("🚪 E - Exit");
        }

        /// <inheritdoc />
        protected override void RenderSpecialEffects()
        {
            // Add blinking effect for high scores
            if (DateTime.Now.Millisecond / 250 % 2 == 0)
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else
            {
                System.Console.ResetColor();
            }
        }
    }

    /// <summary>
    /// Factory for creating different renderer implementations.
    /// </summary>
    public static class RendererFactory
    {
        /// <summary>
        /// Creates a renderer based on the specified type.
        /// </summary>
        /// <param name="rendererType">Type of renderer to create</param>
        /// <returns>Renderer instance</returns>
        public static GameRendererTemplate CreateRenderer(RendererType rendererType)
        {
            return rendererType switch
            {
                RendererType.Ascii => new AsciiConsoleRenderer(),
                RendererType.Emoji => new EmojiConsoleRenderer(),
                _ => throw new ArgumentException($"Unknown renderer type: {rendererType}")
            };
        }
    }

    /// <summary>
    /// Available renderer types.
    /// </summary>
    public enum RendererType
    {
        Ascii,
        Emoji
    }
}