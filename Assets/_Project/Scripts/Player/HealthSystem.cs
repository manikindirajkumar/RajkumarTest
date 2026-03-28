using System;

namespace RajkumarTest.Asteroid.Core
{
    // HealthSystem
    /// <summary>
    /// Tracks player lives. Fires OnPlayerDied
    /// when lives reach zero — GameManager listens
    /// and triggers game over state.
    /// </summary>
    public class HealthSystem : IHealthSystem
    {
        public int  CurrentLives { get; private set; }
        public bool IsAlive      => CurrentLives > 0;

        public event Action<int> OnLivesChanged;
        public event Action      OnPlayerDied;

        private readonly int _initialLives;

        public HealthSystem(int initialLives = 3)
        {
            if (initialLives <= 0)
                throw new ArgumentException(
                    "Initial lives must be greater than zero.",
                    nameof(initialLives));

            _initialLives = initialLives;
            CurrentLives  = initialLives;
        }

        public void LoseLife()
        {
            if (!IsAlive) return;

            CurrentLives--;
            OnLivesChanged?.Invoke(CurrentLives);

            if (!IsAlive)
                OnPlayerDied?.Invoke();
        }

        public void Reset()
        {
            CurrentLives = _initialLives;
            OnLivesChanged?.Invoke(CurrentLives);
        }
    }
}