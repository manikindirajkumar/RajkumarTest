using System;
using System.Collections.Generic;
using UnityEngine;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Generic reusable object pool.
    /// Takes prefab directly — no async loading.
    /// Prefabs must be pre-cached via LoadingScene
    /// before pool is created.
    /// Works for any poolable object type.
    /// </summary>
    public class ObjectPool<T> : IObjectPool<T>
        where T : class
    {
        private readonly Queue<T>       _available = new Queue<T>();
        private readonly GameObject     _prefab;
        private readonly int            _initialSize;
        private readonly Transform      _parent;
        private readonly Action<T>      _onCreated;
        private readonly Func<GameObject, T> _getComponent;

        public int AvailableCount => _available.Count;

        public ObjectPool(
            GameObject prefab,
            int initialSize,
            Transform parent,
            Action<T> onCreated,
            Func<GameObject, T> getComponent = null)
        {
            if (prefab      == null)
                throw new ArgumentNullException(nameof(prefab));
            if (initialSize <= 0)
                throw new ArgumentException(
                    "Pool size must be greater than zero.",
                    nameof(initialSize));
            if (parent      == null)
                throw new ArgumentNullException(nameof(parent));
            if (onCreated   == null)
                throw new ArgumentNullException(nameof(onCreated));

            _prefab       = prefab;
            _initialSize  = initialSize;
            _parent       = parent;
            _onCreated    = onCreated;
            _getComponent = getComponent ??
                (go => go.GetComponent<T>());

            Prewarm();
        }

        public ObjectPool(
            Transform parent,
            Action<T> onCreated,
            Func<GameObject, T> getComponent = null)
        {
            if (parent    == null)
                throw new ArgumentNullException(nameof(parent));
            if (onCreated == null)
                throw new ArgumentNullException(nameof(onCreated));

            _parent       = parent;
            _onCreated    = onCreated;
            _getComponent = getComponent ??
                            (go => go.GetComponent<T>());

            CollectExistingChildren();
        }

        private void CollectExistingChildren()
        {
            foreach (Transform child in _parent)
            {
                T item = _getComponent(child.gameObject);
                if (item == null) continue;

                _onCreated?.Invoke(item);
                _available.Enqueue(item);
            }
        }
        // ── IObjectPool ──────────────────────────────────────────

        public T Get()
        {
            if (_available.Count == 0)
            {
                return CreateItem();
            }

            return _available.Dequeue();
        }

        public void Return(T item)
        {
            _available.Enqueue(item);
        }

        public void Dispose()
        {
            _available.Clear();
        }

        // ── private ──────────────────────────────────────────────

        private void Prewarm()
        {
            for (int i = 0; i < _initialSize; i++)
            {
                T item = CreateItem();
                _available.Enqueue(item);
            }
        }

        private T CreateItem()
        {
            GameObject go = UnityEngine.Object.Instantiate(
                _prefab,
                Vector3.zero,
                Quaternion.identity,
                _parent);

            T item = _getComponent(go);
            _onCreated?.Invoke(item);
            return item;
        }
    }
}
