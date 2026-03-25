using UnityEngine;
using RajkumarTest.Asteroid.Core;
using TMPro;
namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Displays score, lives and wave number.
    /// Subscribes to system events —
    /// updates only when values change.
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private TextMeshProUGUI _waveText;

        private IScoreSystem  _scoreSystem;
        private IHealthSystem _healthSystem;
        private IWaveManager  _waveManager;

        public void Initialise(
            IScoreSystem scoreSystem,
            IHealthSystem healthSystem,
            IWaveManager waveManager)
        {
            _scoreSystem  = scoreSystem;
            _healthSystem = healthSystem;
            _waveManager  = waveManager;

            // Subscribe to events
            _scoreSystem.OnScoreChanged   += UpdateScore;
            _healthSystem.OnLivesChanged  += UpdateLives;
            _waveManager.OnWaveCompleted += UpdateWave;
            _waveManager.OnWaveStarted += UpdateWave;    

            // Set initial values
            UpdateScore(_scoreSystem.CurrentScore);
            UpdateLives(_healthSystem.CurrentLives);
            UpdateWave(_waveManager.CurrentWave);
        }

        private void OnDestroy()
        {
            if (_scoreSystem  != null)
                _scoreSystem.OnScoreChanged  -= UpdateScore;
            if (_healthSystem != null)
                _healthSystem.OnLivesChanged -= UpdateLives;
            if (_waveManager != null)
            {
                _waveManager.OnWaveCompleted -= UpdateWave;
                _waveManager.OnWaveStarted -= UpdateWave;    
            }
        }

        private void UpdateScore(int score)
        {
            if (_scoreText != null)
                _scoreText.text = $"SCORE: {score}";
        }

        private void UpdateLives(int lives)
        {
            if (_livesText != null)
                _livesText.text = $"LIVES: {lives}";
        }

        private void UpdateWave(int wave)
        {
            if (_waveText != null)
                _waveText.text = $"WAVE: {wave}";
        }
    }
}