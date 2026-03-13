using Framework;
using Framework.GlobalServices;
using LevelGen.StageRoster;
using TMPro;
using UnityEngine;
using ValueSO.Core;

namespace UI.Menus
{
    public class GameOverScene : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _scoreText;

        [SerializeField]
        private FloatValueSO _heightValueSO;

        [SerializeField]
        private BoolValueSO _didBeatGameValueSO;

        [SerializeField]
        private StageRosterSO _stageRosterSO;

        private void Start()
        {
            int score = ServiceLocator.GetService<ScoreService>().GetCurrentScore();

            if (_didBeatGameValueSO.Value)
            {
                _scoreText.text = "???";
            }
            else
            {
                float height = Mathf.Min(_heightValueSO.Value, _stageRosterSO.TowerHeight - 1);
                _scoreText.text = $"{height:F0} / {_stageRosterSO.TowerHeight}m";
            }
        }
    }
}