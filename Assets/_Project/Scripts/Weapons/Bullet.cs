using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Projectile fired by the ship.
    /// Notifies BulletPool via OnDeactivated
    /// when it needs to be returned —
    /// no direct reference to pool needed.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour, IBullet
    {
        // ── config ───────────────────────────────────────────────

        [SerializeField]
        private float _speed = 10f;

        [SerializeField]
        private float _lifetime = 2f;

        // ── IBullet ──────────────────────────────────────────────

        public bool IsActive => gameObject.activeSelf;

        /// <summary>
        /// BulletPool subscribes to this.
        /// Fires when bullet deactivates —
        /// tells pool to reclaim this bullet.
        /// </summary>
        public event Action<IBullet> OnDeactivated;

        // ── private fields ───────────────────────────────────────

        private Rigidbody2D _rigidbody;
        private IBoundaryHandler _boundaryHandler;
        private float _lifetimeTimer;

        // ── initialisation ───────────────────────────────────────

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _rigidbody.gravityScale = 0f;
        }

        public void Initialise(IBoundaryHandler boundaryHandler)
        {
            if (boundaryHandler == null)
                throw new System.ArgumentNullException(
                    nameof(boundaryHandler));

            _boundaryHandler = boundaryHandler;
        }

        // ── IBullet implementation ───────────────────────────────

        public void Launch(Vector3 position, Vector3 direction)
        {
            transform.position     = position;
            gameObject.SetActive(true);
            _lifetimeTimer         = _lifetime;
            _rigidbody.linearVelocity = direction.normalized * _speed;
        }

        public void Deactivate()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);

            // Notify pool — "I am done, come get me"
            // Pool listens to this and calls Return(this)
            OnDeactivated?.Invoke(this);
        }

        // ── Unity lifecycle ──────────────────────────────────────

        private void Update()
        {
            if (!IsActive) return;

            _lifetimeTimer -= Time.deltaTime;
            if (_lifetimeTimer <= 0f)
            {
                Deactivate();
                return;
            }

            if (_boundaryHandler == null) return;

            if (_boundaryHandler.ShouldDeactivateAtBoundary)
            {
                // Bullet — deactivate when leaving screen
                if (_boundaryHandler.IsOutOfBounds(transform.position))
                    Deactivate();
            }
            else
            {
                // Ship/Asteroid — wrap to opposite side
                transform.position = _boundaryHandler
                    .HandleBoundary(transform.position);
            }
        }
        
        /// <summary>
        /// Deactivate without firing OnDestroyed.
        /// Used during pool initialisation only.
        /// </summary>
        public void DeactivateSilently()
        {
            if (_rigidbody != null)
                _rigidbody.linearVelocity = Vector2.zero;

            gameObject.SetActive(false);
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            IAsteroid asteroid = other.GetComponent<IAsteroid>();
            if (asteroid == null) return;

            // Asteroid handles the main logic
            // Bullet just deactivates itself
            Deactivate();
        }
    }
}