using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public struct CharacterStateSummary {
                                                // Total: 49 bytes
        public Vector2 Position;                // 2 * 4 = 8 bytes
        public Vector2 Velocity;                // 2 * 4 = 8 bytes
        public Vector2 Acceleration;            // 2 * 4 = 8 bytes
        public bool Direction;                  // 1 bit

        public int JumpCount;                   // 4 bytes
        public bool IsGrounded;                 // 1 bit
        public bool IsFastFalling;              // 1 bit
        public NetworkIdentity CurrentLedge;    // 8 bytes

        public int StateHash;                   // 4 bytes
        public float StateTime;                 // 4 bytes

        public float Damage;                    // 4 bytes
        public float Hitstun;                   // 4 bytes

        public float ShieldHealth;              // 4 bytes

        public byte Stocks;                     // 1 byte
    }

    public interface ICharacterComponent {

        // A Character State has two main components:
        //      Constants - Per character constants for configuration
        //      Variables - Variable properties that change over time.
        //                  Should generally need to be synced across the network.

        void Simulate(float deltaTime, 
                      ref CharacterStateSummary state,
                      ref InputContext input);
        
        void ApplyState(ref CharacterStateSummary summary);
        
        // Resets the state's variables
        // Called on any need for reset, this includes Character death.
        void ResetState(ref CharacterStateSummary state);

        void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context);

    }

    public abstract class CharacterComponent : MonoBehaviour, ICharacterComponent {

        public Character Character { get; private set; }

        protected CharacterState CurrentState => 
            (Character != null && Character.StateController != null) ? Character.StateController.CurrentState : null;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
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
                                     ref CharacterStateSummary state,
                                     ref InputContext input) {
        }

        public virtual void ResetState(ref CharacterStateSummary state) {
        }

        public virtual void ApplyState(ref CharacterStateSummary state) {
        }

        public virtual void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context) {
        }

    }

}
