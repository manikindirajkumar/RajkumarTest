using System;
using UnityEngine;
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
         

        public IObjectPool<IBullet>   BulletPool         => _bulletPool;
        public IObjectPool<IAsteroid> LargeAsteroidPool  => _largeAsteroidPool;
        public IObjectPool<IAsteroid> MediumAsteroidPool => _mediumAsteroidPool;
        public IObjectPool<IAsteroid> SmallAsteroidPool  => _smallAsteroidPool;

         

        private ObjectPool<IBullet>   _bulletPool;
        private ObjectPool<IAsteroid> _largeAsteroidPool;
        private ObjectPool<IAsteroid> _mediumAsteroidPool;
        private ObjectPool<IAsteroid> _smallAsteroidPool;

        private IBoundaryHandler        _boundaryHandler;
        private IBulletBoundaryHandler  _bulletBoundaryHandler;
        private readonly Preloader     _preloader;
        private readonly IScoreSystem  _scoreSystem;
        private readonly IWaveConfig   _waveConfig;

         

        public PoolManager(
            Preloader              preloader,
            IScoreSystem           scoreSystem,
            IWaveConfig            waveConfig,
            IBoundaryHandler       boundaryHandler,
            IBulletBoundaryHandler bulletBoundaryHandler)
        {
            _preloader             = preloader;
            _scoreSystem           = scoreSystem;
            _waveConfig            = waveConfig;
            _boundaryHandler       = boundaryHandler;
            _bulletBoundaryHandler = bulletBoundaryHandler;
            Initialise();
        }
        private void Initialise()
        {
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

 

        private void SetupBulletPool()
        {
            ObjectPool<IBullet> localPool = null;

            localPool = new ObjectPool<IBullet>(_preloader.BulletPoolParent,  
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
                parent,                      
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
        SetupAsteroidPool(_preloader.LargeAsteroidParent, AsteroidSize.Large, out _largeAsteroidPool);
        SetupAsteroidPool(_preloader.MediumAsteroidParent, AsteroidSize.Medium, out _mediumAsteroidPool);
        SetupAsteroidPool(_preloader.SmallAsteroidParent, AsteroidSize.Small, out _smallAsteroidPool);
    }
    
    }
}
