using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ← replace UnityEngine.UI

namespace RajkumarTest.Asteroid.Core
{
    public class GameOverView : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _finalScoreText;  // ✅
        [SerializeField] private Button _restartButton;
        // removed _highScoreText — not implemented yet

        private IGameManager _gameManager;
        private IScoreSystem _scoreSystem;

        public void Initialise(
            IGameManager gameManager,
            IScoreSystem scoreSystem)
        {
            if (gameManager == null)
                throw new System.ArgumentNullException(
                    nameof(gameManager));
            if (scoreSystem == null)
                throw new System.ArgumentNullException(
                    nameof(scoreSystem));

            _gameManager = gameManager;
            _scoreSystem = scoreSystem;

            _gameManager.OnStateChanged += HandleStateChanged;

            if (_panel != null)
                _panel.SetActive(false);

            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnDestroy()
        {
            if (_gameManager != null)
                _gameManager.OnStateChanged -= HandleStateChanged;

            if (_restartButton != null)
                _restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        private void OnRestartClicked()
        {
            if (_panel != null)
                _panel.SetActive(false);

            _gameManager?.RestartGame();
        }

        private void HandleStateChanged(GameState state)
        {
            if (state != GameState.GameOver) return;

            if (_panel != null)
                _panel.SetActive(true);

            if (_finalScoreText != null)
                _finalScoreText.text =
                    $"SCORE: {_scoreSystem.CurrentScore}";
        }
    }
}