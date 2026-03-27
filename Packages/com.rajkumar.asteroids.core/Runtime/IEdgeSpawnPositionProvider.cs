using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Spawns objects just outside screen boundary edges.
    /// Asteroids appear from outside screen and move inward —
    /// gives player time to react before being hit.
    /// Picks a random edge then a random point along that edge.
    /// </summary>
    public interface IEdgeSpawnPositionProvider
    {
        /// <summary>
        /// Returns a random world position within the given boundaries.
        /// </summary>
        Vector2 GetRandomSpawnPosition(IBoundaries boundaries);
    }
}