using TestSnake.Core.Constants;

namespace TestSnake.Core.Configuration
{
    public class GameConfig
    {
        public int Width { get; set; } = GameConstants.DefaultWidth;
        public int Height { get; set; } = GameConstants.DefaultHeight;
        public int MaxCactus { get; set; } = 30;
        public int PointsPerLevel { get; set; } = 3;

        public void AdaptToFieldSize()
        {
            var fieldSize = Width * Height;
            
            MaxCactus = Math.Max(
                GameConstants.ObstacleGeneration.MinimumObstacles,
                (int)(fieldSize * GameConstants.ObstacleGeneration.ObstacleFieldPercentage));
                
            PointsPerLevel = Math.Max(
                GameConstants.LevelProgression.MinimumPointsPerLevel,
                (int)(fieldSize * GameConstants.LevelProgression.PointsPerLevelFieldPercentage));
        }
    }
}