// using System.Threading.Tasks;
// using UnityEngine;
// using RajkumarTest.Asteroid.Core;
//
// namespace RajkumarTest.Asteroid
// {
//     public class GameInstaller : MonoBehaviour
// {
//     // ── scene references ─────────────────────────────────────
//     // AsteroidSpawner and WaveManager removed from here ✅
//
//     [Header("Scene References")]
//     [SerializeField] private KeyboardInputProvider _keyboardInput;
//     [SerializeField] private WaveConfig _waveConfig;
//     [SerializeField] private ShipMovement _shipMovement;
//     [SerializeField] private SpaceGun _spaceGun;
//     [Header("Pool Settings")]
//     [SerializeField] private int _bulletPoolSize   = 10;
//     [SerializeField] private int _asteroidPoolSize = 10;
//     
//     [Header("UI Setup")] 
//     [SerializeField] private HUDView _hudView;
//     [SerializeField] private GameOverView _gameOverView;
//     [SerializeField] private RespawnSystem _respawnSystem;
//     
// // ── private fields ───────────────────────────────────────
//     private IGameManager _gameManager;
//     private InputManager _inputManager;
//     private IBoundaries _boundaries;
//     private IBoundaryHandler _boundaryHandler;
//     private IBulletBoundaryHandler _bulletBoundaryHandler;
//     private IAssetLoader<GameObject> _prefabLoader;
//     private PoolManager _poolManager;
//     private AsteroidSpawner _asteroidSpawner;  
//     private WaveManager _waveManager;          
//     private IScoreSystem _scoreSystem;
//     private IHealthSystem _healthSystem;
//     
//     private async void Start()
//     {
//         try
//         {
//             SetupLoaders();
//             SetupBoundaries();
//             SetupInput();
//             
//             SetupAsteroids();
//             SetupGameManager();   
//             SetupUI();            
//             StartGame();
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError(
//                 $"[GameInstaller] Init failed: " +
//                 $"{e.Message}\n{e.StackTrace}");
//         }
//     }
//
//     private void OnDestroy()
//     {
//         _inputManager?.Unsubscribe();
//        
//         
//         if (_gameManager != null)
//             _gameManager.OnStateChanged  -= HandleStateChanged;
//     }
//     private void SetupGameManager()
//     {
//         _gameManager = new GameManager(
//             _healthSystem,
//             _scoreSystem,
//             _waveManager,
//             _asteroidSpawner);
//         
//
//         _gameManager.OnStateChanged += HandleStateChanged;
//     }
//
//     private void SetupUI()
//     {
//         _hudView?.Initialise(
//             _scoreSystem,
//             _healthSystem,
//             _waveManager);
//
//         _gameOverView?.Initialise(
//             _gameManager,
//             _scoreSystem);
//     }
//
//     private void HandleStateChanged(GameState state)
//     {
//         switch (state)
//         {
//             case GameState.GameOver:
//                 _inputManager.Unsubscribe();
//                 break;
//
//             case GameState.Playing:
//                 // Re-subscribe on restart
//                 _inputManager.Subscribe(_keyboardInput);
//                 break;
//         }
//     }
//     
//
//     private void SetupLoaders()
//     {
//         if (_waveConfig == null)
//         {
//             Debug.LogError("[GameInstaller] WaveConfig not assigned.");
//             return;
//         }
//
//         _prefabLoader  = new AddressableAssetLoader<GameObject>();
//         _scoreSystem   = new ScoreSystem();
//         _healthSystem  = new HealthSystem(3); 
//         
//         _poolManager = new PoolManager(
//             _scoreSystem,
//             _waveConfig,
//             _bulletPoolSize,
//             _asteroidPoolSize);
//     }
//     private void SetupBoundaries()
//     {
//         var boundary = new GameBoardBoundary(Camera.main);
//         _boundaries = boundary;
//
//         _boundaryHandler       = new BoundaryHandler(_boundaries);
//         _bulletBoundaryHandler = new BoundaryDestroyer(_boundaries);
//     }
//
//     private void SetupInput()
//     {
//         _inputManager = new InputManager();
//         _inputManager.Subscribe(_keyboardInput);
//     }
//
//      
//      
//
//     private void SetupAsteroids()
//     {
//         var spawnProvider = new EdgeSpawnPositionProvider();
//
//         // Spawner created first — no waveManager needed
//         _asteroidSpawner = new AsteroidSpawner(
//             _poolManager.LargeAsteroidPool,
//             _poolManager.MediumAsteroidPool,
//             _poolManager.SmallAsteroidPool,
//             spawnProvider,
//             _boundaries,
//             _waveConfig);
//
//         // WaveManager subscribes to spawner in constructor
//         _waveManager = new WaveManager(
//             _asteroidSpawner,
//             _waveConfig);
//     }
//     private void StartGame()
//     {
//         if (_gameManager == null)
//         {
//             Debug.LogError("[GameInstaller] " +
//                            "GameManager not initialised.");
//             return;
//         }
//
//         _gameManager.StartGame();
//     }
// }
// }