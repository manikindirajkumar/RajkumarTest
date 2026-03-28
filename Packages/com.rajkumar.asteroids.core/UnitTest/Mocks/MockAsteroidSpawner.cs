using System;
using System.Numerics;
using RajkumarTest.Asteroid.Core;

public class MockAsteroidSpawner : IAsteroidSpawner
{
    public int  ActiveAsteroidCount { get; set; }
    public int  SpawnWaveCallCount  { get; private set; }
    public int  LastSpawnedCount    { get; private set; }
    public bool ClearAllCalled      { get; private set; }   

    public event Action OnAsteroidDestroyed;

    public void SpawnWave(int count, int waveNumber)
    {
        SpawnWaveCallCount++;
        LastSpawnedCount    = count;
        ActiveAsteroidCount = count;
    }

    public void SpawnSplit(AsteroidSize size, UnityEngine.Vector3 position, int waveNumber)
    {
      
    }

    public void SpawnSplit(
        AsteroidSize size,
        Vector3 position,
        int waveNumber) { }

    public void ClearAll()
    {
        ClearAllCalled      = true;  // ← add
        ActiveAsteroidCount = 0;
    }

    // Helper — tests call this to simulate asteroid destroyed
    public void FireOnAsteroidDestroyed()  // ← add
        => OnAsteroidDestroyed?.Invoke();
}