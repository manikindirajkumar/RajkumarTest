using System;
using RajkumarTest.Asteroid.Core;
using UnityEngine;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Stub implementation of IInputProvider for touch input.
    /// Interface contract fulfilled — touch mapping
    /// planned for future implementation.
    /// </summary>
    public class TouchInputProvider : MonoBehaviour, IInputProvider
    {
        public event Action OnShoot;
        public event Action OnThrustStarted;
        public event Action OnThrustStopped;
        public event Action OnRotateLeft;
        public event Action OnRotateRight;
        public event Action OnRotateStopped;
        public event Action OnRestartGame;
    }
}
