using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public struct CharacterStateSummary {
        public InputContext Input;

        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public bool Direction;
        public bool IsFastFalling;
        public NetworkIdentity CurrentLedge;

        public int JumpCount;
        public bool IsGrounded;

        public int StateHash;
        public float StateTime;

        public float Damage;
        public float Hitstun;

        public float ShieldHealth;
    }

    public interface ICharacterComponent {

        // A Character State has two main components:
        //      Constants - Per character constants for configuration
        //      Variables - Variable properties that change over time.
        //                  Should generally need to be synced across the network.

        void Simulate(float deltaTime, 
                      ref CharacterStateSummary state);
        
        void ApplyState(ref CharacterStateSummary summary);
        
        // Resets the state's variables
        // Called on any need for reset, this includes Character death.
        void ResetState(ref CharacterStateSummary state);

        void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context);

    }

    public abstract class CharacterNetworkComponent : NetworkBehaviour, ICharacterComponent {

        protected Character Character { get; private set; }

        protected CharacterState CurrentState {
            get { return (Character != null && Character.StateController != null) ? Character.StateController.CurrentState : null; }
        }

        protected virtual void Awake() {
            var registrar = GetComponentInParent<IRegistrar<ICharacterComponent>>();
            if (registrar != null)
                registrar.Register(this);
            Character = registrar as Character;
        }

        protected CharacterState GetState(int stateHash) {
            return Character != null ? Character.GetState(stateHash) : null;
        }

        public virtual void Simulate(float deltaTime,
                                     ref CharacterStateSummary previousState) {
        }

        public virtual void ResetState(ref CharacterStateSummary state) {
        }

        public virtual void ApplyState(ref CharacterStateSummary state) {
        }

        public virtual void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context) {
        }

    }

    public abstract class CharacterComponent : BaseBehaviour, ICharacterComponent {

        protected Character Character { get; private set; }
        protected CharacterState CurrentState {
            get { return (Character != null && Character.StateController != null) ? Character.StateController.CurrentState : null; }
        }

        protected override void Awake() {
            base.Awake();
            var registrar = GetComponentInParent<IRegistrar<ICharacterComponent>>();
            if (registrar != null)
                registrar.Register(this);
            Character = registrar as Character;
        }

        protected CharacterState GetState(int stateHash) {
            return Character != null ? Character.GetState(stateHash) : null;
        }

        public virtual void Simulate(float deltaTime,
                                     ref CharacterStateSummary previousState) {
        }

        public virtual void ResetState(ref CharacterStateSummary state) {
        }

        public virtual void ApplyState(ref CharacterStateSummary state) {
        }

        public virtual void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context) {
        }

    }

}
