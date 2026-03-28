using NUnit.Framework;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    public class InputManagerTests
    {
        private InputManager _inputManager;
        private MockInputProvider _mockInput;

        [SetUp]
        public void SetUp()
        {
            _inputManager = new InputManager();
            _mockInput    = new MockInputProvider();
            _inputManager.Subscribe(_mockInput);
        }

        [TearDown]
        public void TearDown()
        {
            _inputManager.Unsubscribe();
        }

         

        [Test]
        public void IsThrusting_WhenThrustStartedFired_BecomesTrue()
        {
            _mockInput.FireThrustStarted();
            Assert.IsTrue(_inputManager.IsThrusting);
        }

        [Test]
        public void IsThrusting_WhenThrustStoppedFired_BecomesFalse()
        {
            _mockInput.FireThrustStarted();
            _mockInput.FireThrustStopped();
            Assert.IsFalse(_inputManager.IsThrusting);
        }

         

        [Test]
        public void TurnDirection_WhenRotateLeftFired_IsNegativeOne()
        {
            _mockInput.FireRotateLeft();
            Assert.AreEqual(-1f, _inputManager.TurnDirection);
        }

        [Test]
        public void TurnDirection_WhenRotateRightFired_IsPositiveOne()
        {
            _mockInput.FireRotateRight();
            Assert.AreEqual(1f, _inputManager.TurnDirection);
        }

        [Test]
        public void TurnDirection_WhenRotateStoppedFired_IsZero()
        {
            _mockInput.FireRotateLeft();
            _mockInput.FireRotateStopped();
            Assert.AreEqual(0f, _inputManager.TurnDirection);
        }

        

        [Test]
        public void Reset_AfterInput_ClearsAllState()
        {
            _mockInput.FireThrustStarted();
            _mockInput.FireRotateRight();

            _inputManager.Reset();

            Assert.IsFalse(_inputManager.IsThrusting);
            Assert.AreEqual(0f, _inputManager.TurnDirection);
        }

        

        [Test]
        public void Unsubscribe_AfterUnsubscribe_InputEventsNoLongerAffectState()
        {
            _inputManager.Unsubscribe();

            // Fire events after unsubscribing
            _mockInput.FireThrustStarted();
            _mockInput.FireRotateRight();

            // State should not have changed
            Assert.IsFalse(_inputManager.IsThrusting,
                "IsThrusting should not change after Unsubscribe");
            Assert.AreEqual(0f, _inputManager.TurnDirection,
                "TurnDirection should not change after Unsubscribe");
        }

         

        [Test]
        public void Subscribe_WithNullProvider_ThrowsArgumentNullException()
        {
            var freshManager = new InputManager();
            Assert.Throws<System.ArgumentNullException>(() =>
            {
                freshManager.Subscribe(null);
            });
        }
    }
}