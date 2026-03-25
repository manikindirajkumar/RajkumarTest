using System;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Pure C# wave progression manager.
    /// No MonoBehaviour — fully testable.
    /// Created and owned by GameInstaller.
    /// </summary>
    public class WaveManager : IWaveManager
    {
        // ── IWaveManager ─────────────────────────────────────────

        public int CurrentWave { get; private set; }
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveCompleted;

        // ── private fields ───────────────────────────────────────

        private readonly IAsteroidSpawner _asteroidSpawner;
        private readonly IWaveConfig _waveConfig;

        // ── constructor ──────────────────────────────────────────
        public WaveManager(
            IAsteroidSpawner asteroidSpawner,
            IWaveConfig waveConfig)
        {
            _asteroidSpawner = asteroidSpawner;
            _waveConfig      = waveConfig;

            // Subscribe to spawner event
            _asteroidSpawner.OnAsteroidDestroyed
                += CheckWaveComplete;
        }

        private void CheckWaveComplete()
        {
            if (_asteroidSpawner.ActiveAsteroidCount > 0)
                return;

            OnWaveCompleted?.Invoke(CurrentWave);
            CurrentWave++;
            SpawnCurrentWave();
        }
        

        // ── IWaveManager ─────────────────────────────────────────

        public void StartWave()
        {
            // Reset to wave 1 on restart
            CurrentWave = 1;
            SpawnCurrentWave();
            OnWaveStarted?.Invoke(CurrentWave);
        }

        public void OnAsteroidDestroyed()
        {
            if (_asteroidSpawner.ActiveAsteroidCount > 0)
                return;

            OnWaveCompleted?.Invoke(CurrentWave);
            CurrentWave++;
            SpawnCurrentWave();
        }

        // ── private ──────────────────────────────────────────────

        private void SpawnCurrentWave()
        {
            int count = _waveConfig
                .GetAsteroidCountForWave(CurrentWave);

            _asteroidSpawner.SpawnWave(count, CurrentWave);
        }
        
         
    }
}