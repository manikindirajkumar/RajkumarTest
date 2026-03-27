using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Test double for ISpawnPositionProvider.
    /// Always returns a fixed, predictable position —
    /// completely ignores boundaries and Random.
    /// Makes spawn position tests fully deterministic.
    /// </summary>
    public class MockSpawnPositionProvider : IEdgeSpawnPositionProvider
    {
        private readonly Vector2 _fixedPosition;

        // Track how many times this was called
        // Useful for verifying spawner calls it correctly
        public int CallCount { get; private set; }

        public MockSpawnPositionProvider(Vector2 fixedPosition)
        {
            _fixedPosition = fixedPosition;
        }

        public Vector2 GetRandomSpawnPosition(IBoundaries boundaries)
        {
            // No null check — mocks don't validate input
            // They just return predictable data
            CallCount++;
            return _fixedPosition;
        }
    }
}