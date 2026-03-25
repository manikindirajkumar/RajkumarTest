using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Generic reusable object pool.
    /// Loads prefab via IAssetLoader — no direct
    /// prefab reference needed.
    /// No MonoBehaviour dependency —
    /// pure C# with Unity instantiation.
    ///
    /// Works for any poolable object that:
    ///   1. Has a component of type T
    ///   2. Implements IPoolable
    /// </summary>
    public class ObjectPool<T> : IObjectPool<T>
        where T : class
    {
        // ── private fields ───────────────────────────────────────

        private readonly Queue<T> _available
            = new Queue<T>();

        private readonly string _address;
        private readonly int _initialSize;
        private readonly Transform _parent;
        private readonly IAssetLoader<GameObject> _assetLoader;
        private readonly Func<GameObject, T> _getComponent;
        private readonly Action<T> _onCreated;

        public int AvailableCount => _available.Count;

        // ── constructor ──────────────────────────────────────────

        /// <summary>
        /// Create a new object pool.
        /// </summary>
        /// <param name="address">
        /// Addressable address of prefab
        /// </param>
        /// <param name="initialSize">
        /// How many objects to pre-create
        /// </param>
        /// <param name="parent">
        /// Parent transform for instantiated objects
        /// </param>
        /// <param name="assetLoader">
        /// Loads the prefab from Addressables
        /// </param>
        /// <param name="getComponent">
        /// How to get T from instantiated GameObject
        /// </param>
        /// <param name="onCreated">
        /// Optional — called after each object created
        /// Use to inject dependencies into pooled object
        /// </param>
        public ObjectPool(
            string address,
            int initialSize,
            Transform parent,
            IAssetLoader<GameObject> assetLoader,
            Func<GameObject, T> getComponent,
            Action<T> onCreated = null)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            if (initialSize <= 0)
                throw new ArgumentException(
                    "Pool size must be greater than zero.",
                    nameof(initialSize));
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (assetLoader == null)
                throw new ArgumentNullException(nameof(assetLoader));
            if (getComponent == null)
                throw new ArgumentNullException(nameof(getComponent));

            _address      = address;
            _initialSize  = initialSize;
            _parent       = parent;
            _assetLoader  = assetLoader;
            _getComponent = getComponent;
            _onCreated    = onCreated;
        }

        // ── IObjectPool ──────────────────────────────────────────

        public T Get()
        {
            if (_available.Count == 0)
            {
                Debug.LogWarning(
                    $"[ObjectPool<{typeof(T).Name}>] " +
                    $"Pool empty for {_address}. " +
                    $"Consider increasing initial size.");
                return null;
            }

            return _available.Dequeue();
        }

        public void Return(T item)
        {
            _available.Enqueue(item);
        }

        // ── initialisation ───────────────────────────────────────

        /// <summary>
        /// Async initialise — loads prefab and prewarms pool.
        /// Must be awaited before using Get().
        /// </summary>
        public async Task InitialiseAsync()
        {
            GameObject prefab = await _assetLoader
                .LoadAsync(_address);

            if (prefab == null)
            {
                Debug.LogError(
                    $"[ObjectPool<{typeof(T).Name}>] " +
                    $"Failed to load prefab: {_address}");
                return;
            }

            for (int i = 0; i < _initialSize; i++)
            {
                T item = CreateItem(prefab);
                _available.Enqueue(item);
            }
        }

        public void Release()
        {
            _assetLoader.Release(_address);
            _available.Clear();
        }

        // ── private ──────────────────────────────────────────────

        private T CreateItem(GameObject prefab)
        {
            GameObject go = UnityEngine.Object.Instantiate(
                prefab,
                Vector3.zero,
                Quaternion.identity,
                _parent);

            T item = _getComponent(go);

            // Inject dependencies via callback
            _onCreated?.Invoke(item);

            return item;
        }
    }
}