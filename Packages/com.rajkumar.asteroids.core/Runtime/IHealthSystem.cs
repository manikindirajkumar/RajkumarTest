using System;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Tracks player lives separately from score.
    /// Single responsibility — lives only.
    /// </summary>
    public interface IHealthSystem
    {
        int CurrentLives { get; }
        bool IsAlive { get; }

        event Action<int> OnLivesChanged;
        event Action OnPlayerDied;

        void LoseLife();
        void Reset();
    }
}