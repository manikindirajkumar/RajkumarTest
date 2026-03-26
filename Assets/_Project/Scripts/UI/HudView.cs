using TMPro;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    /// <summary>
    /// Displays score, lives and wave number.
    /// Subscribes to system events —
    /// updates only when values change.
    /// Uses OnWaveStarted — not OnWaveCompleted —
    /// so display updates when new wave begins.
    /// </summary>
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _livesText;
        [SerializeField] private TextMeshProUGUI _waveText;

        private IScoreSystem  _scoreSystem;
        private IHealthSystem _healthSystem;
        private IAsteroidManager  _asteriodManager;

        public void Initialise(
            IScoreSystem  scoreSystem,
            IHealthSystem healthSystem,
            IAsteroidManager  asteriodManager)
        {
            if (scoreSystem  == null)
                throw new System.ArgumentNullException(
                    nameof(scoreSystem));
            if (healthSystem == null)
                throw new System.ArgumentNullException(
                    nameof(healthSystem));
            if (asteriodManager  == null)
                throw new System.ArgumentNullException(
                    nameof(asteriodManager));

            _scoreSystem  = scoreSystem;
            _healthSystem = healthSystem;
            _asteriodManager  = asteriodManager;

            // Subscribe to events
            _scoreSystem.OnScoreChanged   += UpdateScore;
            _healthSystem.OnLivesChanged  += UpdateLives;
            _asteriodManager.OnWaveStarted    += UpdateAsteriod;

            // Set initial values after StartGame() is called
            UpdateScore(_scoreSystem.CurrentScore);
            UpdateLives(_healthSystem.CurrentLives);
            UpdateAsteriod(_asteriodManager.CurrentWave);
        }

        private void OnDestroy()
        {
            if (_scoreSystem  != null)
                _scoreSystem.OnScoreChanged  -= UpdateScore;
            if (_healthSystem != null)
                _healthSystem.OnLivesChanged -= UpdateLives;
            if (_asteriodManager  != null)
                _asteriodManager.OnWaveStarted   -= UpdateAsteriod;
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

        private void UpdateAsteriod(int wave)
        {
            if (_waveText != null)
                _waveText.text = $"WAVE: {wave}";
        }
    }
}
