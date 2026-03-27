using System;

namespace RajkumarTest.Asteroid.Core
{
    public interface IGameManager
    {
        GameState CurrentState { get; }

        event Action<GameState> OnStateChanged;

        /// <summary>
        /// Fired when player clicks restart.
        /// AsteroidManager subscribes to clear asteroids.
        /// </summary>
        event Action OnGameRestart;

        void StartGame();
        void TriggerGameOver();
        void RestartGame();
    }
}