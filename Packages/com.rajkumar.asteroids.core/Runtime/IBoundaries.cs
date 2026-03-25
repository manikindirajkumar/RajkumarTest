namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Defines the screen boundaries used for wrapping
    /// and spawn position calculation.
    /// Read-only — boundaries are configured once at startup,
    /// not modified during gameplay.
    /// </summary>
    public interface IBoundaries
    {
        float MinX { get; }
        float MaxX { get; }
        float MinY { get; }
        float MaxY { get; }
    }
}