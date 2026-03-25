using NUnit.Framework;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    public class ScoreSystemTests
    {
        private ScoreSystem _scoreSystem;

        [SetUp]
        public void SetUp() =>
            _scoreSystem = new ScoreSystem();

        [Test]
        public void Score_StartsAtZero() =>
            Assert.AreEqual(0, _scoreSystem.CurrentScore);

        [Test]
        public void AddScore_IncreasesScore()
        {
            _scoreSystem.AddScore(100);
            Assert.AreEqual(100, _scoreSystem.CurrentScore);
        }

        [Test]
        public void AddScore_NegativeValue_DoesNothing()
        {
            _scoreSystem.AddScore(-50);
            Assert.AreEqual(0, _scoreSystem.CurrentScore);
        }

        [Test]
        public void AddScore_FiresOnScoreChanged()
        {
            int fired = -1;
            _scoreSystem.OnScoreChanged += s => fired = s;
            _scoreSystem.AddScore(100);
            Assert.AreEqual(100, fired);
        }

        [Test]
        public void Reset_SetsScoreToZero()
        {
            _scoreSystem.AddScore(500);
            _scoreSystem.Reset();
            Assert.AreEqual(0, _scoreSystem.CurrentScore);
        }
    }
}