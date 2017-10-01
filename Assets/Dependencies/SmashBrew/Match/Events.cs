using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Matches {

    public abstract class MatchEvent {
        public Match Match;
    }

    public class MatchStarted : MatchEvent {
    }

    public class MatchCompleted : MatchEvent {
        public MatchResult Result;
        public Player Winner;
    }

    public class MatchResolved : MatchEvent {
        public MatchResult Result;
        public Player Winner;
    }

}
