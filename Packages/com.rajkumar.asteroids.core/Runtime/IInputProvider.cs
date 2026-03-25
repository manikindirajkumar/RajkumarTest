using System;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Abstracts all player input from game logic.
    /// Implement this interface to support different input sources —
    /// keyboard, gamepad, touch, or automated test input —
    /// without changing any game logic code.
    /// </summary>
    public interface IInputProvider
    {
        /// <summary>Fired when player presses the shoot button.</summary>
        event Action OnShoot;

        /// <summary>Fired when player starts applying thrust.</summary>
        event Action OnThrustStarted;

        /// <summary>Fired when player releases the thrust button.</summary>
        event Action OnThrustStopped;

        /// <summary>Fired when player starts rotating left.</summary>
        event Action OnRotateLeft;

        /// <summary>Fired when player starts rotating right.</summary>
        event Action OnRotateRight;

        /// <summary>Fired when player releases the rotation input.</summary>
        event Action OnRotateStopped;

        /// <summary>Fired when player triggers a game restart.</summary>
        event Action OnRestartGame;
    }
}