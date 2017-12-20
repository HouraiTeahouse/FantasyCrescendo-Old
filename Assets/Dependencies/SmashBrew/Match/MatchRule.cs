using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Matches {

    /// <summary> An abstract class  to define a Match Rule. These instances are searched for before the start of a Match to
    /// define the rules of a match. They run as normal MonoBehaviours, but are regularly polled for </summary>
    [RequireComponent(typeof(Match))]
    public abstract class MatchRule : NetworkBehaviour {

        [SyncVar(hook = "LogActive"), SerializeField, ReadOnly]
        bool _isActive;

        public bool IsActive {
            get { return _isActive; }
            private set { _isActive = value; }
        }

        /// <summary> A refernce to the central Match object. </summary>
        protected static Match Match { get; private set; }

        protected virtual void Awake() {
            Match = this.SafeGetComponent<Match>();
        }

        internal void Initialize(MatchConfig config) {
            IsActive = CheckActive(config);
            if (IsActive)
                OnInitialize(config);
        }

        protected abstract bool CheckActive(MatchConfig config);
        protected virtual void OnInitialize(MatchConfig config) { }
        internal virtual void OnMatchTick() { }

        void LogActive(bool isActive) {
            IsActive = isActive;
            if (IsActive)
                Debug.LogFormat("Match rule enabled: {0}", this);
        }

    }

}
