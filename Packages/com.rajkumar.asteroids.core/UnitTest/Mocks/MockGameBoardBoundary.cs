using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Test double for IBoundaries.
    /// Provides fixed, known boundary values
    /// without needing a camera or scene.
    /// Already exists as MockBoundaries —
    /// this is just an alias showing intent.
    /// </summary>
    public class MockGameBoardBoundary : IBoundaries
    {
        public float MinX { get; }
        public float MaxX { get; }
        public float MinY { get; }
        public float MaxY { get; }

        public MockGameBoardBoundary(
            float minX, float maxX,
            float minY, float maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
    }
}