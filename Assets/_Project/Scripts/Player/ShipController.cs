using UnityEngine;
using VContainer;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Controls spaceship movement based on input state.
    /// Dependencies injected by VContainer via [Inject].
    /// No manual Initialise() call needed.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField]
        [Tooltip("How fast the ship rotates in degrees per second")]
        private float _rotationSpeed = 180f;

        [SerializeField]
        [Tooltip("Force applied when thrusting")]
        private float _thrustForce = 5f;
        
        
        [SerializeField]
        [Tooltip("Maximum velocity the ship can reach")]
        private float _maxVelocity = 8f;

        private Rigidbody2D      _rigidbody;
        private InputManager     _inputManager;
        private IBoundaryHandler _boundaryHandler;
        private IHealthSystem    _healthSystem;
        private bool             _isInvincible;
        private bool             _isActive = true;

         

        [Inject]
        public void Construct(
            InputManager inputManager,
            IBoundaryHandler boundaryHandler,
            IHealthSystem healthSystem)
        {
            _inputManager    = inputManager;
            _boundaryHandler = boundaryHandler;
            _healthSystem    = healthSystem;
        }

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            Debug.Assert(_rigidbody != null,
                $"[ShipMovement] Missing Rigidbody2D " +
                $"on {gameObject.name}");

            _rigidbody.gravityScale = 0f;
        }

        private void Update()
        {
            if (_inputManager == null || !_isActive) return;

            HandleRotation();
            HandleThrust();
            ClampVelocity();
            WrapPosition();
        }

         

        public void SetActive(bool active)
        {
            _isActive = active;
            enabled = active;
        }

        public void SetInvincible(bool invincible)
        {
            _isInvincible = invincible;
        }

         

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isInvincible) return;

            if (!other.TryGetComponent(out IAsteroid _)) return;

            _healthSystem?.LoseLife();
        }

         

        private void HandleRotation()
        {
            float rotation = -_inputManager.TurnDirection
                             * _rotationSpeed
                             * Time.deltaTime;

            transform.Rotate(0f, 0f, rotation);
        }

        private void HandleThrust()
        {
            if (!_inputManager.IsThrusting) return;

            _rigidbody.AddForce(
                transform.up * _thrustForce,
                ForceMode2D.Force);
        }

        private void ClampVelocity()
        {
            if (_rigidbody.linearVelocity.magnitude > _maxVelocity)
            {
                _rigidbody.linearVelocity =
                    _rigidbody.linearVelocity.normalized
                    * _maxVelocity;
            }
        }

        private void WrapPosition()
        {
            transform.position = _boundaryHandler
                .HandleBoundary(transform.position);
        }
        public void StopPhysics()
        {
            if (_rigidbody == null) return;

            _rigidbody.linearVelocity        = Vector2.zero;
            _rigidbody.angularVelocity = 0f;
        }
    }
}
