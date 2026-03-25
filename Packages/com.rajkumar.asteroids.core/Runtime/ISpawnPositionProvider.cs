using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Provides random spawn positions within screen boundaries.
    /// Abstracted so tests can inject fixed, known positions
    /// instead of relying on UnityEngine.Random.
    /// </summary>
    public interface ISpawnPositionProvider
    {
        /// <summary>
        /// Returns a random world position within the given boundaries.
        /// </summary>
        Vector2 GetRandomSpawnPosition(IBoundaries boundaries);
    }
}