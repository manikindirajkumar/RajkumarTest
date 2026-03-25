namespace RajkumarTest.Asteroid.Core
{
    public interface IWaveConfig
    {
        int   InitialAsteroidCount { get; }
        float MinSpeed             { get; }
        float MaxSpeed             { get; }

        int   GetAsteroidCountForWave(int wave);
        float GetMaxSpeedForWave(int wave);
        
        int GetScoreForSize(AsteroidSize size);
    }
}