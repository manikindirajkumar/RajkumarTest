using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Single asteroid — moves in straight line,
    /// wraps at screen edges, destroys on bullet hit.
    /// Returns to pool via OnDestroyed event.
    /// Uses TryGetComponent in collision — no GetComponent overhead.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Asteroid : MonoBehaviour, IAsteroid
    {
        public bool IsActive => gameObject.activeSelf;
        public AsteroidSize Size { get; private set; }
        public event Action<IAsteroid, Vector3> OnDestroyed;
        
        private Rigidbody2D      _rigidbody;
        private IBoundaryHandler _boundaryHandler;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            Debug.Assert(_rigidbody != null,
                $"[Asteroid] Missing Rigidbody2D " +
                $"on {gameObject.name}");

            _rigidbody.gravityScale = 0f;
        }

         

        public void Initialise(
            AsteroidSize size,
            IBoundaryHandler boundaryHandler)
        {
            if (boundaryHandler == null)
                throw new ArgumentNullException(
                    nameof(boundaryHandler));

            Size             = size;
            _boundaryHandler = boundaryHandler;
        }

         

        public void Activate(
            Vector3 position,
            Vector3 direction,
            float speed)
        {
            transform.position    = position;
            gameObject.SetActive(true);
            _rigidbody.linearVelocity =
                direction.normalized * speed;
        }

        public void Destroy()
        {
            // Capture position before deactivating
            Vector3 position = transform.position;

            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);

            // Pass position so spawner knows where to split
            OnDestroyed?.Invoke(this, position);
        }
        

        public void DeactivateSilently()
        {
            _rigidbody.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            // No event — used for pool init and ClearAll
        }

         

        private void Update()
        {
            if (!IsActive || _boundaryHandler == null) return;

            transform.position = _boundaryHandler
                .HandleBoundary(transform.position);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // TryGetComponent — faster than GetComponent
            if (!other.TryGetComponent(out IBullet bullet)) return;

            bullet.Deactivate();
            Destroy();
        }
    }
}
