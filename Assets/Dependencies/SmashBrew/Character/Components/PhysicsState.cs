using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Physics State")]
    [RequireComponent(typeof(CharacterController))]
    public class PhysicsState : CharacterComponent {

        // Character Constrants
        [SerializeField]
        [Tooltip("How much the character weighs")]
        float _weight = 1.0f;

        [SerializeField]
        [Tooltip("How fast a charactter reaches their max fall speed, in seconds.")]
        float _gravity = 1.5f;

        public float Weight {
            get { return _weight;}
        }
        public float Gravity {
            get { return _gravity;}
        }

        public CharacterController CharacterController { get; private set; }

        HashSet<Collider> _ignoredColliders;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake() { 
            base.Awake();
            CharacterController = GetComponent<CharacterController>(); 
            _ignoredColliders = new HashSet<Collider>();
        }

        public override void Simulate(float deltaTime, 
                                      ref CharacterStateSummary state,
                                      ref InputContext input) {
            Vector3 originalPosition = transform.position;
            transform.position = state.Position;
            var acceleration = state.Acceleration + Vector2.down * Gravity;
            if (CharacterController.isGrounded)
                acceleration.y = 0;
            state.Velocity += acceleration * deltaTime;
            CharacterController.Move(state.Velocity * deltaTime);
            var grounded = IsGrounded(ref state);
            state.IsGrounded = grounded;
            if (grounded && state.Velocity.y < 0f) {
                var originalPos = transform.position;
                var collision = CharacterController.Move(Vector3.down * Config.Physics.GroundSnapDistance);
                if (collision == CollisionFlags.None)
                    transform.position = originalPos;
            }
            state.Position = transform.position;
            transform.position = originalPosition;
        }

        public override void ApplyState(ref CharacterStateSummary state) {
            transform.position = state.Position;
        }

        public override void UpdateStateContext(ref CharacterStateSummary summary, CharacterStateContext context) {
            context.IsGrounded = summary.IsGrounded;
        }

        public override void ResetState(ref CharacterStateSummary state) {
            state.Velocity = Vector2.zero;
            state.Acceleration = Vector2.zero;
        }

        bool IsGrounded(ref CharacterStateSummary state) {
            if (state.Velocity.y > 0)
                return false;
            var center = Vector3.zero;
            var radius = 1f;
            if (CharacterController != null) {
                center = CharacterController.center - Vector3.up * (CharacterController.height * 0.50f - CharacterController.radius * 0.5f);
                radius = CharacterController.radius * 0.75f;
            }
            return Physics.OverlapSphere(transform.TransformPoint(center), 
                                         radius, Config.Tags.StageMask, 
                                         QueryTriggerInteraction.Ignore)
                                         .Any(col => !_ignoredColliders.Contains(col));
        }


        public void IgnoreCollider(Collider collider, bool state) {
            Physics.IgnoreCollision(CharacterController, collider, state);
            if (state)
                _ignoredColliders.Add(collider);
            else
                _ignoredColliders.Remove(collider);
        }

        /// <summary>
        /// Callback to draw gizmos that are pickable and always drawn.
        /// </summary>
        void OnDrawGizmos() {
            var center = Vector3.zero;
            var radius = 1f;
            if (CharacterController != null) {
                center = CharacterController.center - Vector3.up * (CharacterController.height * 0.5f - CharacterController.radius * 0.5f);
                radius = CharacterController.radius * 0.75f;
                var diff = Vector3.up * (CharacterController.height * 0.5f - CharacterController.radius);
                using (Gizmo.With(Color.red)) {
                    var rad =  CharacterController.radius * transform.lossyScale.Max();
                    Gizmos.DrawWireSphere(transform.TransformPoint(CharacterController.center + diff), rad);
                    Gizmos.DrawWireSphere(transform.TransformPoint(CharacterController.center - diff), rad);
                }
            }
            using (Gizmo.With(Color.blue)) {
                Gizmos.DrawWireSphere(transform.TransformPoint(center), radius);
            }
        }

    }

}

