using System.Text;
using TestSnake.Domain.ValueObjects;
using TestSnake.Presentation.Console;

namespace TestSnake.Presentation.Rendering
{
    public class ConsoleGameRenderer : IGameRenderer
    {
        public static readonly string WallEmoji = "🧱";
        public static readonly string HeadEmoji = "🐍";
        public static readonly string BodyEmoji = "🟢";
        public static readonly string FoodEmoji = "🍎";
        public static readonly string EmptyEmoji = "  ";
        public static readonly string CornerEmoji = "🧱";
        public static readonly string ObstacleEmoji = "🌵";

        public static readonly string WallAscii = "##";
        public static readonly string HeadAscii = "@@";
        public static readonly string BodyAscii = "[]";
        public static readonly string FoodAscii = "()";
        public static readonly string CornerAscii = "##";
        public static readonly string ObstacleAscii = "XX";

        private readonly bool _useEmoji;

        public ConsoleGameRenderer(bool useEmoji = true)
        {
            _useEmoji = useEmoji && IsEmojiSupported();
            try
            {
                System.Console.OutputEncoding = Encoding.UTF8;
                System.Console.InputEncoding = Encoding.UTF8;
            }
            catch
            {
                _useEmoji = false;
            }
        }

        private static bool IsEmojiSupported()
        {
            try
            {
                var (left, top) = System.Console.GetCursorPosition();
                System.Console.Write("🐍");
                System.Console.SetCursorPosition(left, top);
                System.Console.Write(" ");
                System.Console.SetCursorPosition(left, top);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void DrawGame(int width, int height, List<Position> snake, Position food, List<Position> obstacles, int score, int level, int record)
        {
            var wall = _useEmoji ? WallEmoji : WallAscii;
            var corner = _useEmoji ? CornerEmoji : CornerAscii;
            var head = _useEmoji ? HeadEmoji : HeadAscii;
            var body = _useEmoji ? BodyEmoji : BodyAscii;
            var foodChar = _useEmoji ? FoodEmoji : FoodAscii;
            var obstacle = _useEmoji ? ObstacleEmoji : ObstacleAscii;

            int gameWidth = GetGameWidth(width);
            int gameHeight = GetGameHeight(height);
            int padLeft = GetPadLeft(gameWidth);
            int padTop = GetPadTop(gameHeight);

            for (int i = 0; i < padTop; i++) System.Console.WriteLine();

            for (int y = -1; y <= height; y++)
            {
                System.Console.Write(new string(' ', padLeft));
                for (int x = -1; x <= width; x++)
                {
                    if (x == -1 && y == -1 || x == width && y == -1 || 
                        x == -1 && y == height || x == width && y == height)
                    {
                        System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                        System.Console.Write(corner);
                        System.Console.ResetColor();
                    }
                    else if ((y == -1 || y == height) && x >= 0 && x < width)
                    {
                        System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                        System.Console.Write(wall);
                        System.Console.ResetColor();
                    }
                    else if ((x == -1 || x == width) && y >= 0 && y < height)
                    {
                        System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                        System.Console.Write(wall);
                        System.Console.ResetColor();
                    }
                    else if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        var pos = new Position(x, y);
                        if (pos.Equals(food))
                        {
                            System.Console.ForegroundColor = ConsoleColor.Red;
                            System.Console.Write(foodChar);
                            System.Console.ResetColor();
                        }
                        else if (obstacles.Contains(pos))
                        {
                            System.Console.ForegroundColor = ConsoleColor.DarkGreen;
                            System.Console.Write(obstacle);
                            System.Console.ResetColor();
                        }
                        else if (snake.Contains(pos))
                        {
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            if (pos.Equals(snake[0]))
                                System.Console.Write(head);
                            else
                                System.Console.Write(body);
                            System.Console.ResetColor();
                        }
                        else
                        {
                            System.Console.Write(EmptyEmoji);
                        }
                    }
                }
                System.Console.WriteLine();
            }

            for (int i = 0; i < 2; i++) System.Console.WriteLine();
            
            int infoPad;
            
            if (score < record)
            {
                string scoreText = $" Score: {score}   ";
                string levelText = $" Level: {level}";
                
                infoPad = Math.Max(0, (System.Console.WindowWidth - (1 + scoreText.Length + 1 + levelText.Length)) / 2);
                
                System.Console.Write(new string(' ', infoPad));
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write("🏆");
                System.Console.ResetColor();
                System.Console.Write(scoreText);
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("🚩");
                System.Console.ResetColor();
                System.Console.WriteLine(levelText);
            }
            else
            {
                string recordText = $" RECORD: {score}   ";
                string levelText = $" Level: {level}";
                
                infoPad = Math.Max(0, (System.Console.WindowWidth - (1 + recordText.Length + 1 + levelText.Length)) / 2);
                
                System.Console.Write(new string(' ', infoPad));
                
                bool shouldBlink = DateTime.Now.Millisecond / 500 % 2 == 0;
                System.Console.ForegroundColor = shouldBlink ? ConsoleColor.DarkYellow : ConsoleColor.Yellow;
                System.Console.Write("🥇");
                System.Console.ResetColor();
                
                System.Console.Write(recordText);
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("🚩");
                System.Console.ResetColor();
                System.Console.WriteLine(levelText);
            }
            
            string help = "🎮 Use arrows to move, ESC to quit";
            int helpPad = Math.Max(0, (System.Console.WindowWidth - help.Length) / 2);
            System.Console.WriteLine(new string(' ', helpPad) + help);
        }

        public void ShowStartScreen(int record)
        {
            var snake = _useEmoji ? "🐍" : "===";
            var trophy = _useEmoji ? "🏆" : "[TOP]";
            var gamepad = _useEmoji ? "🎮" : "[KEY]";
            var apple = _useEmoji ? "🍎" : "()";
            var cactus = _useEmoji ? "🌵" : "XX";
            var wall = _useEmoji ? "🧱" : "##";

            var rows = new List<MenuRow>
            {
                new($"{snake} SNAKE GAME {snake}", ConsoleColor.Green),
                new(""),
                new($"{trophy} Record: {record}", ConsoleColor.Yellow),
                new(""),
                new($"{gamepad} Use arrows to move", ConsoleColor.Cyan),
                new($"{apple} Eat apples to grow", ConsoleColor.Red),
                new($"{cactus} Avoid cactus and {wall} walls", ConsoleColor.DarkGreen),
                new(""),
                new("Press any key to start", ConsoleColor.Magenta),
                new("(N to disable sounds)", ConsoleColor.Magenta),
            };
            
            var wallChar = _useEmoji ? WallEmoji : WallAscii;
            var cornerChar = _useEmoji ? CornerEmoji : CornerAscii;
            MenuRenderer.RenderMenu(rows, 20, 0, wallChar, cornerChar);
        }

        public void ShowGameOverMenu(int score, int record)
        {
            var skull = _useEmoji ? "💀" : "[X_X]";
            var medal = _useEmoji ? "🥇" : "[!";
            var repeat = _useEmoji ? "🔄" : "[R]";
            var exit = _useEmoji ? "🚪" : "[E]";

            var rows = new List<MenuRow>
            {
                new(""),
                new($"{skull} GAME OVER {skull}", ConsoleColor.Red),
                new(""),
                new($" Final Score: {score}", ConsoleColor.White),
                new($"{medal} Record: {record}", ConsoleColor.Yellow),
                new(""),
                new($"{repeat} [R] Restart", ConsoleColor.Green),
                new($"{exit} [E] Exit", ConsoleColor.Red),
                new(""),
                new("* Choose an option...", ConsoleColor.Cyan),
                new(""),
            };
            
            var wallChar = _useEmoji ? WallEmoji : WallAscii;
            var cornerChar = _useEmoji ? CornerEmoji : CornerAscii;
            MenuRenderer.RenderMenu(rows, 20, 0, wallChar, cornerChar);
        }

        public void ClearScreen()
        {
            System.Console.Clear();
        }

        private static int GetGameWidth(int width) => (width + 2) * 2;
        private static int GetGameHeight(int height) => height + 4;
        private static int GetPadLeft(int gameWidth) => Math.Max(0, (System.Console.WindowWidth - gameWidth) / 2);
        private static int GetPadTop(int gameHeight) => Math.Max(0, (System.Console.WindowHeight - gameHeight) / 2);
    }
}