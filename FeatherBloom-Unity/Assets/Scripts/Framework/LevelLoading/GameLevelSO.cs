using NaughtyAttributes;
using UnityEngine;

namespace Framework.LevelLoading
{
    [CreateAssetMenu(fileName = "GameLevel", menuName = "Framework/Game Level")]
    public class GameLevelSO : ScriptableObject
    {
        [field: SerializeField]
        [field: Scene]
        public string SceneName { get; private set; }

        [field: SerializeField]
        public GameLevelTypes LevelType { get; private set; }

        [field: SerializeField]
        [field: TextArea(3, 10)]
        public string LevelText { get; set; }
    }
}