using System;

namespace Protag.LevelGen.StageRoster
{
    [Serializable]
    public struct RosterEntry
    {
        public StageSO Stage;
        public int Weight;
        public bool Enabled;
        public MapStage Prefab => Stage.StagePrefab;
    }
}