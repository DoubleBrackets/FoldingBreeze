using UnityEngine;

namespace Services
{
    public class ScoreService : MonoBehaviour
    {
        public static ScoreService Instance { get; private set; }

        private int _currentScore;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

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