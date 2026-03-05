using Framework;
using Framework.GlobalServices;
using TMPro;
using UnityEngine;

namespace UI.Menus
{
    public class GameOverScene : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _scoreText;

        private void Start()
        {
            int score = ServiceLocator.GetService<ScoreService>().GetCurrentScore();
            _scoreText.text = $"{score}";
        }
    }
}