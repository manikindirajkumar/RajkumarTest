using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Abstract base class providing shared boundary
    /// calculation logic for BoundaryHandler and BoundaryDestroyer.
    /// Never instantiated directly.
    /// Avoids code duplication while respecting LSP —
    /// base class makes no promises about HandleBoundary behaviour.
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

        // ── IBoundaryHandler ─────────────────────────────────────

        public abstract bool ShouldDeactivateAtBoundary { get; }

        public abstract Vector3 HandleBoundary(Vector3 position);

        /// <summary>
        /// Shared by both subclasses — no duplication.
        /// </summary>
        public bool IsOutOfBounds(Vector3 position)
        {
            return position.x > Boundaries.MaxX ||
                   position.x < Boundaries.MinX ||
                   position.y > Boundaries.MaxY ||
                   position.y < Boundaries.MinY;
        }
    }
}