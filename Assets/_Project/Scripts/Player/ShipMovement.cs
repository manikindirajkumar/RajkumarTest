using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Controls spaceship movement based on input state.
    ///
    /// Responsibilities:
    ///   - Rotate ship based on TurnDirection from InputManager
    ///   - Apply thrust force when IsThrusting is true
    ///   - Wrap position via IBoundaryHandler
    ///
    /// Does NOT:
    ///   - Read keyboard input directly (KeyboardInputProvider does that)
    ///   - Know anything about scoring or health
    ///   - Handle shooting (SpaceGun does that)
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class ShipMovement : MonoBehaviour
    {
        // ── serialised config ────────────────────────────────────

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

        // ── private dependencies ─────────────────────────────────

        private Rigidbody2D _rigidbody;
        private InputManager _inputManager;
        private IBoundaryHandler _boundaryHandler;
        private IHealthSystem _healthSystem;
        // ── initialisation ───────────────────────────────────────

        /// <summary>
        /// Inject dependencies after construction.
        /// Called by GameInstaller or equivalent bootstrap class.
        /// </summary>
        public void Initialise(
            InputManager inputManager,
            IBoundaryHandler boundaryHandler,
            IHealthSystem healthSystem)     // ← add
        {
            _inputManager    = inputManager;
            _boundaryHandler = boundaryHandler;
            _healthSystem    = healthSystem;
        }

       

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            // Remove gravity — space has no gravity
            _rigidbody.gravityScale = 0f;
        }

        // ── Unity lifecycle ──────────────────────────────────────

        private void Update()
        {
            if (_inputManager == null) return;

            HandleRotation();
            HandleThrust();
            ClampVelocity();
            WrapPosition();
        }

        // ── private movement methods ─────────────────────────────

        private void HandleRotation()
        {
            // TurnDirection: -1 = left, 0 = none, 1 = right
            float rotation = -_inputManager.TurnDirection
                             * _rotationSpeed
                             * Time.deltaTime;

            transform.Rotate(0f, 0f, rotation);
        }

        private void HandleThrust()
        {
            if (!_inputManager.IsThrusting) return;

            // Apply force in the direction the ship is facing
            // transform.up = the ship's forward direction in 2D
            _rigidbody.AddForce(
                transform.up * _thrustForce,
                ForceMode2D.Force);
        }

        private void ClampVelocity()
        {
            // Prevent ship from accelerating infinitely
            if (_rigidbody.linearVelocity.magnitude > _maxVelocity)
            {
                _rigidbody.linearVelocity = _rigidbody.linearVelocity
                    .normalized * _maxVelocity;
            }
        }

        private void WrapPosition()
        {
            transform.position = _boundaryHandler
                .HandleBoundary(transform.position);
        }
        
        private bool _isInvincible;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void SetInvincible(bool invincible)
        {
            _isInvincible = invincible;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore hits during invincibility
            if (_isInvincible) return;

            if (!other.TryGetComponent(out IAsteroid _)) return;
            _healthSystem?.LoseLife();
        }
    }
}