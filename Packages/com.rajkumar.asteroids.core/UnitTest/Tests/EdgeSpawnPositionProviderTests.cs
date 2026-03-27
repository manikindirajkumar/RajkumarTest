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
    public class EdgeSpawnPositionProviderTests
    {
        // ── shared objects ──────────────────────────────────────
        private IBoundaries _boundaries;
        private EdgeSpawnPositionProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _provider = new EdgeSpawnPositionProvider();

            // ← add this
            _boundaries = new MockGameBoardBoundary(
                minX: -10f,
                maxX:  10f,
                minY:  -6f,
                maxY:   6f);
        }


        // ── happy path tests ────────────────────────────────────

         
         
        [Test]
        public void GetRandomSpawnPosition_WithZeroSizeBoundaries_IsNearBoundary()
        {
            var zeroBoundaries = new MockGameBoardBoundary(
                minX: 3f, maxX: 3f,
                minY: 5f, maxY: 5f);

            Vector2 position = _provider
                .GetRandomSpawnPosition(zeroBoundaries);

            // Position should be exactly SpawnOffset away from boundary
            // on one of the four edges
            bool isOnEdge =
                Mathf.Approximately(position.x, 3f + 1f) || // Right
                Mathf.Approximately(position.x, 3f - 1f) || // Left
                Mathf.Approximately(position.y, 5f + 1f) || // Top
                Mathf.Approximately(position.y, 5f - 1f);   // Bottom

            Assert.IsTrue(isOnEdge,
                $"Expected position on edge but got {position}");
        }
        [Test]
        public void GetRandomSpawnPosition_CalledMultipleTimes_AlwaysOutsideBoundaries()
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 position = _provider
                    .GetRandomSpawnPosition(_boundaries);

                bool isOutside =
                    position.x < _boundaries.MinX ||
                    position.x > _boundaries.MaxX ||
                    position.y < _boundaries.MinY ||
                    position.y > _boundaries.MaxY;

                Assert.IsTrue(isOutside,
                    $"Run {i}: position {position} should be outside boundaries");
            }
        }

        // ── edge case tests ─────────────────────────────────────

        [Test]
        public void GetRandomSpawnPosition_WithNullBoundaries_ThrowsArgumentNullException()
        {
            // Test the REAL implementation — not the mock
            var provider = new EdgeSpawnPositionProvider();

            Assert.Throws<ArgumentNullException>(() =>
                provider.GetRandomSpawnPosition(null));
        }

        [Test]
        public void GetRandomSpawnPosition_IsOutsideBoundaries()
        {
            // Position must be outside the play area
            Vector2 position = _provider
                .GetRandomSpawnPosition(_boundaries);

            bool isOutside =
                position.x < _boundaries.MinX ||
                position.x > _boundaries.MaxX ||
                position.y < _boundaries.MinY ||
                position.y > _boundaries.MaxY;

            Assert.IsTrue(isOutside,
                $"Spawn position {position} should be outside boundaries");
        }

        [Test]
        public void GetRandomSpawnPosition_IsExactlyOneUnitOutsideBoundary()
        {
            // Run multiple times to hit different edges
            for (int i = 0; i < 20; i++)
            {
                Vector2 position = _provider
                    .GetRandomSpawnPosition(_boundaries);

                bool isExactlyOffset =
                    Mathf.Approximately(position.x, _boundaries.MinX - 1f) ||
                    Mathf.Approximately(position.x, _boundaries.MaxX + 1f) ||
                    Mathf.Approximately(position.y, _boundaries.MinY - 1f) ||
                    Mathf.Approximately(position.y, _boundaries.MaxY + 1f);

                Assert.IsTrue(isExactlyOffset,
                    $"Position {position} is not exactly 1 unit outside boundary");
            }
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