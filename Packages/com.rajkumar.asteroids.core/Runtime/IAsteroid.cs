using System;
using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Represents a single asteroid in the game.
    /// Handles its own movement and lifecycle.
    /// Notifies via OnDestroyed when it is hit
    /// so spawner and score system can react.
    /// </summary>
    public interface IAsteroid
    {
        /// <summary>
        /// True if asteroid is active in scene.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Size of this asteroid.
        /// Determines score value and split behaviour.
        /// </summary>
        AsteroidSize Size { get; }

        /// <summary>
        /// Fired when asteroid is destroyed.
        /// Passes self so spawner knows which
        /// asteroid was destroyed and its size.
        /// </summary>
        event Action<IAsteroid, Vector3> OnDestroyed;

        /// <summary>
        /// Activate asteroid at position
        /// moving in given direction at given speed.
        /// </summary>
        void Activate(
            Vector3 position,
            Vector3 direction,
            float speed);

        /// <summary>
        /// Destroy this asteroid —
        /// fires OnDestroyed and returns to pool.
        /// </summary>
        void Destroy();
        // IAsteroid
        event Action<IAsteroid> OnReturnToPool;

        void DeactivateSilently();
    }

    /// <summary>
    /// Asteroid size affects score and splitting.
    /// Large  → splits into 2 Medium
    /// Medium → splits into 2 Small
    /// Small  → destroyed, no split
    /// </summary>
    public enum AsteroidSize
    {
        Large  = 0,
        Medium = 1,
        Small  = 2
    }
}