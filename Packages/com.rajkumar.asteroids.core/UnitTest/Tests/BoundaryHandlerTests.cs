using NUnit.Framework;
using UnityEngine;
using RajkumarTest.Asteroid.Core;
using RajkumarTest.Asteroid.Tests;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Tests wrapping behaviour using mocks only.
    /// No real BoundaryHandler, no Camera, no scene needed.
    /// Pure logic testing via IBoundaryHandler contract.
    /// </summary>
    public class BoundaryHandlerTests
    {
        private IBoundaries _boundaries;
        private IBoundaryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _boundaries = new MockGameBoardBoundary(
                minX: -10f, maxX: 10f,
                minY:  -6f, maxY:  6f);

            // Using mock — no real BoundaryHandler needed
            _handler = new MockBoundaryHandler(_boundaries);
        }

         

        [Test]
        public void HandleBoundary_InsideBounds_ReturnsSamePosition()
        {
            var position = new Vector3(0f, 0f, 0f);

            Vector3 result = _handler.HandleBoundary(position);

            Assert.AreEqual(position, result);
        }

         

        [Test]
        public void HandleBoundary_BeyondMaxX_WrapsToMinX()
        {
            var position = new Vector3(11f, 0f, 0f);

            Vector3 result = _handler.HandleBoundary(position);

            Assert.AreEqual(_boundaries.MinX, result.x);
        }

        [Test]
        public void HandleBoundary_BeyondMinX_WrapsToMaxX()
        {
            var position = new Vector3(-11f, 0f, 0f);

            Vector3 result = _handler.HandleBoundary(position);

            Assert.AreEqual(_boundaries.MaxX, result.x);
        }

        

        [Test]
        public void HandleBoundary_BeyondMaxY_WrapsToMinY()
        {
            var position = new Vector3(0f, 7f, 0f);

            Vector3 result = _handler.HandleBoundary(position);

            Assert.AreEqual(_boundaries.MinY, result.y);
        }

        [Test]
        public void HandleBoundary_BeyondMinY_WrapsToMaxY()
        {
            var position = new Vector3(0f, -7f, 0f);

            Vector3 result = _handler.HandleBoundary(position);

            Assert.AreEqual(_boundaries.MaxY, result.y);
        }



        [Test]
        public void HandleBoundary_ZCoordinate_IsNeverChanged()
        {
            var position = new Vector3(11f, 7f, 5f);

            Vector3 result = _handler.HandleBoundary(position);

            Assert.AreEqual(5f, result.z,
                "Z should never be affected by wrapping");
        }

         

        [Test]
        public void HandleBoundary_BeyondCorner_WrapsBothAxes()
        {
            var position = new Vector3(11f, 7f, 0f);

            Vector3 result = _handler.HandleBoundary(position);

            Assert.AreEqual(_boundaries.MinX, result.x,
                "X should wrap to MinX");
            Assert.AreEqual(_boundaries.MinY, result.y,
                "Y should wrap to MinY");
        }

 

        [Test]
        public void HandleBoundary_CalledThreeTimes_CallCountIsThree()
        {
            var mockHandler = new MockBoundaryHandler(_boundaries);

            mockHandler.HandleBoundary(Vector3.zero);
            mockHandler.HandleBoundary(Vector3.zero);
            mockHandler.HandleBoundary(Vector3.zero);

            Assert.AreEqual(3, mockHandler.WrapPositionCallCount,
                "HandleBoundary should have been called exactly 3 times");
        }
    }
}