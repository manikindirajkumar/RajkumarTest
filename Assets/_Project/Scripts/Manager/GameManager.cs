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
        public event Action OnGameRestart;          

        private readonly IHealthSystem _healthSystem;
        private readonly IScoreSystem  _scoreSystem;
        
        public GameManager(
            IHealthSystem healthSystem,
            IScoreSystem  scoreSystem)
        {
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

            // Fire event — Game Restart
            OnGameRestart?.Invoke();

            CurrentState = GameState.Playing;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}
