using System;
using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Represents a single projectile.
    /// Notifies via OnDeactivated when it
    /// wants to return to pool —
    /// without knowing who owns the pool.
    /// </summary>
    public interface IBullet
    {
        /// <summary>
        /// True if bullet is active in scene.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Fired when bullet deactivates itself.
        /// BulletPool subscribes to this to
        /// know when to reclaim the bullet.
        /// </summary>
        event Action<IBullet> OnDeactivated;

        /// <summary>
        /// Launch from position in direction.
        /// </summary>
        void Launch(Vector3 position, Vector3 direction);

        /// <summary>
        /// Deactivate and signal pool to reclaim.
        /// </summary>
        void Deactivate();
    }

    public enum BulletType
    {
        Bullet1,
        Bullet2,
        Bullet3
    }
}