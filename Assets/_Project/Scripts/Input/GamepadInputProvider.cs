using System;
using RajkumarTest.Asteroid.Core;
using UnityEngine;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Stub implementation of IInputProvider for gamepad input.
    /// Interface contract fulfilled — input mapping
    /// planned for future implementation.
    /// </summary>
    public class GamepadInputProvider : MonoBehaviour, IInputProvider
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
