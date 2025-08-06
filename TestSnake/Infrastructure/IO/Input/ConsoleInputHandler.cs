using TestSnake.Infrastructure.Console;

namespace TestSnake.Infrastructure.IO.Input
{
    public class ConsoleInputHandler(IConsoleService consoleService) : IInputHandler
    {
        private readonly IConsoleService _consoleService = consoleService;

        public ConsoleKey? GetNextCommand()
        {
            if (_consoleService.KeyAvailable)
            {
                var keyInfo = _consoleService.ReadKey(true);
                return keyInfo.Key;
            }
            return null;
        }
    }
}