using UnityEngine;

namespace Framework.GlobalServices
{
    public class ScoreService
    {
        private int _currentScore;

        public void AddScore(int points)
        {
            _currentScore += points;
            Debug.Log($"Score added: {points}. Current Score: {_currentScore}");
        }

        public int GetCurrentScore()
        {
            return _currentScore;
        }

        public void ResetScore()
        {
            _currentScore = 0;
        }
    }
}