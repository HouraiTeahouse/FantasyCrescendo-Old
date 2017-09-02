using System;
using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Stage;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    [DisallowMultipleComponent]
    [AddComponentMenu("Smash Brew/Character/Movement State")]
    public class MovementState : CharacterComponent {

        public enum CharacterFacingMode {
            Rotation, Scale
        }

        CharacterController MovementCollider { get; set; }

        public event Action OnJump;

        [Header("Constants")]
        [SerializeField]
        CharacterFacingMode _characterFacingMode;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The maximum downward velocity of the character under normal conditions")]
        float _maxFallSpeed = 5f;

        [SerializeField]
        [Range(2f, 10f)]
        [Tooltip("The downward velocity of the character while fast falling")]
        float _fastFallSpeed = 5f;

        [SerializeField]
        float[] _jumpPower = { 5f, 10f };

        [SerializeField]
        Transform _ledgeTarget;

        [SerializeField]
        float _rotationOffset;

        [SerializeField]
        Transform _skeletonRoot;

        public float MaxFallSpeed {
            get { return _maxFallSpeed; }
        }

        public float FastFallSpeed {
            get { return _fastFallSpeed; }
        }

        public CharacterFacingMode FacingMode {
            get { return _characterFacingMode; }
        }

        public int MaxJumpCount {
            get { return _jumpPower != null ? _jumpPower.Length : 0; }
        }

        public Transform LedgeTarget {
            get { return _ledgeTarget; }
        }

        bool _direction;
        float _grabTime;

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start() {
            MovementCollider = this.SafeGetComponent<CharacterController>();
            OnChangeDirection(_direction, true);
            _ledgeTarget = this.CachedGetComponent(_ledgeTarget, () => transform);
            if (Character == null)
                return;
            var stateController = Character.StateController;
            var states = Character.States;
            stateController.OnStateChange += (b, a) => {
                var characterState = Character.State;
                if (states.Jump == a)
                    Jump(ref characterState);
                if (states.LedgeRelease == a)  {
                    characterState.IsFastFalling = false;
                    characterState.CurrentLedge = null;
                }
                if (states.LedgeAttack == b || states.LedgeClimb == b) {
                    characterState.CurrentLedge = null;

                    var originalPosition = transform.position;
                    transform.position = _skeletonRoot.position;
                    _skeletonRoot.position = transform.position;
                    MovementCollider.Move(Vector3.down * MovementCollider.height);
                    Character.StateController.ChangeState(Character.States.Idle);
                    characterState.Position = transform.position;
                    transform.position = originalPosition;
                }
                if (states.EscapeForward == b)
                    characterState.Direction = !characterState.Direction;
                Character.State = characterState;
            };
        }

        void SnapToLedge(ref CharacterStateSummary state) {
            Assert.IsNotNull(state.CurrentLedge);
            state.IsFastFalling = false;
            var offset = LedgeTarget.position - transform.position;
            state.Position = state.CurrentLedge.transform.position - offset;
            ResetJumps(ref state);
        }

        void LedgeMovement(ref CharacterStateSummary state, ref InputContext input) {
            ResetJumps(ref state);
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || 
                input.Smash.Value.y <= -DirectionalInput.DeadZone) {
                state.CurrentLedge = null;
            } else {
                SnapToLedge(ref state);
            }
        }

        void Jump(ref CharacterStateSummary state) {
            state.CurrentLedge = null;
            OnJump.SafeInvoke();
            state.Velocity.y = _jumpPower[MaxJumpCount - state.JumpCount];
            state.JumpCount--;
        }

        bool GetKeys(params KeyCode[] keys) {
            return keys.Any(Input.GetKey);
        }

        bool GetKeysDown(params KeyCode[] keys) {
            return keys.Any(Input.GetKeyDown);
        }

        float ButtonAxis(bool neg, bool pos) {
            var val = neg ? -1f : 0f;
            return val + (pos ? 1f : 0f);
        }

        public override void Simulate(float deltaTime, 
                                      ref CharacterStateSummary state,
                                      ref InputContext input) {
            if (Mathf.Approximately(deltaTime, 0) || state.Hitstun > 0)
                return;
            var currentState = GetState(state.StateHash);
            if (currentState.Data.MovementType == MovementType.Locked) {
                state.Velocity = Vector2.zero;
                return;
            }
            // If currently hanging from a edge
            if (state.CurrentLedge != null) {
                LedgeMovement(ref state, ref input);
                // TODO(james7132): Make this non-dependent on component state
                if (Time.time > _grabTime + Config.Player.MaxLedgeHangTime) {
                    state.CurrentLedge = null;
                } else {
                    return;
                }
            } 
            var movementInput = input.Movement.Value;
            if (state.IsGrounded) {
                state.IsFastFalling = false;
                if (state.JumpCount != MaxJumpCount)
                    state.JumpCount = MaxJumpCount;
                if (movementInput.x > DirectionalInput.DeadZone)
                    state.Direction = true;
                if (movementInput.x < -DirectionalInput.DeadZone)
                    state.Direction = false;
            } else {
                state.IsFastFalling |= input.Smash.Value.y < -DirectionalInput.DeadZone;
                LimitFallSpeed(ref state);
            }
            ApplyControlledMovement(ref state, currentState, movementInput);
        }

        public override void ResetState(ref CharacterStateSummary state) {
            ResetJumps(ref state);
        }

        public override void ApplyState(ref CharacterStateSummary state) {
            OnChangeDirection(state.Direction);
        }

        public override void UpdateStateContext(ref CharacterStateSummary state, CharacterStateContext context) {
            context.Direction = state.Direction ? 1.0f : -1.0f;
            context.IsGrabbingLedge = state.CurrentLedge != null;
            context.CanJump = state.JumpCount > 0 && state.JumpCount <= MaxJumpCount;
        }

        void ApplyControlledMovement(ref CharacterStateSummary state, 
                                     CharacterState currentState,
                                     Vector2 movementInput) {
            switch(currentState.Data.MovementType) {
                case MovementType.Normal:
                    var dir = 1f;
                    dir = state.Direction ? 1f : -1f;
                    state.Velocity.x =  dir * Mathf.Abs(movementInput.x) * currentState.Data.MovementSpeed.Max;
                    break;
                case MovementType.DirectionalInfluenceOnly:
                    state.Velocity.x = movementInput.x * currentState.Data.MovementSpeed.Max;
                    break;
            }
        }

        void LimitFallSpeed(ref CharacterStateSummary state) {
            if (state.IsFastFalling)
                state.Velocity.y = -FastFallSpeed;
            else if (state.Velocity.y < -MaxFallSpeed)
                state.Velocity.y = -MaxFallSpeed;
        }

        void OnControllerColliderHit(ControllerColliderHit hit) {
            var platform = hit.gameObject.GetComponent<Platform>();
            if (platform != null)
                platform.CharacterCollision(MovementCollider);
        }

        void ResetJumps(ref CharacterStateSummary state) { 
            state.JumpCount = MaxJumpCount; 
        }

        void SetDirection(bool direction, ref CharacterStateSummary state) {
            if (GetState(state.StateHash).Data.CanTurn)
                state.Direction = direction;
        }

        void OnChangeDirection(bool direction, bool force = false) {
            if (_direction == direction && !force)
                return;
            _direction = direction;
            if (FacingMode == CharacterFacingMode.Rotation) {
                var euler = transform.localEulerAngles;
                euler.y = (direction ? 0 : 180) + _rotationOffset;
                transform.localEulerAngles = euler;
            } else {
                var scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (direction ? 1 : -1);
                transform.localScale = scale;
            }
        }

    }

}
