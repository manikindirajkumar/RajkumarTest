using UnityEngine;
using VContainer.Unity;

namespace RajkumarTest.Asteroid.Core
{
    public class GameBootstrapper : IStartable
    {
        private readonly InputManager              _inputManager;
        private readonly IInputProvider            _inputProvider;
        private readonly PoolManager               _poolManager;
        private readonly IBoundaryHandler          _boundaryHandler;
        private readonly IBulletBoundaryHandler    _bulletBoundary;
        private readonly IScoreSystem              _scoreSystem;
        private readonly IHealthSystem             _healthSystem;
        private readonly IWaveConfig               _waveConfig;
        private readonly IBoundaries               _boundaries;
        private readonly HUDView                   _hudView;
        private readonly GameOverView              _gameOverView;
        private readonly PlayerFactory             _playerFactory;
        private readonly IGameManager               _gameManager;
    
        // Created manually after pools ready
        private AsteroidSpawner     _asteroidSpawner;
        private AsteroidManager     _asteroidManager;
    

        public GameBootstrapper(
            InputManager           inputManager,
            IGameManager           gameManager,
            IInputProvider         inputProvider,
            PoolManager            poolManager,
            IBoundaryHandler       boundaryHandler,
            IBulletBoundaryHandler bulletBoundary,
            IScoreSystem           scoreSystem,
            IHealthSystem          healthSystem,
            IWaveConfig            waveConfig,
            IBoundaries            boundaries,
            HUDView                hudView,
            GameOverView           gameOverView,
            PlayerFactory          playerFactory)
        {
            _inputManager    = inputManager;
            _gameManager     = gameManager;
            _inputProvider   = inputProvider;
            _poolManager     = poolManager;
            _boundaryHandler = boundaryHandler;
            _bulletBoundary  = bulletBoundary;
            _scoreSystem     = scoreSystem;
            _healthSystem    = healthSystem;
            _waveConfig      = waveConfig;
            _boundaries      = boundaries;
            _hudView         = hudView;
            _gameOverView    = gameOverView;
            _playerFactory   = playerFactory;
        }

        public void Start()
        {
            SetupInput();
            SetupAsteroids();
            SpawnPlayer();
            SetupUI();

            _gameManager.StartGame();
            _asteroidManager.StartGame();
        }

        private void SetupInput()
        {
            _inputManager.Subscribe(_inputProvider);
        }

         

        private void SetupAsteroids()
        {
            var spawnProvider = new EdgeSpawnPositionProvider();
            _asteroidSpawner = new AsteroidSpawner(
                _poolManager.LargeAsteroidPool,
                _poolManager.MediumAsteroidPool,
                _poolManager.SmallAsteroidPool,
                spawnProvider,
                _boundaries,
                _waveConfig);

            // WaveManager subscribes to spawner in constructor
            _asteroidManager = new AsteroidManager(
                _asteroidSpawner,
                _waveConfig,
                _gameManager); 
        

            // Wire ClearAll return callback
            _asteroidSpawner.SetReturnCallback(asteroid =>
            {
                if (asteroid.Size == AsteroidSize.Large)
                    _poolManager.LargeAsteroidPool.Return(asteroid);
                else if (asteroid.Size == AsteroidSize.Medium)
                    _poolManager.MediumAsteroidPool.Return(asteroid);
                else
                    _poolManager.SmallAsteroidPool.Return(asteroid);
            });
        }

        private void SpawnPlayer()
        {
            _playerFactory.Create(Vector3.zero);
        }

        private void SetupUI()
        {
            _hudView?.Initialise(_scoreSystem, _healthSystem, _asteroidManager);
            _gameOverView?.Initialise(_gameManager, _scoreSystem);
        }
    }
}