using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Test double for IBoundaryHandler.
    /// Returns predictable wrapped positions
    /// without needing real screen boundaries.
    /// Lets us test wrap behaviour in complete isolation.
    /// </summary>
    public class MockBoundaryHandler : IBoundaryHandler
    {
        private readonly IBoundaries _boundaries;

        // Track how many times WrapPosition was called
        // Useful for verifying ShipMovement calls it every frame
        public int WrapPositionCallCount { get; private set; }

        public MockBoundaryHandler(IBoundaries boundaries)
        {
            _boundaries = boundaries;
        }

        public bool ShouldDeactivateAtBoundary => false;

        public Vector3 HandleBoundary(Vector3 position)
        {
            WrapPositionCallCount++;

            float x = position.x;
            float y = position.y;

            if (x > _boundaries.MaxX)      x = _boundaries.MinX;
            else if (x < _boundaries.MinX) x = _boundaries.MaxX;

            if (y > _boundaries.MaxY)      y = _boundaries.MinY;
            else if (y < _boundaries.MinY) y = _boundaries.MaxY;

            return new Vector3(x, y, position.z);
        }

        public bool IsOutOfBounds(Vector3 position)
        {
            return position.x > _boundaries.MaxX ||
                   position.x < _boundaries.MinX ||
                   position.y > _boundaries.MaxY ||
                   position.y < _boundaries.MinY;
        }
    }
}