using System;
using System.Collections.Generic;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Lightweight service locator for cross-scene services.
    /// Used only for services that must persist between scenes
    /// and cannot be injected via VContainer at registration time.
    /// Preloader registers itself here before GameScene loads.
    /// GameLifetimeScope retrieves it without any scene search.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services
            = new Dictionary<Type, object>();

        public static void Register<T>(T service) where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services[typeof(T)] = service;
        }

        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out object service))
                return service as T;

            throw new InvalidOperationException(
                $"[ServiceLocator] Service not registered: {typeof(T).Name}. " +
                $"Make sure LoadingScene ran before GameScene.");
        }

        public static bool IsRegistered<T>() where T : class
            => _services.ContainsKey(typeof(T));

        public static void Unregister<T>() where T : class
            => _services.Remove(typeof(T));

        /// <summary>
        /// Clear all services — call in tests or full restart.
        /// </summary>
        public static void Clear()
            => _services.Clear();
    }
}