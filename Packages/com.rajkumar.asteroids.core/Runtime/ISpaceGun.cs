using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Manages ship weapon system.
    /// Handles fire rate, cooldown,
    /// and bullet launching.
    /// Ship calls TryFire() —
    /// gun decides if it can shoot.
    /// </summary>
    public interface ISpaceGun
    {
        /// <summary>
        /// True if cooldown complete and ready to fire.
        /// </summary>
        bool CanFire { get; }

        /// <summary>
        /// Attempt to fire from position in direction.
        /// Does nothing if CanFire is false.
        /// </summary>
        /// <param name="position">Muzzle world position</param>
        /// <param name="direction">Normalised fire direction</param>
        void TryFire(Vector3 position, Vector3 direction);
        
        void SetActive(bool active);
    }
}