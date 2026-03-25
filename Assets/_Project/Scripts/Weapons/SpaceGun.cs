using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Ship weapon system.
    /// Gets bullets from pool, launches them,
    /// enforces fire rate cooldown.
    ///
    /// Listens to IInputProvider.OnShoot —
    /// does not read input directly.
    /// </summary>
    public class SpaceGun : MonoBehaviour, ISpaceGun
    {
        // ── config ───────────────────────────────────────────────

        [SerializeField]
        [Tooltip("Seconds between each shot")]
        private float _fireRate = 0.3f;

        [SerializeField]
        [Tooltip("Where bullet spawns — tip of ship")]
        private Transform _muzzlePoint;

        // ── ISpaceGun ────────────────────────────────────────────

        public bool CanFire => _cooldownTimer <= 0f;

        // ── private fields ───────────────────────────────────────

        private IObjectPool<IBullet> _bulletPool;
        private IInputProvider _inputProvider;
        private float _cooldownTimer;
        private bool _isActive = true;

        // ── initialisation ───────────────────────────────────────

        public void Initialise(
            IObjectPool<IBullet> bulletPool,
            IInputProvider inputProvider)
        {
            if (bulletPool == null)
                throw new System.ArgumentNullException(
                    nameof(bulletPool));

            if (inputProvider == null)
                throw new System.ArgumentNullException(
                    nameof(inputProvider));

            _bulletPool    = bulletPool;
            _inputProvider = inputProvider;

            // Subscribe to shoot event
            _inputProvider.OnShoot += HandleShoot;
        }

        private void OnDestroy()
        {
            if (_inputProvider != null)
                _inputProvider.OnShoot -= HandleShoot;
        }

        // ── ISpaceGun implementation ─────────────────────────────

        public void TryFire(Vector3 position, Vector3 direction)
        {
            if (!CanFire) return;

            IBullet bullet = _bulletPool.Get();
            bullet.Launch(position, direction);

            // Start cooldown
            _cooldownTimer = _fireRate;
        }
        public void SetActive(bool active)
        {
            _isActive = active;
        }
        // ── Unity lifecycle ──────────────────────────────────────

        private void Update()
        {
            // Count down cooldown timer
            if (_cooldownTimer > 0f)
                _cooldownTimer -= Time.deltaTime;
        }

        // ── private handlers ─────────────────────────────────────
        private void HandleShoot()
        {
            if (!_isActive) return;
            TryFire(_muzzlePoint.position, _muzzlePoint.up);
        }
    }
}