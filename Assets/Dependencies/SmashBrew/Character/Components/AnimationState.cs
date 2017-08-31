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

        [SerializeField]
        PlayableDirector _director;

        Dictionary<int, CharacterState> _states;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            _director = this.CachedGetComponent(_director, () => GetComponentInChildren<PlayableDirector>());
            _director.timeUpdateMode = DirectorUpdateMode.Manual;
            if (Character != null)
                Character.StateController.OnStateChange += (b, a) => {
                    Character.State.StateTime = 0f;
                    PlayState(a);
                };
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            BuildStateMap();
        }

        void BuildStateMap() {
            if (Character == null)
                return;
            _states = Character.StateController.States.ToDictionary(s => s.AnimatorHash);
        }

        void PlayState(CharacterState state, float time = 0f) {
            if (_director == null || state.Data.Timeline == null)
                return;
            if (state.Data.Timeline != _director.playableAsset)
                _director.Play(state.Data.Timeline);
            _director.time = time % _director.duration;
            _director.Evaluate();
        }

        public override void Simulate(float deltaTime, ref CharacterStateSummary state) {
            state.StateTime += deltaTime;
        }

        public override void ApplyState(ref CharacterStateSummary state) {
            PlayState(GetState(state.StateHash), state.StateTime);
        }

        public override void UpdateStateContext(ref CharacterStateSummary state, CharacterStateContext context) {
            if (_director == null || _director.duration == 0f)
                return;
            context.NormalizedAnimationTime = (float)(state.StateTime / GetDuration(ref state));
        }

        double GetDuration(ref CharacterStateSummary state) {
            return Character.GetState(state.StateHash).Data.Timeline.duration;
        }

    }

}

