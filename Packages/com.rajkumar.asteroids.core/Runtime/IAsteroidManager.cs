using System;

namespace RajkumarTest.Asteroid.Core
{
    public interface IAsteroidManager
    {
        int CurrentWave { get; }

        event Action<int> OnWaveStarted;
        event Action<int> OnWaveCompleted;

        void StartGame();
        void RestartGame();  // ← handles ClearAll internally
    }
}