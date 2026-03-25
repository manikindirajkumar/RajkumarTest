using System;


namespace RajkumarTest.Asteroid.Core
{
    public class ScoreSystem : IScoreSystem
    {
         
        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;
        
        public void AddScore(int points)
        {
            if (points <= 0) return;
            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);
        }

         

        public void Reset()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}