using UnityEngine;
using VContainer;
using VContainer.Unity;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// VContainer composition root.
    /// Registers all dependencies and their lifetimes.
    /// All registrations are synchronous —
    /// prefabs pre-cached in LoadingScene via Addressables.
    /// </summary>
    public class GameLifetimeScope : LifetimeScope
    {
         

        [Header("Camera")]
        [SerializeField] private Camera _gameCamera;

        [Header("Input")]
        [SerializeField] private KeyboardInputProvider _keyboardInput;

        [Header("Config")]
        [SerializeField] private WaveConfig _waveConfig;

        [Header("UI")]
        [SerializeField] private HUDView      _hudView;
        [SerializeField] private GameOverView _gameOverView;

         
         

        protected override void Configure(IContainerBuilder builder)
        {
          
            builder.RegisterInstance(_gameCamera);

             
            var preloader = ServiceLocator.Get<Preloader>();
            builder.RegisterInstance(preloader);

            
             
            // GameBoardBoundary receives Camera via constructor
            builder.Register<GameBoardBoundary>(Lifetime.Singleton).As<IBoundaries>();

            // BoundaryHandler receives IBoundaries via constructor
            builder.Register<BoundaryHandler>(Lifetime.Singleton).As<IBoundaryHandler>();

            // BoundaryDestroyer — separate interface, no conflict
            builder.Register<BoundaryDestroyer>(Lifetime.Singleton).As<IBulletBoundaryHandler>();

             
            builder.RegisterInstance(_keyboardInput).As<IInputProvider>();

            builder.Register<InputManager>(Lifetime.Singleton);
            

             
            builder.RegisterInstance(_waveConfig).As<IWaveConfig>();

             
            builder.Register<ScoreSystem>(Lifetime.Singleton).As<IScoreSystem>();
             

            builder.Register<HealthSystem>(Lifetime.Singleton).As<IHealthSystem>().WithParameter("initialLives", 3);

             
            
            builder.Register<GameManager>(Lifetime.Singleton).As<IGameManager>();

             
            builder.Register<PoolManager>(Lifetime.Singleton);

             
            builder.Register<PlayerFactory>(Lifetime.Singleton);

             
            builder.RegisterInstance(_hudView);
            builder.RegisterInstance(_gameOverView);

             
            builder.RegisterEntryPoint<GameBootstrapper>();
        }
    }
}
