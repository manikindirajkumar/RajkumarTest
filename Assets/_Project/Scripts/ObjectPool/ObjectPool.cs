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
            Transform parent,
            Action<T> onCreated,
            Func<GameObject, T> getComponent = null)
        {
            if (parent    == null)
                throw new ArgumentNullException(nameof(parent));
            if (onCreated == null)
                throw new ArgumentNullException(nameof(onCreated));
            _prefab    = null; 
            _parent       = parent;
            _onCreated    = onCreated;
            _getComponent = getComponent ??
                            (go => go.GetComponent<T>());

            CollectExistingChildren();
        }
        public ObjectPool(IEnumerable<T> items, Action<T> onCreated = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            _prefab    = null; 
            foreach (T item in items)
            {
                onCreated?.Invoke(item);
                _available.Enqueue(item);
            }
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
         

        public T Get()
        {
            if (_available.Count > 0)
                return _available.Dequeue();

            if (_prefab == null)      
                return null;

            T newItem = CreateItem();
            _onCreated?.Invoke(newItem);
            return newItem;
        }


        public void Return(T item)
        {
            _available.Enqueue(item);
        }

        public void Dispose()
        {
            _available.Clear();
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
