namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Generic object pool interface.
    /// Reuse objects instead of Instantiate/Destroy —
    /// critical for live game performance.
    /// Bullets and asteroids both use this.
    /// </summary>
    /// <typeparam name="T">Type of object to pool</typeparam>
    public interface IObjectPool<T>
    {
        /// <summary>
        /// Get an object from the pool.
        /// If pool is empty, creates a new one.
        /// </summary>
        T Get();

        /// <summary>
        /// Return an object back to the pool.
        /// </summary>
        void Return(T item);

        /// <summary>
        /// How many objects are currently available.
        /// </summary>
        int AvailableCount { get; }
    }
}