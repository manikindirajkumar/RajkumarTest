using System;
using NUnit.Framework;
using UnityEngine;
using RajkumarTest.Asteroid.Core;
using RajkumarTest.Asteroid;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Tests for SpawnPositionProvider.
    /// All tests run in EditMode — no scene or camera needed.
    /// </summary>
    public class SpawnPositionProviderTests
    {
        // ── shared objects ──────────────────────────────────────
        private IBoundaries _boundaries;
        private MockSpawnPositionProvider _provider;

        [SetUp]
        public void SetUp()
        {
            // Fixed boundaries — same every test
            _boundaries = new MockGameBoardBoundary(
                minX: -10f,
                maxX:  10f,
                minY:  -6f,
                maxY:   6f);

            _provider = new MockSpawnPositionProvider(new Vector2(3f, 2f));
        }

        // ── happy path tests ────────────────────────────────────

        [Test]
        public void GetRandomSpawnPosition_XValue_IsWithinBoundaries()
        {
            Vector2 position = _provider.GetRandomSpawnPosition(_boundaries);

            Assert.GreaterOrEqual(position.x, _boundaries.MinX,
                "X should be >= MinX");
            Assert.LessOrEqual(position.x, _boundaries.MaxX,
                "X should be <= MaxX");
        }

        [Test]
        public void GetRandomSpawnPosition_YValue_IsWithinBoundaries()
        {
            Vector2 position = _provider.GetRandomSpawnPosition(_boundaries);

            Assert.GreaterOrEqual(position.y, _boundaries.MinY,
                "Y should be >= MinY");
            Assert.LessOrEqual(position.y, _boundaries.MaxY,
                "Y should be <= MaxY");
        }

        [Test]
        public void GetRandomSpawnPosition_CalledMultipleTimes_AlwaysWithinBoundaries()
        {
            // Run 50 times to verify randomness never goes out of bounds
            for (int i = 0; i < 50; i++)
            {
                Vector2 position = _provider
                    .GetRandomSpawnPosition(_boundaries);

                Assert.GreaterOrEqual(position.x, _boundaries.MinX,
                    $"Run {i}: X below MinX");
                Assert.LessOrEqual(position.x, _boundaries.MaxX,
                    $"Run {i}: X above MaxX");
                Assert.GreaterOrEqual(position.y, _boundaries.MinY,
                    $"Run {i}: Y below MinY");
                Assert.LessOrEqual(position.y, _boundaries.MaxY,
                    $"Run {i}: Y above MaxY");
            }
        }

        // ── edge case tests ─────────────────────────────────────

        [Test]
        public void GetRandomSpawnPosition_WithNullBoundaries_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _provider.GetRandomSpawnPosition(null);
            });
        }

        [Test]
        public void GetRandomSpawnPosition_WithZeroSizeBoundaries_ReturnsSamePoint()
        {
            // Boundaries where min == max — only one valid point
            var zeroBoundaries = new MockGameBoardBoundary(
                minX: 5f, maxX: 5f,
                minY: 3f, maxY: 3f);

            Vector2 position = _provider
                .GetRandomSpawnPosition(zeroBoundaries);

            Assert.AreEqual(5f, position.x,
                "Zero-width boundary should return exact X");
            Assert.AreEqual(3f, position.y,
                "Zero-height boundary should return exact Y");
        }

        // ── mock provider tests ─────────────────────────────────

        [Test]
        public void MockProvider_AlwaysReturnsFixedPosition()
        {
            // Verify that our mock behaves predictably
            var expectedPosition = new Vector2(3f, 2f);
            var mockProvider = new MockSpawnPositionProvider(expectedPosition);

            Vector2 result = mockProvider
                .GetRandomSpawnPosition(_boundaries);

            Assert.AreEqual(expectedPosition, result,
                "Mock should always return the fixed position");
        }

        [Test]
        public void MockProvider_IgnoresBoundaries_ReturnsFixedPosition()
        {
            // Mock should return fixed position regardless of boundaries
            var expectedPosition = new Vector2(-5f, 4f);
            var mockProvider = new MockSpawnPositionProvider(expectedPosition);

            // Pass completely different boundaries
            var differentBoundaries = new MockGameBoardBoundary(
                minX: 0f, maxX: 1f,
                minY: 0f, maxY: 1f);

            Vector2 result = mockProvider
                .GetRandomSpawnPosition(differentBoundaries);

            Assert.AreEqual(expectedPosition, result,
                "Mock ignores boundaries and returns fixed position");
        }
    }
}