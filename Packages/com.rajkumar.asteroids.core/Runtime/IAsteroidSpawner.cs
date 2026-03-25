using System;
using UnityEngine;


namespace RajkumarTest.Asteroid.Core
{
    public interface IAsteroidSpawner
    {
        int ActiveAsteroidCount { get; }
        event Action OnAsteroidDestroyed;
        void SpawnWave(int count, int waveNumber);

        // New — spawns split asteroids at position
        void SpawnSplit(
            AsteroidSize size,
            Vector3 position,
            int waveNumber);
        // IAsteroidSpawner
        void ClearAll();
    }

}