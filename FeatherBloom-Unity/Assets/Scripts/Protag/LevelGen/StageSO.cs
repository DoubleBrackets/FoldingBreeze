using UnityEngine;

namespace Protag.LevelGen
{
    [CreateAssetMenu(fileName = "StageSO", menuName = "StageSO")]
    public class StageSO : ScriptableObject
    {
        [field: SerializeField]
        public string StageName { get; private set; }

        [field: SerializeField]
        public MapStage StagePrefab { get; private set; }
    }
}