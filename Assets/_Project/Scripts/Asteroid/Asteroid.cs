using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Single asteroid — moves in straight line,
    /// wraps at screen edges, destroys on bullet hit.
    /// Uses BoundaryHandler for wrapping —
    /// same as ship.
    /// Returns to pool via OnDestroyed event.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Asteroid : MonoBehaviour, IAsteroid
    {
        // ── IAsteroid ────────────────────────────────────────────

        public bool IsActive => gameObject.activeSelf;
        public AsteroidSize Size { get; private set; }
        public event Action<IAsteroid, Vector3> OnDestroyed;

        // ── private fields ───────────────────────────────────────
        [SerializeField]
        private Rigidbody2D _rigidbody;
        private IBoundaryHandler _boundaryHandler;
        private string _playerTag = "Player";
        public event Action<IAsteroid> OnReturnToPool;
        // ── initialisation ───────────────────────────────────────
         

        private void Awake()
        {

            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody2D>();
            }
            if (_rigidbody != null)
            {
                _rigidbody.gravityScale = 0f;    
            }
        }

        public void Initialise(
            AsteroidSize size,
            IBoundaryHandler boundaryHandler)
        {
            Size             = size;
            _boundaryHandler = boundaryHandler;
        }

        // ── IAsteroid implementation ─────────────────────────────

        public void Activate(
            Vector3 position,
            Vector3 direction,
            float speed)
        {
            transform.position = position;
            gameObject.SetActive(true);

            _rigidbody.linearVelocity =
                direction.normalized * speed;
        }

        public void Destroy()
        {
            Vector3 position = transform.position; // capture before deactivate
            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            OnDestroyed?.Invoke(this, position);
        }

        // ── Unity lifecycle ──────────────────────────────────────

        private void Update()
        {
            if (!IsActive || _boundaryHandler == null)
                return;

            // Wrap position — same as ship
            transform.position = _boundaryHandler
                .HandleBoundary(transform.position);
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
            if (!other.TryGetComponent(out IBullet bullet)) return;
            if (bullet == null) return;

            bullet.Deactivate();
            Destroy();
        }
    }
}