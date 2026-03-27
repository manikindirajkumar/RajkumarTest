using System;
using NUnit.Framework;
using RajkumarTest.Asteroid.Core;

public class HealthSystemTests
{
    private HealthSystem _healthSystem;

    [SetUp]
    public void SetUp()
    {
        _healthSystem = new HealthSystem(3);
    }

    [Test]
    public void CurrentLives_OnStart_IsInitialLives()
        => Assert.AreEqual(3, _healthSystem.CurrentLives);

    [Test]
    public void IsAlive_WhenLivesAboveZero_IsTrue()
        => Assert.IsTrue(_healthSystem.IsAlive);

    [Test]
    public void LoseLife_DecreasesLivesByOne()
    {
        _healthSystem.LoseLife();
        Assert.AreEqual(2, _healthSystem.CurrentLives);
    }

    [Test]
    public void LoseLife_FiresOnLivesChanged()
    {
        int fired = -1;
        _healthSystem.OnLivesChanged += lives => fired = lives;
        _healthSystem.LoseLife();
        Assert.AreEqual(2, fired);
    }

    [Test]
    public void LoseLife_WhenZeroLives_FiresOnPlayerDied()
    {
        bool died = false;
        _healthSystem.OnPlayerDied += () => died = true;

        _healthSystem.LoseLife();
        _healthSystem.LoseLife();
        _healthSystem.LoseLife();

        Assert.IsTrue(died);
    }

    [Test]
    public void LoseLife_WhenAlreadyDead_DoesNothing()
    {
        _healthSystem.LoseLife();
        _healthSystem.LoseLife();
        _healthSystem.LoseLife();
        _healthSystem.LoseLife(); // extra call

        Assert.AreEqual(0, _healthSystem.CurrentLives);
    }

    [Test]
    public void Reset_RestoresInitialLives()
    {
        _healthSystem.LoseLife();
        _healthSystem.LoseLife();
        _healthSystem.Reset();
        Assert.AreEqual(3, _healthSystem.CurrentLives);
    }

    [Test]
    public void IsAlive_WhenZeroLives_IsFalse()
    {
        _healthSystem.LoseLife();
        _healthSystem.LoseLife();
        _healthSystem.LoseLife();
        Assert.IsFalse(_healthSystem.IsAlive);
    }

    [Test]
    public void Constructor_WithZeroLives_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new HealthSystem(0));
    }
}