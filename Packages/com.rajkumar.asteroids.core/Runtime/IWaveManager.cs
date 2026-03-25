using System;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Manages wave progression.
    /// Tracks current wave, asteroid count,
    /// and triggers next wave when all cleared.
    /// </summary>
    public interface IWaveManager
    {
        int CurrentWave { get; }
        event Action<int> OnWaveCompleted;
        event Action<int> OnWaveStarted;
        void StartWave();
        // OnAsteroidDestroyed removed — 
        // WaveManager subscribes to spawner event instead
    }
}