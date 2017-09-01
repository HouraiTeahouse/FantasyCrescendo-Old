using HouraiTeahouse.SmashBrew.States;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    /// <summary> General character class for handling the physics and animations of individual characters </summary>
    [DisallowMultipleComponent]
    public class Character : NetworkBehaviour, IRegistrar<ICharacterComponent>, IDataComponent<Player> {

        public InputState Input { get; private set; }
        public StateController<CharacterState, CharacterStateContext> StateController { get; private set; }
        public CharacterStateContext Context { get; private set; }
        public CharacterStateSummary State;

        public CharacterControllerBuilder States {
            get { return _controller; }
        }

        const int kInputHistorySize = 4;

        ReadOnlyCollection<Hitbox> _hitboxes;
        ReadOnlyCollection<Hitbox> _hurtboxes;
        Dictionary<int, CharacterState> _stateMap;
        List<ICharacterComponent> _components;
        HashSet<object> _hitHistory;
        InputSlice _input;
        CharacterStateHistory _history;

#pragma warning disable 414
        [SyncVar(hook = "ChangeActive")]
        bool _isActive;
#pragma warning restore 414

        [SerializeField]
        CharacterControllerBuilder _controller;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            gameObject.tag = Config.Tags.PlayerTag;
            gameObject.layer = Config.Tags.CharacterLayer;
            InitializedComponents();
            InitializeStates();
            InitializeHitboxes();
            ResetCharacter();
            _history = new CharacterStateHistory(this);
        }

        void InitializeStates() {
            if (_controller == null)
                throw new InvalidOperationException("Cannot start a character without a State Controller!");
            StateController = _controller.BuildCharacterControllerImpl(
                new StateControllerBuilder<CharacterState, CharacterStateContext>());
            Context = new CharacterStateContext();
            _stateMap = StateController.States.ToDictionary(s => s.AnimatorHash);
            if (Debug.isDebugBuild)
                StateController.OnStateChange += (b, a) => 
                    Log.Debug("{0} changed states: {1} => {2}".With(name, b.Name, a.Name));
        }

        void InitializedComponents() {
            Input = this.SafeGetComponent<InputState>();
            _components = new List<ICharacterComponent>();
        }

        void InitializeHitboxes() {
            Assert.IsNotNull(StateController);
            _hitHistory = new HashSet<object>();
            var hitboxComponets = GetComponentsInChildren<Hitbox>(true);
            _hitboxes = new ReadOnlyCollection<Hitbox>(hitboxComponets.ToArray());
            _hurtboxes = new ReadOnlyCollection<Hitbox>(
                hitboxComponets.Where(h => h.CurrentType != Hitbox.Type.Offensive).ToArray());
            var typeMap = new Dictionary<ImmunityType, Hitbox.Type> {
                {ImmunityType.Normal, Hitbox.Type.Damageable},
                {ImmunityType.Intangible, Hitbox.Type.Intangible},
                {ImmunityType.Invincible, Hitbox.Type.Invincible}
            };
            StateController.OnStateChange += (b, a) => {
                foreach (var hitbox in _hitboxes)
                    hitbox.ResetState();
                _hitHistory.Clear();
            };
            StateController.OnStateChange += (b, a) => {
                var hitboxType = Hitbox.Type.Damageable;
                if (!typeMap.TryGetValue(a.Data.DamageType, out hitboxType))
                    return;
                foreach (var hurtbox in _hurtboxes)
                    hurtbox.CurrentType = hitboxType;
            };
        }

        public bool CheckHistory(object obj) {
            var result = _hitHistory.Contains(obj);
            if (!result)
                _hitHistory.Add(obj);
            return result;
        }

        internal CharacterStateSummary Simulate(float dt, CharacterStateSummary state, InputContext input) {
            foreach (var component in _components) {
                Assert.IsNotNull(component);
                component.Simulate(dt, ref state, ref input);
            }
            return state;
        }

        internal void ApplyState(ref CharacterStateSummary state) {
            State = state;
            foreach (var component in _components) {
                Assert.IsNotNull(component);
                component.ApplyState(ref state);
            }
        }

         internal void UpdateStateContext(ref CharacterStateSummary state, InputContext input) {
            Context.Input = input;
            foreach (var component in _components) {
                component.UpdateStateContext(ref state, Context);
            }
        }

        void IRegistrar<ICharacterComponent>.Register(ICharacterComponent component) {
            if (_components.Contains(Argument.NotNull(component)))
                return;
            _components.Add(component);
        }

        bool IRegistrar<ICharacterComponent>.Unregister(ICharacterComponent component) {
            return _components.Remove(component);
        }

        public void ResetAllHitboxes() {
            foreach (Hitbox hitbox in Hitboxes.IgnoreNulls())
                hitbox.ResetState();
        }

        public CharacterState GetState(int hash) {
            CharacterState state;
            if (_stateMap.TryGetValue(hash, out state))
                return state;
            return null;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() { 
            _isActive = true; 
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        void OnDisable() { 
            _isActive = false; 
        }

        /// <summary>
        /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
        /// </summary>
        void FixedUpdate() {
            if (!isLocalPlayer || SmashTimeManager.Paused)
                return;    
            InputSlice currentInput = Input.GetInput();
            State = _history.Advance(currentInput, State);
            ApplyState(ref State);
            if (isServer)
                RpcUpdateState(_history.LatestTimestamp, State);
            else
                CmdSendInput(_history.LatestTimestamp, currentInput);
        }

        internal CharacterStateSummary Advance(CharacterStateSummary state, 
                                               float deltaTime, 
                                               InputContext input) {
            var currentState = State;
            State = state;
            var controllerState = GetState(state.StateHash);
            StateController.SetState(controllerState ?? StateController.DefaultState);
            State.StateHash = StateController.CurrentState.AnimatorHash;
            State = Simulate(deltaTime, State, input);
            UpdateStateContext(ref State, input);
            StateController.UpdateState(Context);
            State.StateHash = StateController.CurrentState.AnimatorHash;
            state = State;
            State = currentState;
            return state;
        }

        [Command]
        void CmdSendInput(int timestamp, InputSlice input) {
            State = Advance(State, Time.fixedDeltaTime, new InputContext(_input, input));
            // State = _history.AcknowledgeState(timestamp, State);
            RpcUpdateState(timestamp, State);
            _input = input;
        }

        [ClientRpc]
        void RpcUpdateState(int timestamp, CharacterStateSummary state) {
            if (isLocalPlayer)
                State = _history.AcknowledgeState(timestamp, state);
            else
                ApplyState(ref state);
        }

        /// <summary> 
        /// Gets an immutable collection of hitboxes that belong to the character.
        /// </summary>
        public ReadOnlyCollection<Hitbox> Hitboxes {
            get { return _hitboxes; }
        }

        /// <summary> 
        /// Gets an immutable collection of hurtboxes that belong to the character.
        /// </summary>
        public ReadOnlyCollection<Hitbox> Hurtboxes {
            get { return _hurtboxes; }
        }

        void IDataComponent<Player>.SetData(Player data) {
            gameObject.name = "Player {0} ({1},{2})".With(data.ID, data.Selection.Character.name, data.Selection.Pallete);
        }

        public CharacterStateSummary ResetState(CharacterStateSummary state) {
            foreach (var component in _components)
                component.ResetState(ref state);
            state.StateHash = StateController.DefaultState.AnimatorHash;
            return state;
        }

        public void ResetCharacter() {
            StateController.ResetState();
            _hitHistory.Clear();
            State = ResetState(new CharacterStateSummary());
            foreach (IResettable resetable in GetComponentsInChildren<IResettable>())
                resetable.OnReset();
        }

        void ChangeActive(bool active) {
            _isActive = active;
            gameObject.SetActive(active);
        }

    }

}
