using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Provides randomised values for asteroid spawning behaviour.
    /// Abstracted so unit tests can inject deterministic results
    /// instead of relying on UnityEngine.Random,
    /// making tests predictable and repeatable.
    /// </summary>
    public interface IAsteroidSelector
    {
        /// <summary>
        /// Returns a random asteroid size index.
        /// 0 = Large, 1 = Medium, 2 = Small
        /// </summary>
        int GetRandomAsteroidSizeIndex();

        /// <summary>
        /// Returns a random normalised direction vector
        /// for asteroid movement.
        /// </summary>
        Vector2 GetRandomDirection();
    }
}