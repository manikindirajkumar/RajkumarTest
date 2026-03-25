using System;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Reads input state from IInputProvider and exposes
    /// clean movement values for the ship to consume.
    /// 
    /// Separates "what button is pressed" (IInputProvider)
    /// from "what state does the ship need" (this class).
    /// 
    /// Subscribe once, unsubscribe cleanly using
    /// stored method references — never lambdas.
    /// </summary>
    public class InputManager
    {
        // ── public state ─────────────────────────────────────────

        /// <summary>
        /// -1 = rotating left, 0 = not rotating, 1 = rotating right
        /// </summary>
        public float TurnDirection { get; private set; }

        /// <summary>
        /// True while the player is holding thrust input.
        /// </summary>
        public bool IsThrusting { get; private set; }

        // ── private fields ───────────────────────────────────────

        private IInputProvider _inputProvider;

        // Store method references so we can unsubscribe correctly
        // Anonymous lambdas CANNOT be unsubscribed — always use named methods
        private Action _onThrustStarted;
        private Action _onThrustStopped;
        private Action _onRotateLeft;
        private Action _onRotateRight;
        private Action _onRotateStopped;

        // ── public methods ───────────────────────────────────────

        /// <summary>
        /// Subscribe to input events from the given provider.
        /// Must be called before using TurnDirection or IsThrusting.
        /// </summary>
        public void Subscribe(IInputProvider inputProvider)
        {
            if (inputProvider == null)
                throw new ArgumentNullException(
                    nameof(inputProvider),
                    "InputProvider cannot be null.");

            _inputProvider = inputProvider;

            // Store as named methods so Unsubscribe works correctly
            _onThrustStarted  = () => IsThrusting   = true;
            _onThrustStopped  = () => IsThrusting   = false;
            _onRotateLeft     = () => TurnDirection  = -1f;
            _onRotateRight    = () => TurnDirection  =  1f;
            _onRotateStopped  = () => TurnDirection  =  0f;

            _inputProvider.OnThrustStarted  += _onThrustStarted;
            _inputProvider.OnThrustStopped  += _onThrustStopped;
            _inputProvider.OnRotateLeft     += _onRotateLeft;
            _inputProvider.OnRotateRight    += _onRotateRight;
            _inputProvider.OnRotateStopped  += _onRotateStopped;
        }

        /// <summary>
        /// Unsubscribe from all input events.
        /// Always call this when the ship is destroyed or disabled
        /// to prevent memory leaks and ghost callbacks.
        /// </summary>
        public void Unsubscribe()
        {
            if (_inputProvider == null) return;

            _inputProvider.OnThrustStarted  -= _onThrustStarted;
            _inputProvider.OnThrustStopped  -= _onThrustStopped;
            _inputProvider.OnRotateLeft     -= _onRotateLeft;
            _inputProvider.OnRotateRight    -= _onRotateRight;
            _inputProvider.OnRotateStopped  -= _onRotateStopped;

            // Clear references — allow GC to collect
            _inputProvider    = null;
            _onThrustStarted  = null;
            _onThrustStopped  = null;
            _onRotateLeft     = null;
            _onRotateRight    = null;
            _onRotateStopped  = null;
        }

        /// <summary>
        /// Reset input state to neutral.
        /// Call on game restart or ship respawn.
        /// </summary>
        public void Reset()
        {
            TurnDirection = 0f;
            IsThrusting   = false;
        }
    }
}