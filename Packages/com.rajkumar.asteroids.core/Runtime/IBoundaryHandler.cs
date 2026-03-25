using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    public interface IBoundaryHandler
    {
        bool ShouldDeactivateAtBoundary { get; }

        /// <summary>
        /// Process position — wrap or return unchanged.
        /// </summary>
        Vector3 HandleBoundary(Vector3 position);

        /// <summary>
        /// True if position is outside screen bounds.
        /// </summary>
        bool IsOutOfBounds(Vector3 position);
    }
}