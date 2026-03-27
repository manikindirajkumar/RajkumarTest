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
        // ── scene references ─────────────────────────────────────

        [Header("Camera")]
        [SerializeField] private Camera _gameCamera;

        [Header("Input")]
        [SerializeField] private KeyboardInputProvider _keyboardInput;

        [Header("Config")]
        [SerializeField] private WaveConfig _waveConfig;

        [Header("UI")]
        [SerializeField] private HUDView      _hudView;
        [SerializeField] private GameOverView _gameOverView;

         
        // ── VContainer registration ──────────────────────────────

        protected override void Configure(IContainerBuilder builder)
        {
            // ── Camera ───────────────────────────────────────────
            builder.RegisterInstance(_gameCamera);

            // ── Preloader — retrieved via ServiceLocator ──────────
            var preloader = ServiceLocator.Get<Preloader>();
            builder.RegisterInstance(preloader);

            
            // ── Boundaries ───────────────────────────────────────
            // GameBoardBoundary receives Camera via constructor
            builder.Register<GameBoardBoundary>(Lifetime.Singleton).As<IBoundaries>();

            // BoundaryHandler receives IBoundaries via constructor
            builder.Register<BoundaryHandler>(Lifetime.Singleton).As<IBoundaryHandler>();

            // BoundaryDestroyer — separate interface, no conflict
            builder.Register<BoundaryDestroyer>(Lifetime.Singleton).As<IBulletBoundaryHandler>();

            // ── Input ─────────────────────────────────────────────
            builder.RegisterInstance(_keyboardInput).As<IInputProvider>();

            builder.Register<InputManager>(Lifetime.Singleton);
            

            // ── Config ────────────────────────────────────────────
            builder.RegisterInstance(_waveConfig).As<IWaveConfig>();

            // ── Systems ───────────────────────────────────────────
            int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
            builder.Register<ScoreSystem>(Lifetime.Singleton).WithParameter("savedHighScore", savedHighScore).As<IScoreSystem>();
             

            builder.Register<HealthSystem>(Lifetime.Singleton).As<IHealthSystem>().WithParameter("initialLives", 3);

            // // ── Asteroid Systems ──────────────────────────────────
            builder.Register<AsteroidSpawner>(Lifetime.Singleton).As<IAsteroidSpawner>();
            
            builder.Register<AsteroidManager>(Lifetime.Singleton).As<IAsteroidManager>();
            
            builder.Register<GameManager>(Lifetime.Singleton).As<IGameManager>();

            // ── Pool Manager ──────────────────────────────────────
            builder.Register<PoolManager>(Lifetime.Singleton);

            // ── Player Factory ────────────────────────────────────
            builder.Register<PlayerFactory>(Lifetime.Singleton);

            // ── UI ────────────────────────────────────────────────
            builder.RegisterInstance(_hudView);
            builder.RegisterInstance(_gameOverView);

            // ── Entry Point ───────────────────────────────────────
            builder.RegisterEntryPoint<GameBootstrapper>();
        }
    }
}
