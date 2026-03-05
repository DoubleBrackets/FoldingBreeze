using Framework.LevelLoading;
using Saving;
using UnityEngine;
using ValueSO.Core;

namespace Framework
{
    [CreateAssetMenu(fileName = "StartupConfig", menuName = "Framework/Startup Config")]
    public class StartupConfigSO : ScriptableObject
    {
        [field: Header("Time Scale")]

        [field: SerializeField]
        public FloatValueSO TimeScaleLerpFactor { get; private set; }

        [field: Header("Level Loading")]

        [field: SerializeField]
        public GameLevelSO StartupGameLevel { get; private set; }

        [field: SerializeField]
        public GameLevelSO GameplayLevel { get; private set; }

        [field: Header("Save")]

        [field: SerializeField]
        public SaveDataValueSOLoader SaveDataValueSOLoader { get; private set; }
    }
}