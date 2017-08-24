using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.SmashBrew.Characters {

    public class AnimationState : CharacterNetworkComponent {

        const int StateLayer = 0;
        const float UpdateRate = 0.2f;

        [SerializeField]
        float _transitionTime = 0.1f;

        [SerializeField]
        PlayableDirector _director;

        Dictionary<int, CharacterState> _states;
        double _loopTime;
        double _stateTime;
        float _updateTimer;

        protected override void Awake() {
            base.Awake();
            _updateTimer = 0f;
            _stateTime = 0f;
            if (Character != null)
                Character.StateController.OnStateChange += (b, a) => {
                    PlayState(a);
                    _loopTime = 0f;
                };
        }

        void Start() {
            BuildStateMap();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            UpdateStateTime();
            if (!hasAuthority)
                return;
            _updateTimer += Time.unscaledDeltaTime;
            if (_updateTimer <= UpdateRate)
                return;
            _updateTimer = 0f;
            CmdChangeState(CurrentState.AnimatorHash, (float)(_director.time / _director.duration));
        }

        void UpdateStateTime() {
            if (_director == null)
                return;
            if (_director.time < _stateTime) {
                // Time has looped at least once
                _stateTime += (_director.duration - _loopTime) + _director.time;
            } else {
                _stateTime += Math.Abs(_director.time - _loopTime);
                _loopTime = _director.time;
            }
        }

        public override void OnStartAuthority() {
            // Update server when the local client has changed.
            Character.StateController.OnStateChange += (b, a) => CmdChangeState(a.AnimatorHash, 0f);
        }

        [Command]
        void CmdChangeState(int animHash, float normalizedTime) {
            //TODO(james7132): Make proper verfications server side
            if (_states == null)
                return;
            if (!_states.ContainsKey(animHash)) {
                Log.Error("Client attempted to set state to one with hash {0}, which has no matching server state.", animHash);
                return;
            }
            RpcChangeState(animHash, normalizedTime);
        }

        [ClientRpc]
        void RpcChangeState(int animHash, float normalizedTime) {
            //TODO(james7132): This gives local players complete control over their networked state. The server should be authoritative on this.
            if (hasAuthority)
                return;
            CharacterState newState;
            if (!_states.TryGetValue(animHash, out newState)) {
                Log.Error("Server attempted to set state to one with hash {0}, which has no matching client state.", animHash);
                return;
            }
            Character.StateController.SetState(newState);
            PlayState(newState);
        }

        void PlayState(CharacterState state, float time = 0f) {
            if (_director == null || state.Data.Timeline == null)
                return;
            _director.Play(state.Data.Timeline);
            _director.time = (time % 1f) * _director.duration;
            _director.Evaluate();
            _stateTime = time;
        }

        void BuildStateMap() {
            if (Character == null)
                return;
            _states = Character.StateController.States.ToDictionary(s => s.AnimatorHash);
        }

        public override void UpdateStateContext(CharacterStateContext context) {
            if (_director == null || _director.duration == 0f)
                return;
            context.NormalizedAnimationTime = (float)(_stateTime / _director.duration);
        }

        public override void ResetState() {
            //TODO(james7132): implement
        }

    }

}

