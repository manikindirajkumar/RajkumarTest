using NUnit.Framework;
using UnityEngine;

namespace RajkumarTest.Asteroid.Tests.Test
{
    public class BoundaryTests
    {
        
        [Test]
        public void SpawnPosition_IsAlwaysWithinBoundaries()
        {
            var boundaries = new MockGameBoardBoundary(-10f, 10f, -6f, 6f);
            var provider = new MockSpawnPositionProvider(new Vector2(3f, 2f));

            Vector2 position = provider.GetRandomSpawnPosition(boundaries);

            Assert.IsTrue(position.x >= boundaries.MinX);
            Assert.IsTrue(position.x <= boundaries.MaxX);
            Assert.IsTrue(position.y >= boundaries.MinY);
            Assert.IsTrue(position.y <= boundaries.MaxY);
        }
    }
}