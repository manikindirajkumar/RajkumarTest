using System;
using NUnit.Framework;
using System.Collections.Generic;
using RajkumarTest.Asteroid.Core;
using RajkumarTest.Asteroid.Tests.Mocks;

namespace RajkumarTest.Asteroid.Tests
{
    public class ObjectPoolTests
    {
        private ObjectPool<MockBullet> CreatePoolWithItems(
            params MockBullet[] items)
        {
            return new ObjectPool<MockBullet>(items);
        }

        [Test]
        public void Get_ReturnsItemFromPool()
        {
            var pool = CreatePoolWithItems(
                new MockBullet(),
                new MockBullet());

            var result = pool.Get();

            Assert.IsNotNull(result);
        }

        [Test]
        public void Get_WhenPoolEmpty_ReturnsNull()
        {
            var pool = CreatePoolWithItems(new MockBullet());

            pool.Get(); // take the only item

            var result = pool.Get();

            Assert.IsNull(result);
        }

        [Test]
        public void Return_AddsItemBackToPool()
        {
            var bullet = new MockBullet();
            var pool   = CreatePoolWithItems(bullet);

            pool.Get();           // remove it
            pool.Return(bullet);  // put it back

            Assert.AreEqual(1, pool.AvailableCount);
        }

        [Test]
        public void AvailableCount_DecreasesOnGet()
        {
            var pool = CreatePoolWithItems(
                new MockBullet(),
                new MockBullet(),
                new MockBullet());

            Assert.AreEqual(3, pool.AvailableCount);

            pool.Get();
            Assert.AreEqual(2, pool.AvailableCount);

            pool.Get();
            Assert.AreEqual(1, pool.AvailableCount);
        }

        [Test]
        public void AvailableCount_IncreasesOnReturn()
        {
            var bullet = new MockBullet();
            var pool   = CreatePoolWithItems(bullet);

            pool.Get();
            Assert.AreEqual(0, pool.AvailableCount);

            pool.Return(bullet);
            Assert.AreEqual(1, pool.AvailableCount);
        }

        [Test]
        public void Constructor_WithNullItems_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ObjectPool<MockBullet>(null));
        }
    }
}