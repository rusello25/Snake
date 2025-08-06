using TestSnake.Domain.ValueObjects;

namespace TestSnake.Presentation.Rendering
{
    public interface IGameRenderer
    {
        void DrawGame(int width, int height, List<Position> snake, Position food, List<Position> obstacles, int score, int level, int record);
        void ShowStartScreen(int record);
        void ShowGameOverMenu(int score, int record);
        void ClearScreen();
    }
}
