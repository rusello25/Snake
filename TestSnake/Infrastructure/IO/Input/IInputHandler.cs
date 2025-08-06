namespace TestSnake.Infrastructure.IO.Input
{
    public interface IInputHandler
    {
        ConsoleKey? GetNextCommand();
    }
}