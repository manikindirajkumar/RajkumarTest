namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Marker interface for bullet boundary handler.
    /// Allows VContainer to distinguish between
    /// ship/asteroid boundary (wrap) and
    /// bullet boundary (destroy).
    /// </summary>
    public interface IBulletBoundaryHandler : IBoundaryHandler { }
}