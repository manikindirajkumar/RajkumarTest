using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Deactivates object when it reaches screen edge.
    /// Used by bullets — they disappear at screen edge.
    /// Implements IBulletBoundaryHandler so VContainer
    /// can distinguish from BoundaryHandler (wrap).
    /// </summary>
    public class BoundaryDestroyer : BoundaryBase, IBulletBoundaryHandler
    {
        public override bool ShouldDeactivateAtBoundary => true;

        public BoundaryDestroyer(IBoundaries boundaries)
            : base(boundaries) { }

        public override Vector3 HandleBoundary(Vector3 position)
        {
            // No wrapping — position unchanged
            // Bullet checks IsOutOfBounds and deactivates itself
            return position;
        }
    }
}