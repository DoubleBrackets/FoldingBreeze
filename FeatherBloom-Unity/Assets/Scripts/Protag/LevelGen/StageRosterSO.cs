using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Protag.LevelGen
{
    [CreateAssetMenu(fileName = "StageRosterSO", menuName = "Stage Roster SO")]
    public class StageRosterSO : ScriptableObject
    {
        [Serializable]
        public struct RosterEntry
        {
            public StageSO Stage;
            public int Weight;
            public bool Enabled;
            public MapStage Prefab => Stage.StagePrefab;
        }

        [field: SerializeField]
        public List<RosterEntry> RosterEntries { get; private set; }

        [field: SerializeField]
        private int _startingStageIndex;

        public RosterEntry GetRandomStageEntry(StageSO exclude)
        {
            List<RosterEntry> enabledEntries =
                RosterEntries.FindAll(entry => entry.Enabled && entry.Stage != exclude);

            var totalWeight = 0;
            foreach (RosterEntry entry in enabledEntries)
            {
                totalWeight += entry.Weight;
            }

            int randomValue = Random.Range(0, totalWeight);
            var cumulativeWeight = 0;

            foreach (RosterEntry entry in enabledEntries)
            {
                cumulativeWeight += entry.Weight;
                if (randomValue < cumulativeWeight)
                {
                    return entry;
                }
            }

            throw new InvalidOperationException("No stage entries available.");
        }

        public RosterEntry GetStartingStageEntry()
        {
            if (_startingStageIndex < 0 || _startingStageIndex >= RosterEntries.Count)
            {
                throw new IndexOutOfRangeException("Starting stage index is out of range.");
            }

            return RosterEntries[_startingStageIndex];
        }
    }
}