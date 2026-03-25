using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Base class providing shared boundary
    /// calculation logic only.
    /// Never instantiated directly —
    /// use BoundaryHandler or BoundaryDestroyer.
    /// </summary>
    public abstract class BoundaryBase : IBoundaryHandler
    {
        protected readonly IBoundaries Boundaries;

        protected BoundaryBase(IBoundaries boundaries)
        {
            if (boundaries == null)
                throw new System.ArgumentNullException(
                    nameof(boundaries));

            Boundaries = boundaries;
        }

        // Shared by both subclasses — no duplication
        public bool IsOutOfBounds(Vector3 position)
        {
            return position.x > Boundaries.MaxX ||
                   position.x < Boundaries.MinX ||
                   position.y > Boundaries.MaxY ||
                   position.y < Boundaries.MinY;
        }

        // Each subclass defines its own behaviour
        public abstract bool ShouldDeactivateAtBoundary { get; }
        public abstract Vector3 HandleBoundary(Vector3 position);
    }
}