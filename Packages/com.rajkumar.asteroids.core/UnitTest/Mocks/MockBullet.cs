using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Tests.Mocks
{
    public class MockBullet : IBullet
    {
        public bool    IsActive    { get; private set; }
        public event Action<IBullet> OnDeactivated;
        public float   DamageValue => 1f;

        public bool LaunchCalled      { get; private set; }
        public bool DeactivateCalled  { get; private set; }

        public void Launch(Vector3 position, Vector3 direction)
        {
            IsActive     = true;
            LaunchCalled = true;
        }

        public void Deactivate()
        {
            IsActive        = false;
            DeactivateCalled = true;
        }
    }
}