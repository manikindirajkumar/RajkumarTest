using System;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Manages all asteroid behaviour —
    /// wave progression, spawning and clearing.
    /// Subscribes to IGameManager.OnGameRestart
    /// to clear asteroids when game restarts.
    /// Single responsibility — asteroids only.
    /// </summary>
    public class AsteroidManager : IAsteroidManager
    {
        public int CurrentWave { get; private set; }
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;

        private readonly IAsteroidSpawner _asteroidSpawner;
        private readonly IWaveConfig      _waveConfig;
        private readonly IGameManager     _gameManager;

        public AsteroidManager(
            IAsteroidSpawner asteroidSpawner,
            IWaveConfig waveConfig,
            IGameManager gameManager)
        {
            if (asteroidSpawner == null)
                throw new ArgumentNullException(
                    nameof(asteroidSpawner));
            if (waveConfig == null)
                throw new ArgumentNullException(
                    nameof(waveConfig));
            if (gameManager == null)
                throw new ArgumentNullException(
                    nameof(gameManager));

            _asteroidSpawner = asteroidSpawner;
            _waveConfig      = waveConfig;
            _gameManager     = gameManager;

            // Subscribe to spawner — wave completion check
            _asteroidSpawner.OnAsteroidDestroyed
                += CheckWaveComplete;

            // Subscribe to game restart event
            _gameManager.OnGameRestart += RestartGame;
        }

        public void StartGame()
        {
            CurrentWave = 1;
            OnWaveStarted?.Invoke(CurrentWave);
            SpawnCurrentWave();
        }

        public void RestartGame()
        {
            // Clear all active asteroids
            _asteroidSpawner.ClearAll();

            // Reset to wave 1
            CurrentWave = 1;
            OnWaveStarted?.Invoke(CurrentWave);
            SpawnCurrentWave();
        }

        private void CheckWaveComplete()
        {
            if (_asteroidSpawner.ActiveAsteroidCount > 0)
                return;

            OnWaveCompleted?.Invoke(CurrentWave);
            CurrentWave++;
            OnWaveStarted?.Invoke(CurrentWave);
            SpawnCurrentWave();
        }

        private void SpawnCurrentWave()
        {
            int count = _waveConfig
                .GetAsteroidCountForWave(CurrentWave);

            _asteroidSpawner.SpawnWave(count, CurrentWave);
        }
    }
}