using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary> A MatchRule that adds a time limit. The match ends the instant the timer hits zero. Note this rule does not
    /// determine a winner, only ends the Match. </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Matches/Time Match")]
    public sealed class TimeMatch : MatchRule {

        [SyncVar, SerializeField, ReadOnly]
        float _currentTime;

        /// <summary> The amount of time remaining in the Match, in seconds. </summary>
        public float CurrentTime {
            get { return _currentTime; }
            private set {
                if(hasAuthority)
                    _currentTime = value;
            }
        }

        protected override bool CheckActive(MatchConfig config) => config.Time > 0;

        protected override void OnInitialize(MatchConfig config)  {
            CurrentTime = config.Time;
        }

        internal override void OnMatchTick() {
            CurrentTime -= Time.unscaledDeltaTime;
            if (CurrentTime <= 0)
                Match.Finish(MatchResult.Tie, null);
        }

    }

}
