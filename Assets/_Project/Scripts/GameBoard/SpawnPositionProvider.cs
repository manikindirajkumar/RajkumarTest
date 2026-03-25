using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;
using Random = UnityEngine.Random;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Generates random spawn positions within screen boundaries
    /// using UnityEngine.Random.
    /// 
    /// In unit tests, replace this with MockSpawnPositionProvider
    /// to get fixed, deterministic positions instead.
    /// </summary>
    public class SpawnPositionProvider : ISpawnPositionProvider
    {
        /// <summary>
        /// Returns a random Vector2 within the given screen boundaries.
        /// Coordinates are in world space.
        /// </summary>
        public Vector2 GetRandomSpawnPosition(IBoundaries boundaries)
        {
            if (boundaries == null)
                throw new ArgumentNullException(
                    nameof(boundaries),
                    "Boundaries cannot be null.");

            float x = Random.Range(boundaries.MinX, boundaries.MaxX);
            float y = Random.Range(boundaries.MinY, boundaries.MaxY);

            return new Vector2(x, y);
        }
    }
}