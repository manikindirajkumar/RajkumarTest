using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Fixed wave config for tests.
    /// No ScriptableObject needed.
    /// </summary>
    public class MockWaveConfig : IWaveConfig
    {
        public int InitialAsteroidCount { get; set; } = 4;
        public float MinSpeed           { get; set; } = 1f;
        public float MaxSpeed           { get; set; } = 3f;

        private int[] _scoresForSizes = new[] { 50, 100, 150 };
        public int GetAsteroidCountForWave(int wave)
            => InitialAsteroidCount + (wave - 1) * 2;

        public float GetMaxSpeedForWave(int wave)
            => MaxSpeed + (wave - 1) * 0.2f;

        public int GetScoreForSize(AsteroidSize size)
        {
          return _scoresForSizes[(int)size];
        }
    }
}