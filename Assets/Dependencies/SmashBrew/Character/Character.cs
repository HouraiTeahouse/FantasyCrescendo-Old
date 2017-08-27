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
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(MovementState))]
    public class Character : NetworkBehaviour, IRegistrar<ICharacterComponent> {

        public CharacterController Controller { get; private set; }
        public MovementState Movement { get; private set; }
        public StateController<CharacterState, CharacterStateContext> StateController { get; private set; }
        public CharacterStateContext Context { get; private set; }

        public CharacterControllerBuilder States {
            get { return _controller; }
        }

        ReadOnlyCollection<Hitbox> _hitboxes;
        ReadOnlyCollection<Hitbox> _hurtboxes;
        Dictionary<int, CharacterState> _stateMap;
        List<ICharacterComponent> _components;
        HashSet<object> _hitHistory;

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
            Controller = this.SafeGetComponent<CharacterController>();
            Movement = this.SafeGetComponent<MovementState>();
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
        /// LateUpdate is called every frame, if the Behaviour is enabled.
        /// It is called after all Update functions have been called.
        /// </summary>
        void LateUpdate() {
            if (!isLocalPlayer || SmashTimeManager.Paused)
                return;
            foreach (var component in _components)
                component.UpdateStateContext(Context);
            StateController.UpdateState(Context);
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

        public void ResetCharacter() {
            StateController.ResetState();
            _hitHistory.Clear();
            foreach (IResettable resetable in GetComponentsInChildren<IResettable>())
                resetable.OnReset();
        }

        void ChangeActive(bool active) {
            _isActive = active;
            gameObject.SetActive(active);
        }

        public override void OnStartServer() {
            Log.Error("HELLO");
            Assert.IsNotNull(StateController);
            StateController.OnStateChange += (b, a) => {
                if (isServer)
                    RpcSetState(a.AnimatorHash, 0f);
            };
        }

        [ClientRpc]
        void RpcSetState(int hash, float time) {
            CharacterState newState;
            if (!_stateMap.TryGetValue(hash, out newState)) {
                Log.Error("Server attempted to set state to one with hash {0}, which has no matching client state.", hash);
                return;
            }
            Assert.IsNotNull(newState);
            StateController.SetState(newState);
        }

    }

}
