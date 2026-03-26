using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Manages creation and lifecycle of all object pools.
    /// Pure C# — no MonoBehaviour dependency.
    /// Synchronous — prefabs pre-cached in LoadingScene
    /// so WaitForCompletion() is instant.
    /// Call Initialise() after VContainer builds.
    /// Call Dispose() on game shutdown.
    /// </summary>
    public class PoolManager
    {
        // ── public accessors ─────────────────────────────────────

        public IObjectPool<IBullet>   BulletPool         => _bulletPool;
        public IObjectPool<IAsteroid> LargeAsteroidPool  => _largeAsteroidPool;
        public IObjectPool<IAsteroid> MediumAsteroidPool => _mediumAsteroidPool;
        public IObjectPool<IAsteroid> SmallAsteroidPool  => _smallAsteroidPool;

        // ── private fields ───────────────────────────────────────

        private ObjectPool<IBullet>   _bulletPool;
        private ObjectPool<IAsteroid> _largeAsteroidPool;
        private ObjectPool<IAsteroid> _mediumAsteroidPool;
        private ObjectPool<IAsteroid> _smallAsteroidPool;

        private IBoundaryHandler        _boundaryHandler;
        private IBulletBoundaryHandler  _bulletBoundaryHandler;
        private readonly Preloader     _preloader;
        private readonly IScoreSystem  _scoreSystem;
        private readonly IWaveConfig   _waveConfig;

        // ── constructor ──────────────────────────────────────────

        public PoolManager(
            Preloader     preloader, 
            IScoreSystem scoreSystem,
            IWaveConfig  waveConfig)
        {
            if (scoreSystem == null)
                throw new ArgumentNullException(nameof(scoreSystem));
            if (waveConfig  == null)
                throw new ArgumentNullException(nameof(waveConfig));

            _scoreSystem    = scoreSystem;
            _waveConfig     = waveConfig;
            _preloader      = preloader;
        }

        // ── public methods ───────────────────────────────────────

        /// <summary>
        /// Initialise all pools synchronously.
        /// Prefabs must be cached in Addressables
        /// before calling — use LoadingScene for this.
        /// </summary>
        public void Initialise(
            IBoundaryHandler       boundaryHandler,
            IBulletBoundaryHandler bulletBoundaryHandler)
        {

            _boundaryHandler       = boundaryHandler;
            _bulletBoundaryHandler = bulletBoundaryHandler;

            SetupBulletPool();
            SetupAsteroidPools();
        }

        public void Dispose()
        {
            _bulletPool?.Dispose();
            _largeAsteroidPool?.Dispose();
            _mediumAsteroidPool?.Dispose();
            _smallAsteroidPool?.Dispose();
        }

        // ── private setup ────────────────────────────────────────

        private void SetupBulletPool()
        {
            ObjectPool<IBullet> localPool = null;

            localPool = new ObjectPool<IBullet>(
                _preloader.BulletPoolParent,  // ← Transform, not prefab
                bullet =>
                {
                    if (bullet is Bullet b)
                        b.Initialise(_bulletBoundaryHandler);

                    bullet.OnDeactivated +=
                        _ => localPool?.Return(bullet);

                    (bullet as Bullet)?.DeactivateSilently();
                });

            _bulletPool = localPool;
        }

        private void SetupAsteroidPool(
            Transform parent,
            AsteroidSize size,
            out ObjectPool<IAsteroid> pool)
        {
            ObjectPool<IAsteroid> localPool = null;

            localPool = new ObjectPool<IAsteroid>(
                parent,                       // ← Transform, not prefab
                asteroid =>
                {
                    if (asteroid is Asteroid a)
                        a.Initialise(size, _boundaryHandler);

                    asteroid.OnDestroyed += (destroyed, pos) =>
                        _scoreSystem.AddScore(
                            _waveConfig.GetScoreForSize(
                                destroyed.Size));

                    asteroid.OnDestroyed +=
                        (destroyed, pos) =>
                            localPool?.Return(destroyed);

                    (asteroid as Asteroid)?.DeactivateSilently();
                });

            pool = localPool;
        }

    private void SetupAsteroidPools()
    {
        SetupAsteroidPool(
            _preloader.LargeAsteroidParent,   // ← direct ✅
            AsteroidSize.Large,
            out _largeAsteroidPool);

        SetupAsteroidPool(
            _preloader.MediumAsteroidParent,  // ← direct ✅
            AsteroidSize.Medium,
            out _mediumAsteroidPool);

        SetupAsteroidPool(
            _preloader.SmallAsteroidParent,   // ← direct ✅
            AsteroidSize.Small,
            out _smallAsteroidPool);
    }

        private Transform CreatePoolParent(string name)
        {
            return new GameObject(name).transform;
        }
    }
}
