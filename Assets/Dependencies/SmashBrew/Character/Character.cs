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
        public Player Player { get; private set; }

        public CharacterControllerBuilder States {
            get { return _controller;}
        }

        public const int kInputHistorySize = 3;

        ReadOnlyCollection<Hitbox> _hitboxes;
        ReadOnlyCollection<Hitbox> _hurtboxes;
        Dictionary<int, CharacterState> _stateMap;
        List<ICharacterComponent> _components;
        HashSet<object> _hitHistory;
        InputSlice _input;
        CharacterStateHistory _history;
        CharacterStateSummary _lastServerState;

        int _inputHistoryIndex;
        PlayerInputSet _cachedInputSet;

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
            _cachedInputSet = new PlayerInputSet {
                Inputs = new InputSlice[kInputHistorySize]
            };
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
                    Debug.Log(string.Format("{0} changed states: {1} => {2}", name, a.Name, b.Name));
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
                hitboxComponets.Where(h => h.CurrentType != HitboxType.Offensive).ToArray());
            var typeMap = new Dictionary<ImmunityType, HitboxType> {
                {ImmunityType.Normal, HitboxType.Damageable},
                {ImmunityType.Intangible, HitboxType.Intangible},
                {ImmunityType.Invincible, HitboxType.Invincible}
            };
            StateController.OnStateChange += (b, a) => {
                foreach (var hitbox in _hitboxes)
                    hitbox.ResetState();
                _hitHistory.Clear();
            };
            StateController.OnStateChange += (b, a) => {
                var hitboxType = HitboxType.Damageable;
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
            if (SmashTimeManager.Paused)
                return;
            if (isLocalPlayer) {
                LocalPlayerUpdate();
            } else {
                RemotePlayerUpdate();
            }
        }

        void LocalPlayerUpdate() {
            InputSlice currentInput = Input.GetInput();
            State = _history.Advance(currentInput, State);
            ApplyState(ref State);
            _cachedInputSet.Inputs[_inputHistoryIndex] = currentInput;
            _inputHistoryIndex++;
            if (_inputHistoryIndex < kInputHistorySize)
                return;
            _inputHistoryIndex = 0;
            if (isServer)  {
                RpcUpdateState(_history.LatestTimestamp, State, currentInput);
                return;
            }
            Assert.IsTrue(_history.LatestTimestamp - kInputHistorySize >= 0);
            _cachedInputSet.Timestamp = _history.LatestTimestamp - kInputHistorySize;
            connectionToServer.Send(SmashNetworkMessages.PlayerInput, _cachedInputSet);
        }

        void RemotePlayerUpdate() {
            InputSlice currentInput = _input == null ? new InputSlice() : _input.Clone();
            if (isServer) {
                _lastServerState = Advance(_lastServerState, Time.fixedDeltaTime, new InputContext(currentInput, currentInput));
                ApplyState(ref _lastServerState);
            } else {
                State = _history.Advance(currentInput, State);
                ApplyState(ref State);
            }
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

        [Server]
        internal void ServerRecieveInput(PlayerInputSet inputSet) {
            InputSlice previous = _input;
            foreach (var input in inputSet.Inputs) {
                State = Advance(State, Time.fixedDeltaTime, new InputContext(previous, input));
                Debug.LogWarning(Convert.ToString(input.buttons, 2).PadLeft(8, '0'));
                _input = input;
            }
            RpcUpdateState((uint)(inputSet.Timestamp + inputSet.Inputs.Length), State, _input);
        }

        [ClientRpc]
        void RpcUpdateState(uint timestamp, CharacterStateSummary state, InputSlice latestInput) {
            State = _history.ReconcileState(timestamp, state);
            _lastServerState = State;
            ApplyState(ref State);
            if (!isLocalPlayer)
                _input = latestInput;
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
            Player = data;
            var selection = data.Selection;
            gameObject.name = string.Format("Player {0} ({1},{2})", data.ID, selection.Character.name, selection.Pallete);
            if (isServer)
                RpcSetPlayerId((byte)data.ID);
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
            State = ResetState(State.Reset());
            _lastServerState = State;
            foreach (IResettable resetable in GetComponentsInChildren<IResettable>())
                resetable.OnReset();
        }

        void ChangeActive(bool active) {
            _isActive = active;
            gameObject.SetActive(active);
        }

        [ClientRpc]
        void RpcSetPlayerId(byte id) {
            if (_cachedInputSet == null)
                _cachedInputSet = new PlayerInputSet {
                    Inputs = new InputSlice[kInputHistorySize]
                };
            _cachedInputSet.PlayerId = id;
        }

    }

}
