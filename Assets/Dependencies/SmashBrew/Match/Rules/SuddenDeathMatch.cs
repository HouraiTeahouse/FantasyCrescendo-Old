using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Matches {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Sudden Death Match")]
    public sealed class SuddenDeathMatch : MatchRule {

        protected override bool CheckActive(MatchConfig config) => false;

    }

}
