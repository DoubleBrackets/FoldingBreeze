using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Protag.LevelGen.StageRoster
{
    public class StageSelector
    {
        private readonly StageRosterSO _stageRoster;

        private Queue<StageSO> _stageQueue;

        private StageSO _lastChosenStage;

        public StageSelector(StageRosterSO stageRoster)
        {
            _stageRoster = stageRoster;
            _stageQueue = new Queue<StageSO>(_stageRoster.FixedOrderStartStages);
        }

        public StageSO GetNextStage()
        {
            if (_stageQueue.Count == 0)
            {
                _stageQueue.Enqueue(GetRandomStageEntry().Stage);
            }

            return _stageQueue.Dequeue();
        }

        private RosterEntry GetRandomStageEntry()
        {
            if (_stageRoster.RandomSelectionPool.Count == 1)
            {
                return _stageRoster.RandomSelectionPool[0];
            }

            List<RosterEntry> enabledEntries =
                _stageRoster.RandomSelectionPool.FindAll(entry => entry.Enabled && entry.Stage != _lastChosenStage);

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
                    _lastChosenStage = entry.Stage;
                    return entry;
                }
            }

            throw new InvalidOperationException("No stage entries available.");
        }
    }
}