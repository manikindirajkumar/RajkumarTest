using System.Threading.Tasks;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Manages creation and lifecycle of all object pools.
    /// Pure C# class — no MonoBehaviour dependency.
    /// Call InitialiseAsync() after boundaries are ready.
    /// Call Dispose() on game shutdown.
    /// </summary>
    public class PoolManager
    {
        // ── public accessors — read only ─────────────────────────

        public IObjectPool<IBullet>   BulletPool          => _bulletPool;
        public IObjectPool<IAsteroid> LargeAsteroidPool   => _largeAsteroidPool;
        public IObjectPool<IAsteroid> MediumAsteroidPool  => _mediumAsteroidPool;
        public IObjectPool<IAsteroid> SmallAsteroidPool   => _smallAsteroidPool;

        // ── private fields ───────────────────────────────────────

        private ObjectPool<IBullet>   _bulletPool;
        private ObjectPool<IAsteroid> _largeAsteroidPool;
        private ObjectPool<IAsteroid> _mediumAsteroidPool;
        private ObjectPool<IAsteroid> _smallAsteroidPool;

        private IBoundaryHandler _boundaryHandler;
        private IBoundaryHandler _bulletBoundaryHandler;
        private readonly IAssetLoader<GameObject> _assetLoader;
        private readonly int _bulletPoolSize;
        private readonly int _asteroidPoolSize;

        // ── constructor ──────────────────────────────────────────

        private readonly IScoreSystem _scoreSystem;
        private readonly IWaveConfig  _waveConfig;

        public PoolManager(
            IAssetLoader<GameObject> assetLoader,
            IScoreSystem scoreSystem,
            IWaveConfig waveConfig,
            int bulletPoolSize   = 10,
            int asteroidPoolSize = 10)
        {
            _assetLoader      = assetLoader;
            _scoreSystem      = scoreSystem;
            _waveConfig       = waveConfig;
            _bulletPoolSize   = bulletPoolSize;
            _asteroidPoolSize = asteroidPoolSize;
        }

        // ── public methods ───────────────────────────────────────

        /// <summary>
        /// Initialise all pools.
        /// Must be called AFTER boundaries are created.
        /// Must be awaited before using any pool.
        /// </summary>
        public async Task InitialiseAsync(
            IBoundaryHandler boundaryHandler,
            IBoundaryHandler bulletBoundaryHandler)
        {
            if (boundaryHandler == null)
                throw new System.ArgumentNullException(
                    nameof(boundaryHandler));

            if (bulletBoundaryHandler == null)
                throw new System.ArgumentNullException(
                    nameof(bulletBoundaryHandler));

            _boundaryHandler       = boundaryHandler;
            _bulletBoundaryHandler = bulletBoundaryHandler;

            await SetupBulletPoolAsync();
            await SetupAsteroidPoolsAsync();
        }

        /// <summary>
        /// Release all pool resources.
        /// Call from GameInstaller.OnDestroy().
        /// </summary>
        public void Dispose()
        {
            _bulletPool?.Release();
            _largeAsteroidPool?.Release();
            _mediumAsteroidPool?.Release();
            _smallAsteroidPool?.Release();
        }

        // ── private setup ────────────────────────────────────────

        private async Task SetupBulletPoolAsync()
        {
            Transform parent = CreatePoolParent("[Pool] Bullets");

            _bulletPool = new ObjectPool<IBullet>(
                address:      AddressableKeys.Prefabs.Bullet1,
                initialSize:  _bulletPoolSize,
                parent:       parent,
                assetLoader:  _assetLoader,
                getComponent: go => go.GetComponent<IBullet>(),
                onCreated:    bullet =>
                {
                    if (bullet is Bullet b)
                        b.Initialise(_bulletBoundaryHandler);

                    bullet.OnDeactivated += _bulletPool.Return;
                    (bullet as Bullet)?.DeactivateSilently();
                });

            await _bulletPool.InitialiseAsync();
        }

        private async Task SetupAsteroidPoolsAsync()
        {
            Transform parent =
                CreatePoolParent("[Pool] Asteroids");

            _largeAsteroidPool  = CreateAsteroidPool(
                AddressableKeys.Prefabs.LargeAsteroid,
                AsteroidSize.Large,
                10,
                parent);

            _mediumAsteroidPool = CreateAsteroidPool(
                AddressableKeys.Prefabs.MediumAsteroid,
                AsteroidSize.Medium,
                20,
                parent);

            _smallAsteroidPool  = CreateAsteroidPool(
                AddressableKeys.Prefabs.SmallAsteroid,
                AsteroidSize.Small,
                40,
                parent);

            await _largeAsteroidPool.InitialiseAsync();
            await _mediumAsteroidPool.InitialiseAsync();
            await _smallAsteroidPool.InitialiseAsync();
        }

        private ObjectPool<IAsteroid> CreateAsteroidPool(
            string address,
            AsteroidSize size,
            int pooledObjectCount,
            Transform parent)
        {
            ObjectPool<IAsteroid> pool = null;

            pool = new ObjectPool<IAsteroid>(
                address: address,
                initialSize: _asteroidPoolSize,
                parent: parent,
                assetLoader: _assetLoader,
                getComponent: go => go.GetComponent<IAsteroid>(),
                onCreated: asteroid =>
                {
                    if (asteroid is Asteroid a)
                        a.Initialise(size, _boundaryHandler); // ← simplified

                    // Score via observer pattern
                    asteroid.OnDestroyed += (destroyed, pos) =>
                        _scoreSystem.AddScore(
                            _waveConfig.GetScoreForSize(destroyed.Size));

                    asteroid.OnDestroyed += (destroyed, pos) => pool?.Return(destroyed);
                    asteroid.OnReturnToPool += item        => pool?.Return(item);

                     
                    (asteroid as Asteroid)?.DeactivateSilently();
                });

            return pool;
        }

        private Transform CreatePoolParent(string name)
        {
            return new GameObject(name).transform;
        }
    }
}