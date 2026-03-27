using System;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Test double for IInputProvider.
    /// Lets tests fire specific input events manually
    /// without needing a keyboard or Unity Input System.
    /// </summary>
    public class MockInputProvider : IInputProvider
    {
        public event Action OnShoot;
        public event Action OnThrustStarted;
        public event Action OnThrustStopped;
        public event Action OnRotateLeft;
        public event Action OnRotateRight;
        public event Action OnRotateStopped;
        public event Action OnRestartGame;

        // Helper methods — tests call these to simulate input
        public void FireShoot()          => OnShoot?.Invoke();
        public void FireThrustStarted()  => OnThrustStarted?.Invoke();
        public void FireThrustStopped()  => OnThrustStopped?.Invoke();
        public void FireRotateLeft()     => OnRotateLeft?.Invoke();
        public void FireRotateRight()    => OnRotateRight?.Invoke();
        public void FireRotateStopped()  => OnRotateStopped?.Invoke();
        public void FireRestartGame()    => OnRestartGame?.Invoke();
    }
}