namespace TestSnake.Core.Constants
{
    /// <summary>
    /// Contains all game-related constants used throughout the application.
    /// These constants define the core game mechanics and configuration values.
    /// </summary>
    public static class GameConstants
    {
        // Core game dimensions
        public const int DefaultWidth = 20;
        public const int DefaultHeight = 10;
        public const int DefaultInitialSnakeLength = 3;
        
        // Game timing
        public const int GameUpdateIntervalMs = 150;
        
        // UI
        public const string GameTitle = "🐍 Snake Game 🐍";
        
        // Level progression
        public const int PointsPerLevel = 5;
        
        /// <summary>
        /// Configuration for obstacle generation mechanics.
        /// </summary>
        public static class ObstacleGeneration
        {
            public const int MinimumObstacles = 5;
            public const double ObstacleFieldPercentage = 0.02; // 2% of field
        }
        
        /// <summary>
        /// Configuration for level progression mechanics.
        /// </summary>
        public static class LevelProgression
        {
            public const double PointsPerLevelFieldPercentage = 0.01; // 1% of field
            public const int MinimumPointsPerLevel = 2;
            public const double BaseSpeedMultiplier = 1.0;
            public const double SpeedIncreasePerLevel = 0.1;
        }

        /// <summary>
        /// Configuration for game physics and collision detection.
        /// </summary>
        public static class Physics
        {
            public const int CollisionTolerance = 0;
        }

        /// <summary>
        /// Configuration for audio and visual effects.
        /// </summary>
        public static class Effects
        {
            public const int BlinkIntervalMs = 500;
            public const int FanfareDelayMs = 100;
        }
    }
}