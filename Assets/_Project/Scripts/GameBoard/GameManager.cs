using System;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
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
        public event Action OnGameRestart;          // ← new

        private readonly IHealthSystem _healthSystem;
        private readonly IScoreSystem  _scoreSystem;
        

        // No IAsteroidSpawner needed ✅
        public GameManager(
            IHealthSystem healthSystem,
            IScoreSystem  scoreSystem)
        {
            if (healthSystem == null)
                throw new ArgumentNullException(
                    nameof(healthSystem));
            if (scoreSystem == null)
                throw new ArgumentNullException(
                    nameof(scoreSystem));

            _healthSystem = healthSystem;
            _scoreSystem  = scoreSystem;
            
            _healthSystem.OnPlayerDied += TriggerGameOver;
        }

         

        public void StartGame()
        {
            CurrentState = GameState.Playing;
            OnStateChanged?.Invoke(CurrentState);
        }

        public void TriggerGameOver()
        {
            CurrentState = GameState.GameOver;
            OnStateChanged?.Invoke(CurrentState);
        }

        public void RestartGame()
        {
            // Reset systems
            _scoreSystem.Reset();
            _healthSystem.Reset();

            // Fire event — AsteroidManager handles clearing
            OnGameRestart?.Invoke();

            CurrentState = GameState.Playing;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}
