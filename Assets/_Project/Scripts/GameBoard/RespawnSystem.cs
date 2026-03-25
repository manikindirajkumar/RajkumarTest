using System.Collections;
using System.Threading;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Handles ship respawn after losing a life.
    /// Ship briefly flashes and is invincible during respawn.
    /// Attached to Player GameObject.
    /// </summary>
    public class RespawnSystem : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How long ship is invincible after respawn")]
        private float _invincibilityDuration = 3f;

        [SerializeField]
        [Tooltip("How fast ship flashes during invincibility")]
        private float _flashInterval = 0.2f;

        private IHealthSystem _healthSystem;
        private ShipMovement  _shipMovement;
        private SpriteRenderer _spriteRenderer;
        private bool _isInvincible;

        public void Initialise(
            IHealthSystem healthSystem,
            ShipMovement shipMovement, SpriteRenderer spriteRenderer)
        {
            if (healthSystem == null)
                throw new System.ArgumentNullException(
                    nameof(healthSystem));  
            if(spriteRenderer == null)
                throw new System.ArgumentNullException(
                    nameof(spriteRenderer));
            
            _healthSystem   = healthSystem;
            _shipMovement   = shipMovement;
            _spriteRenderer = spriteRenderer;

            _healthSystem.OnLivesChanged += HandleLivesChanged;
        }

        private void OnDestroy()
        {
            if (_healthSystem != null)
                _healthSystem.OnLivesChanged -= HandleLivesChanged;
        }

        private void HandleLivesChanged(int lives)
        {
            // Only respawn if still alive
            if (lives > 0)
                StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            // Disable ship briefly
            _shipMovement.SetActive(false);

            yield return new WaitForSeconds(1f);

            // Reset position to center
            _shipMovement.transform.position = Vector3.zero;
            _shipMovement.transform.rotation = Quaternion.identity;
            _shipMovement.SetActive(true);

            // Flash for invincibility duration
            yield return StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _isInvincible = true;
            _shipMovement.SetInvincible(true);

            float elapsed = 0f;
            while (elapsed < _invincibilityDuration)
            {
                if (_spriteRenderer != null)
                    _spriteRenderer.enabled =
                        !_spriteRenderer.enabled;

                yield return new WaitForSeconds(_flashInterval);
                elapsed += _flashInterval;
            }

            // Ensure visible at end
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = true;

            _isInvincible = false;
            _shipMovement.SetInvincible(false);
        }
    }
}