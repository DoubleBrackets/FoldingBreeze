using Services;
using TMPro;
using UnityEngine;

namespace Protag.Menus
{
    public class GameOverScene : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _scoreText;

        private void Start()
        {
            int score = ScoreService.Instance.GetCurrentScore();
            _scoreText.text = $"{score}";
        }
    }
}