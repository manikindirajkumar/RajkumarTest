using System;
using UnityEngine;
using UnityEngine.InputSystem;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Reads player input using Unity's new Input System
    /// and fires events on the IInputProvider contract.
    ///
    /// Uses AsteroidsInputActions — an Input Actions asset
    /// loaded via Addressables for clean asset management.
    ///
    /// All game logic subscribes to IInputProvider events —
    /// nothing reads hardware input directly.
    /// Swapping to gamepad or touch only requires
    /// a new IInputProvider implementation.
    /// </summary>
    public class KeyboardInputProvider : MonoBehaviour, IInputProvider
    {
        // ── IInputProvider events ────────────────────────────────

        public event Action OnShoot;
        public event Action OnThrustStarted;
        public event Action OnThrustStopped;
        public event Action OnRotateLeft;
        public event Action OnRotateRight;
        public event Action OnRotateStopped;
        public event Action OnRestartGame;

        // ── private fields ───────────────────────────────────────

        private AsteroidsInputActions _inputActions;

        // ── Unity lifecycle ──────────────────────────────────────

        private void Awake()
        {
            _inputActions = new AsteroidsInputActions();
        }

        private void OnEnable()
        {
            _inputActions.Gameplay.Enable();
            SubscribeToActions();
        }

        private void OnDisable()
        {
            UnsubscribeFromActions();
            _inputActions.Gameplay.Disable();
        }

        private void OnDestroy()
        {
            _inputActions?.Dispose();

            // Clear all subscribers
            OnShoot         = null;
            OnThrustStarted = null;
            OnThrustStopped = null;
            OnRotateLeft    = null;
            OnRotateRight   = null;
            OnRotateStopped = null;
            OnRestartGame   = null;
        }

        // ── subscription ─────────────────────────────────────────

        private void SubscribeToActions()
        {
            _inputActions.Gameplay.Thrust.started   += OnThrustStartedHandler;
            _inputActions.Gameplay.Thrust.canceled  += OnThrustStoppedHandler;

            _inputActions.Gameplay.RotateLeft.started  += OnRotateLeftHandler;
            _inputActions.Gameplay.RotateLeft.canceled += OnRotateStoppedHandler;

            _inputActions.Gameplay.RotateRight.started  += OnRotateRightHandler;
            _inputActions.Gameplay.RotateRight.canceled += OnRotateStoppedHandler;

            _inputActions.Gameplay.Shoot.performed   += OnShootHandler;
            _inputActions.Gameplay.Restart.performed += OnRestartHandler;
        }

        private void UnsubscribeFromActions()
        {
            _inputActions.Gameplay.Thrust.started   -= OnThrustStartedHandler;
            _inputActions.Gameplay.Thrust.canceled  -= OnThrustStoppedHandler;

            _inputActions.Gameplay.RotateLeft.started  -= OnRotateLeftHandler;
            _inputActions.Gameplay.RotateLeft.canceled -= OnRotateStoppedHandler;

            _inputActions.Gameplay.RotateRight.started  -= OnRotateRightHandler;
            _inputActions.Gameplay.RotateRight.canceled -= OnRotateStoppedHandler;

            _inputActions.Gameplay.Shoot.performed   -= OnShootHandler;
            _inputActions.Gameplay.Restart.performed -= OnRestartHandler;
        }

        // ── handlers ─────────────────────────────────────────────

        private void OnThrustStartedHandler(InputAction.CallbackContext ctx)
            => OnThrustStarted?.Invoke();

        private void OnThrustStoppedHandler(InputAction.CallbackContext ctx)
            => OnThrustStopped?.Invoke();

        private void OnRotateLeftHandler(InputAction.CallbackContext ctx)
            => OnRotateLeft?.Invoke();

        private void OnRotateRightHandler(InputAction.CallbackContext ctx)
            => OnRotateRight?.Invoke();

        private void OnRotateStoppedHandler(InputAction.CallbackContext ctx)
            => OnRotateStopped?.Invoke();

        private void OnShootHandler(InputAction.CallbackContext ctx)
            => OnShoot?.Invoke();

        private void OnRestartHandler(InputAction.CallbackContext ctx)
            => OnRestartGame?.Invoke();
    }
}