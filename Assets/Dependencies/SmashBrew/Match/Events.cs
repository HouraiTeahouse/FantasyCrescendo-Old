using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Matches {

    public class MatchStarted {
        public Match Match;
    }

    public class MatchCompleted {
        public Match Match;
        public MatchResult Result;
        public Player Winner;
    }

    public class MatchResolved {
        public Match Match;
    }

}
