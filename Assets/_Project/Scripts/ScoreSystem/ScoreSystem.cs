using System;
using UnityEngine;
using RajkumarTest.Asteroid.Core;

namespace RajkumarTest.Asteroid
{
    // ScoreSystem
    /// <summary>
    /// Tracks current score and persists high score
    /// via PlayerPrefs across sessions.
    /// Saves on Reset() — once per game, not per hit.
    /// </summary>
    public class ScoreSystem : IScoreSystem
    {
        private const string k_HighScoreKey = "HighScore";

        public int CurrentScore { get; private set; }
        public int HighScore    { get; private set; }

        public event Action<int> OnScoreChanged;

        public ScoreSystem()
        {
            HighScore = PlayerPrefs.GetInt(k_HighScoreKey, 0);
        }

        public void AddScore(int points)
        {
            if (points <= 0) return;

            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);
            TryUpdateHighScore(CurrentScore);
        }

        private void TryUpdateHighScore(int score)
        {
            if (score <= HighScore) return;
            HighScore = score;
        }

        public void Reset()
        {
            // Save before resetting — score still valid here
            PlayerPrefs.SetInt(k_HighScoreKey, HighScore);
            PlayerPrefs.Save();

            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}