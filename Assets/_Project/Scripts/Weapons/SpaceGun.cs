using UnityEngine;
using VContainer;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Ship weapon system.
    /// Dependencies injected by VContainer via [Inject].
    /// Handles fire rate cooldown and bullet launching.
    /// </summary>
    public class SpaceGun : MonoBehaviour, ISpaceGun
    {
        [SerializeField]
        [Tooltip("Seconds between each shot")]
        private float _fireRate = 0.3f;

        [SerializeField]
        [Tooltip("Where bullet spawns — tip of ship")]
        private Transform _muzzlePoint;

        public bool CanFire => _cooldownTimer <= 0f;
        
        private IInputProvider       _inputProvider;
        private IGameManager         _gameManager;
        private PoolManager          _poolManager;
        private float                _cooldownTimer;
        private bool                 _isActive = true;

         

        [Inject]
        public void Construct(
            PoolManager poolManager,      // ← registered in VContainer ✅
            IInputProvider inputProvider,
            IGameManager gameManager)
        {
            // Get pool from PoolManager after it's initialised
            _poolManager   = poolManager;
            _inputProvider = inputProvider;
            _inputProvider.OnShoot += HandleShoot;
            gameManager.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
                _inputProvider.OnShoot -= HandleShoot;
            
            if (_gameManager != null)
                _gameManager.OnStateChanged -= HandleStateChanged;
        }

         

        public void TryFire(Vector3 position, Vector3 direction)
        {
            if (!CanFire || !_isActive) return;

            // Access via PoolManager — always available ✅
            IBullet bullet = _poolManager.BulletPool?.Get();
            if (bullet == null) return;

            bullet.Launch(position, direction);
            _cooldownTimer = _fireRate;
        }

        public void SetActive(bool active)
        {
            _isActive = active;
        }

         

        private void Update()
        {
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
        }

         

        private void HandleShoot()
        {
            if (_muzzlePoint == null) return;

            // Get pool lazily — guaranteed initialised by this point
            TryFire(
                _muzzlePoint.position,
                _muzzlePoint.up);
        }
        private void HandleStateChanged(GameState state)
        {
            // React to state — no one needs to tell us
            _isActive = state == GameState.Playing;
        }

    }
}
