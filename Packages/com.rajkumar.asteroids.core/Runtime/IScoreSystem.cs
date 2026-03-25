using System;

namespace RajkumarTest.Asteroid.Core
{
    /// <summary>
    /// Tracks player score.
    /// Fires OnScoreChanged so UI can react
    /// without polling every frame.
    /// </summary>
    public interface IScoreSystem
    {
        int CurrentScore { get; }
        event Action<int> OnScoreChanged;
        void AddScore(int points);
        void Reset();
    }
}