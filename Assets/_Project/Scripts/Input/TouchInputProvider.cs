using System;
using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
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
