using System.Collections.Generic;
using UnityEngine;

namespace Protag.LevelGen.StageRoster
{
    [CreateAssetMenu(fileName = "StageRosterSO", menuName = "Stage Roster SO")]
    public class StageRosterSO : ScriptableObject
    {
        [field: SerializeField]
        public List<RosterEntry> RandomSelectionPool { get; private set; }

        [field: SerializeField]
        public List<StageSO> FixedOrderStartStages { get; private set; }
    }
}