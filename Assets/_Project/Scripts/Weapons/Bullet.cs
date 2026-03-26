using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Projectile fired by the ship.
    /// Notifies pool via OnDeactivated when done.
    /// Deactivates at screen edge via IBulletBoundaryHandler.
    /// Uses TryGetComponent in collision for performance.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Bullet : MonoBehaviour, IBullet
    {
        [SerializeField] private float _speed    = 10f;
        [SerializeField] private float _lifetime = 2f;

        // ── IBullet ──────────────────────────────────────────────

        public bool IsActive => gameObject.activeSelf;
        public event Action<IBullet> OnDeactivated;

        // ── private fields ───────────────────────────────────────

        private Rigidbody2D      _rigidbody;
        private IBoundaryHandler _boundaryHandler;
        private float            _lifetimeTimer;

        // ── Unity lifecycle ──────────────────────────────────────

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            Debug.Assert(_rigidbody != null,
                $"[Bullet] Missing Rigidbody2D " +
                $"on {gameObject.name}");

            _rigidbody.gravityScale = 0f;
        }

        // ── initialisation ───────────────────────────────────────

        public void Initialise(IBoundaryHandler boundaryHandler)
        {
            if (boundaryHandler == null)
                throw new ArgumentNullException(
                    nameof(boundaryHandler));

            _boundaryHandler = boundaryHandler;
        }

        // ── IBullet ──────────────────────────────────────────────

        public void Launch(Vector3 position, Vector3 direction)
        {
            transform.position    = position;
            gameObject.SetActive(true);
            _lifetimeTimer        = _lifetime;
            _rigidbody.linearVelocity =
                direction.normalized * _speed;
        }

        public void Deactivate()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            OnDeactivated?.Invoke(this);
        }

        public void DeactivateSilently()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            // No event — used during pool initialisation
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
                if (_boundaryHandler.IsOutOfBounds(
                    transform.position))
                    Deactivate();
            }
            else
            {
                transform.position = _boundaryHandler
                    .HandleBoundary(transform.position);
            }
        }
    }
}
