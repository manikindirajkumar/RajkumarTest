using System;


namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Orchestrates overall game state.
    /// Listens to health system — triggers game over.
    /// Handles restart — resets all systems.
    /// </summary>
    public class GameManager : IGameManager
    {
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;

        private readonly IHealthSystem  _healthSystem;
        private readonly IScoreSystem   _scoreSystem;
        private readonly IWaveManager   _waveManager;
        private readonly IAsteroidSpawner _asteroidSpawner;
        private readonly ISpaceGun _spaceGun;

        public GameManager(
            IHealthSystem healthSystem,
            IScoreSystem scoreSystem,
            IWaveManager waveManager,
            IAsteroidSpawner asteroidSpawner,
            ISpaceGun spaceGun)
        {
            if (healthSystem    == null)
                throw new ArgumentNullException(
                    nameof(healthSystem));
            if (scoreSystem     == null)
                throw new ArgumentNullException(
                    nameof(scoreSystem));
            if (waveManager     == null)
                throw new ArgumentNullException(
                    nameof(waveManager));
            if (asteroidSpawner == null)
                throw new ArgumentNullException(
                    nameof(asteroidSpawner));
            if (spaceGun == null)
                throw new ArgumentNullException(
                    nameof(spaceGun));

            _healthSystem    = healthSystem;
            _scoreSystem     = scoreSystem;
            _waveManager     = waveManager;
            _asteroidSpawner = asteroidSpawner;
            _spaceGun        = spaceGun;

            // Listen to health system
            _healthSystem.OnPlayerDied += TriggerGameOver;
        }

        public void StartGame()
        {
            CurrentState = GameState.Playing;
            OnStateChanged?.Invoke(CurrentState);
            _waveManager.StartWave();
        }

        public void TriggerGameOver()
        {
            CurrentState = GameState.GameOver;
            OnStateChanged?.Invoke(CurrentState);

            // Stop shooting
            _spaceGun?.SetActive(false);
        }

        public void RestartGame()
        {
            // 1. Clear all asteroids
            _asteroidSpawner.ClearAll();

            // 2. Reset systems
            _scoreSystem.Reset();
            _healthSystem.Reset();

            // 3. Re-enable ship
            _spaceGun?.SetActive(true);

            // 4. Update state — before StartWave
            CurrentState = GameState.Playing;
            OnStateChanged?.Invoke(CurrentState);

            // 5. Start fresh wave
            _waveManager.StartWave();
        }
    }
}