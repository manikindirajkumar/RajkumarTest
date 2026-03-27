using NUnit.Framework;
using RajkumarTest.Asteroid.Core;
using RajkumarTest.Asteroid.Tests;

public class AsteroidManagerTests
{
    private AsteroidManager    _asteroidManager;
    private MockAsteroidSpawner _mockSpawner;
    private MockWaveConfig      _mockConfig;
    private MockGameManager     _mockGameManager;

    [SetUp]
    public void SetUp()
    {
        _mockSpawner     = new MockAsteroidSpawner();
        _mockConfig      = new MockWaveConfig
        {
            InitialAsteroidCount = 4
        };
        _mockGameManager = new MockGameManager();

        _asteroidManager = new AsteroidManager(
            _mockSpawner,
            _mockConfig,
            _mockGameManager);
    }

    [Test]
    public void StartGame_SetsCurrentWaveToOne()
    {
        _asteroidManager.StartGame();
        Assert.AreEqual(1, _asteroidManager.CurrentWave);
    }

    [Test]
    public void StartGame_CallsSpawnWave()
    {
        _asteroidManager.StartGame();
        Assert.AreEqual(1, _mockSpawner.SpawnWaveCallCount);
    }

    [Test]
    public void StartGame_FiresOnWaveStarted()
    {
        int firedWave = -1;
        _asteroidManager.OnWaveStarted += w => firedWave = w;
        _asteroidManager.StartGame();
        Assert.AreEqual(1, firedWave);
    }

    [Test]
    public void OnAsteroidDestroyed_WhenAsteroidsRemain_DoesNotAdvanceWave()
    {
        _asteroidManager.StartGame();
        _mockSpawner.ActiveAsteroidCount = 3;
        _mockSpawner.FireOnAsteroidDestroyed();
        Assert.AreEqual(1, _asteroidManager.CurrentWave);
    }

    [Test]
    public void OnAsteroidDestroyed_WhenAllCleared_AdvancesWave()
    {
        _asteroidManager.StartGame();
        _mockSpawner.ActiveAsteroidCount = 0;
        _mockSpawner.FireOnAsteroidDestroyed();
        Assert.AreEqual(2, _asteroidManager.CurrentWave);
    }

    [Test]
    public void OnAsteroidDestroyed_WhenAllCleared_FiresOnWaveCompleted()
    {
        int completedWave = -1;
        _asteroidManager.OnWaveCompleted +=
            w => completedWave = w;

        _asteroidManager.StartGame();
        _mockSpawner.ActiveAsteroidCount = 0;
        _mockSpawner.FireOnAsteroidDestroyed();

        Assert.AreEqual(1, completedWave);
    }

    [Test]
    public void RestartGame_ResetsToWaveOne()
    {
        _asteroidManager.StartGame();
        _mockSpawner.ActiveAsteroidCount = 0;
        _mockSpawner.FireOnAsteroidDestroyed(); // advance to wave 2

        _asteroidManager.RestartGame();

        Assert.AreEqual(1, _asteroidManager.CurrentWave);
    }

    [Test]
    public void RestartGame_CallsClearAll()
    {
        _asteroidManager.StartGame();
        _asteroidManager.RestartGame();
        Assert.IsTrue(_mockSpawner.ClearAllCalled);
    }
}