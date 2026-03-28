using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Spawns asteroids each wave and tracks active count.
    /// Plain C# — no MonoBehaviour needed.
    /// Fires OnAsteroidDestroyed event so WaveManager
    /// can check wave completion without circular dependency.
    /// </summary>
    public class AsteroidSpawner : IAsteroidSpawner
    {
         

        public int ActiveAsteroidCount { get; private set; }
        public event Action OnAsteroidDestroyed;

         

        private int _currentWaveNumber;
        private Action<IAsteroid> _returnToPool;

        private readonly List<IAsteroid> _activeAsteroids
            = new List<IAsteroid>();

        private readonly Dictionary<AsteroidSize,
            IObjectPool<IAsteroid>> _pools;

        private readonly IEdgeSpawnPositionProvider _spawnProvider;
        private readonly IBoundaries            _boundaries;
        private readonly IWaveConfig            _waveConfig;

         

        public AsteroidSpawner(
            IObjectPool<IAsteroid> largePool,
            IObjectPool<IAsteroid> mediumPool,
            IObjectPool<IAsteroid> smallPool,
            IEdgeSpawnPositionProvider spawnProvider,
            IBoundaries            boundaries,
            IWaveConfig            waveConfig)
        {
            if (largePool     == null)
                throw new ArgumentNullException(nameof(largePool));
            if (mediumPool    == null)
                throw new ArgumentNullException(nameof(mediumPool));
            if (smallPool     == null)
                throw new ArgumentNullException(nameof(smallPool));
            if (spawnProvider == null)
                throw new ArgumentNullException(nameof(spawnProvider));
            if (boundaries    == null)
                throw new ArgumentNullException(nameof(boundaries));
            if (waveConfig    == null)
                throw new ArgumentNullException(nameof(waveConfig));

            _pools = new Dictionary<AsteroidSize,
                IObjectPool<IAsteroid>>
            {
                { AsteroidSize.Large,  largePool  },
                { AsteroidSize.Medium, mediumPool },
                { AsteroidSize.Small,  smallPool  }
            };

            _spawnProvider = spawnProvider;
            _boundaries    = boundaries;
            _waveConfig    = waveConfig;
        }

         

        public void SpawnWave(int count, int waveNumber)
        {
            ActiveAsteroidCount = 0;
            _currentWaveNumber  = waveNumber;

            for (int i = 0; i < count; i++)
            {
                Vector2 spawnPos = _spawnProvider
                    .GetRandomSpawnPosition(_boundaries);

                SpawnAsteroidAt(
                    AsteroidSize.Large,
                    spawnPos,
                    waveNumber);
            }
        }

        public void SpawnSplit(
            AsteroidSize size,
            Vector3 position,
            int waveNumber)
        {
            // Small asteroids don't split
            if (size == AsteroidSize.Small) return;

            AsteroidSize splitSize = size == AsteroidSize.Large
                ? AsteroidSize.Medium
                : AsteroidSize.Small;

            for (int i = 0; i < 2; i++)
                SpawnAsteroidAt(splitSize, position, waveNumber);
        }

        public void ClearAll()
        {
            var toDestroy = new List<IAsteroid>(_activeAsteroids);

            foreach (var asteroid in toDestroy)
            {
                // Unsubscribe first — no scoring or wave check
                asteroid.OnDestroyed -= HandleAsteroidDestroyed;

                // Silent deactivate — no events
                asteroid.DeactivateSilently();

                // Manually return to pool
                _returnToPool?.Invoke(asteroid);
            }

            _activeAsteroids.Clear();
            ActiveAsteroidCount = 0;
        }

        /// <summary>
        /// Set callback for manually returning asteroids to pool.
        /// Called by GameInstaller/GameBootstrapper after pools ready.
        /// </summary>
        public void SetReturnCallback(
            Action<IAsteroid> returnCallback)
        {
            _returnToPool = returnCallback;
        }

         

        private void SpawnAsteroidAt(
            AsteroidSize size,
            Vector3 position,
            int waveNumber)
        {
            IAsteroid asteroid = _pools[size].Get();
            if (asteroid == null)
            {
                return;
            }

            // Wave spawns aim toward center
            // Splits move in random direction
            Vector3 direction = size == AsteroidSize.Large
                ? GetDirectionTowardCenter(position)
                : GetRandomDirection();

            float speed = Random.Range(
                _waveConfig.MinSpeed,
                _waveConfig.GetMaxSpeedForWave(waveNumber));

            asteroid.Activate(position, direction, speed);

            ActiveAsteroidCount++;
            _activeAsteroids.Add(asteroid);
            asteroid.OnDestroyed += HandleAsteroidDestroyed;
        }

        private void HandleAsteroidDestroyed(
            IAsteroid asteroid,
            Vector3 position)
        {
            asteroid.OnDestroyed -= HandleAsteroidDestroyed;
            _activeAsteroids.Remove(asteroid);
            ActiveAsteroidCount--;

            // Split before notifying wave manager
            SpawnSplit(
                asteroid.Size,
                position,
                _currentWaveNumber);

            // Notify WaveManager via event
            OnAsteroidDestroyed?.Invoke();
        }

        private Vector3 GetDirectionTowardCenter(Vector2 spawnPos)
        {
            Vector2 toCenter =
                (Vector2.zero - spawnPos).normalized;

            float angle =
                Random.Range(-45f, 45f) * Mathf.Deg2Rad;

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            return new Vector3(
                toCenter.x * cos - toCenter.y * sin,
                toCenter.x * sin + toCenter.y * cos,
                0f).normalized;
        }

        private Vector3 GetRandomDirection()
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            return new Vector3(
                Mathf.Cos(angle),
                Mathf.Sin(angle),
                0f);
        }
    }
}
