using System;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Core
{
    public interface IGameManager
    {
        GameState CurrentState { get; }
        event Action<GameState> OnStateChanged;
        void StartGame();
        void RestartGame();
        void TriggerGameOver();
    }
}