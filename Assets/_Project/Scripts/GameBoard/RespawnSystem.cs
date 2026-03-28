using System.Collections;
using UnityEngine;
using VContainer;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Handles ship respawn after losing a life.
    /// Ship briefly flashes and is invincible during respawn.
    /// SpriteRenderer serialized in prefab Inspector.
    /// IHealthSystem injected by VContainer.
    /// </summary>
    public class RespawnSystem : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How long ship is invincible after respawn")]
        private float _invincibilityDuration = 3f;

        [SerializeField]
        [Tooltip("How fast ship flashes during invincibility")]
        private float _flashInterval = 0.2f;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private ShipController shipController;

        private IHealthSystem _healthSystem;

         

        [Inject]
        public void Construct(IHealthSystem healthSystem)
        {
            if (shipController == null)
            {
                Debug.LogError("[RespawnSystem] shipController not assigned in Inspector.");
                return;
            }
            
            _healthSystem = healthSystem;
            _healthSystem.OnLivesChanged += HandleLivesChanged;
        }

        private void OnDestroy()
        {
            if (_healthSystem != null)
                _healthSystem.OnLivesChanged -= HandleLivesChanged;
        }

         

        private void HandleLivesChanged(int lives)
        {
            if (lives > 0)
            {
                StartCoroutine(RespawnRoutine());
            }
            else
            {
                RemoveSpaceShipFromView();
            }
        }

        private IEnumerator RespawnRoutine()
        {
            RemoveSpaceShipFromView();
            yield return new WaitForSeconds(1f);
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            shipController?.SetActive(true);

            yield return StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            float elapsed = 0f;
            while (elapsed < _invincibilityDuration)
            {
                if (_spriteRenderer != null)
                    _spriteRenderer.enabled =
                        !_spriteRenderer.enabled;

                yield return new WaitForSeconds(_flashInterval);
                elapsed += _flashInterval;
            }

            if (_spriteRenderer != null)
                _spriteRenderer.enabled = true;

            shipController?.SetInvincible(false);
        }

        private void RemoveSpaceShipFromView()
        {
            shipController?.SetActive(false);
            shipController?.StopPhysics();
            shipController?.SetInvincible(true);
            _spriteRenderer.enabled = false;
        }
    }
}
