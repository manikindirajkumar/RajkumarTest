using System;
using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Spawns and tracks asteroids.
    /// Fires OnAsteroidDestroyed so WaveManager
    /// can react without a direct reference back —
    /// breaks circular dependency.
    /// </summary>
    public interface IAsteroidSpawner
    {
        /// <summary>
        /// Current number of active asteroids.
        /// Wave ends when this reaches zero.
        /// </summary>
        int ActiveAsteroidCount { get; }

        /// <summary>
        /// Fired every time any asteroid is destroyed.
        /// WaveManager subscribes to check wave complete.
        /// </summary>
        event Action OnAsteroidDestroyed;

        /// <summary>
        /// Spawn initial asteroids for a new wave.
        /// waveNumber used to scale asteroid speed.
        /// </summary>
        void SpawnWave(int count, int waveNumber);

        /// <summary>
        /// Spawn split asteroids at position.
        /// Large → 2 Medium, Medium → 2 Small, Small → nothing.
        /// </summary>
        void SpawnSplit(
            AsteroidSize size,
            Vector3 position,
            int waveNumber);

        /// <summary>
        /// Deactivate all active asteroids silently.
        /// Used on game restart — no scoring or wave events.
        /// </summary>
        void ClearAll();
    }
}