using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests
{
    /// <summary>
    /// Test double for ISpaceGun.
    /// Tracks fire calls without needing
    /// a real bullet pool or prefabs.
    /// </summary>
    public class MockSpaceGun : ISpaceGun
    {
        public bool CanFire { get; set; } = true;
        public int FireCallCount { get; private set; }
        public Vector3 LastFirePosition { get; private set; }
        public Vector3 LastFireDirection { get; private set; }

        public void TryFire(Vector3 position, Vector3 direction)
        {
            if (!CanFire) return;

            FireCallCount++;
            LastFirePosition  = position;
            LastFireDirection = direction;
        }

        public void SetActive(bool active)
        {
            
        }
    }
}