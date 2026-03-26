using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Wraps position to opposite screen edge.
    /// Used by ship and asteroids.
    /// Extends BoundaryBase — shares IsOutOfBounds logic.
    /// </summary>
    public class BoundaryHandler : BoundaryBase
    {
        public override bool ShouldDeactivateAtBoundary => false;

        public BoundaryHandler(IBoundaries boundaries)
            : base(boundaries) { }

        public override Vector3 HandleBoundary(Vector3 position)
        {
            float x = position.x;
            float y = position.y;

            if (x > Boundaries.MaxX)       x = Boundaries.MinX;
            else if (x < Boundaries.MinX)  x = Boundaries.MaxX;

            if (y > Boundaries.MaxY)       y = Boundaries.MinY;
            else if (y < Boundaries.MinY)  y = Boundaries.MaxY;

            return new Vector3(x, y, position.z);
        }
    }
}