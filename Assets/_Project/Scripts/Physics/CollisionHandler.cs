using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Detects bullet-asteroid collisions and fires event.
    /// Does NOT handle scoring, destruction or any game logic.
    /// Other systems subscribe to OnCollision and react
    /// independently — Open/Closed principle.
    /// </summary>
    public class CollisionHandler : MonoBehaviour
    {
        /// <summary>
        /// Fired when bullet hits this asteroid.
        /// Passes asteroid and bullet so subscribers
        /// have full context.
        /// </summary>
        public event Action<IAsteroid, IBullet> OnCollision;

        private IAsteroid _asteroid;

        public void Initialise(IAsteroid asteroid)
        {
            if (asteroid == null)
                throw new ArgumentNullException(
                    nameof(asteroid));

            _asteroid = asteroid;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            IBullet bullet =
                other.GetComponent<IBullet>();

            if (bullet == null) return;

            // Just report — don't decide what happens
            OnCollision?.Invoke(_asteroid, bullet);
        }
    }
}