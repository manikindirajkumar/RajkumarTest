using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Signals object should deactivate at screen edge.
    /// Used by bullets.
    /// </summary>
    public class BoundaryDestroyer : BoundaryBase
    {
        public override bool ShouldDeactivateAtBoundary => true;

        public BoundaryDestroyer(IBoundaries boundaries)
            : base(boundaries) { }

        public override Vector3 HandleBoundary(Vector3 position)
        {
            // No wrapping — position unchanged
            // Bullet.cs checks IsOutOfBounds and deactivates
            return position;
        }
    }
}