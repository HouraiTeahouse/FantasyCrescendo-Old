using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    ///     A MatchRule that adds a time limit.
    ///     The match ends the instant the timer hits zero.
    ///     Note this rule does not determine a winner, only ends the Match.
    /// </summary>
    public sealed class TimeMatch : MatchRule {
        private Mediator _eventManager;
        [SerializeField] private readonly float _time = 180f;

        /// <summary>
        ///     The amount of time remaining in the Match, in seconds.
        /// </summary>
        public float CurrentTime { get; private set; }

        /// <summary>
        ///     Gets the winner of the Match. Null if the rule does not declare one.
        /// </summary>
        /// <remarks>TimeMatch doesn't determine winners, so this will always be null.</remarks>
        /// <returns>the winner of the match. Always null.</returns>
        public override Player GetWinner() {
            return null;
        }

        /// <summary>
        ///     Unity Callback. Called on object instantiation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _eventManager = GlobalMediator.Instance;
            _eventManager.Subscribe<MatchStartEvent>(OnMatchStart);
        }

        /// <summary>
        ///     Event callback. Called when the Match starts and ends.
        /// </summary>
        /// <param name="startEventArgs">the event parameters</param>
        private void OnMatchStart(MatchStartEvent startEventArgs) {
            CurrentTime = _time;
        }

        /// <summary>
        ///     Unity Callback. Called once every frame.
        /// </summary>
        private void Update() {
            CurrentTime -= Time.unscaledDeltaTime;
            if (CurrentTime <= 0)
                Match.FinishMatch();
        }
    }
}