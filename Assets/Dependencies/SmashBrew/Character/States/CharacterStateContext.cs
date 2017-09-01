using Newtonsoft.Json;
using UnityEngine;
using HouraiTeahouse.SmashBrew.States;
using UnityEngine.Networking;

namespace HouraiTeahouse.SmashBrew.Characters {

    public struct ButtonContext {

        public bool LastFrame;
        public bool Current;

        public bool WasPressed {
            get { return !LastFrame && Current; }
        }

        public bool WasReleased {
            get { return LastFrame && !Current; }
        }

        public void Update(bool value) {
            LastFrame = Current;
            Current = value;
        }

        public override int GetHashCode() {
            return 2 * LastFrame.GetHashCode() + Current.GetHashCode();
        }

    }

    public enum Direction {
        Neutral = 0,
        Up,
        Down,
        Left,
        Right
    }

    public struct DirectionalInput {

        public const float DeadZone = 0.3f;
        public Vector2 Value;
        public Direction Direction {
            get {
                var absX = Mathf.Abs(Value.x);
                var absY = Mathf.Abs(Value.y);
                if (absX > absY) {
                    if (Value.x < -DeadZone)
                        return Direction.Left;
                    if (Value.x > DeadZone)
                        return Direction.Right;
                }
                if (absX <= absY) {
                    if (Value.y < -DeadZone)
                        return Direction.Down;
                    if (Value.y > DeadZone)
                        return Direction.Up;
                }
                return Direction.Neutral;
            }
        }

        public static implicit operator DirectionalInput(Vector2 value) {
            return new DirectionalInput { Value = value };
        }
    }

    public struct InputContext {
        public DirectionalInput Movement;
        public DirectionalInput Smash;
        public ButtonContext Attack;
        public ButtonContext Special;
        public ButtonContext Jump;
        public ButtonContext Shield;

        public InputContext(InputSlice old, InputSlice current) {
            Movement = new DirectionalInput {
                Value = current.Movement
            };
            Smash = new DirectionalInput {
                Value = current.Smash
            };
            Attack = new ButtonContext {
                LastFrame = old.Attack,
                Current = current.Attack
            };
            Special = new ButtonContext {
                LastFrame = old.Special,
                Current = current.Special
            };
            Jump = new ButtonContext {
                LastFrame = old.Jump,
                Current = current.Jump
            };
            Shield = new ButtonContext {
                LastFrame = old.Shield,
                Current = current.Shield
            };
        }

        public override int GetHashCode() {
            return Shield.GetHashCode() + 
                3 * Jump.GetHashCode() +
                7 * Special.GetHashCode() +
                11 * Attack.GetHashCode() +
                17 * Smash.GetHashCode() +
                19 * Movement.GetHashCode();
        }

        public override string ToString() {
            return GetHashCode().ToString();
        }
    }


    public class CharacterStateContext {
        public float NormalizedAnimationTime { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsGrabbingLedge { get; set; }
        public bool IsHit { get; set; }
        public float ShieldHP { get; set; }
        public bool CanJump { get; set; }
        public float Hitstun { get; set; }
        // The direction the character is facing in.
        // Will be positive if facing to the right.
        // Will be negative if facing to the left.
        public float Direction { get; set; }
        public InputContext Input;

        public override string ToString() {
            return "t:{0} d:{5} g:{1} l:{2} h:{3} s:{4} i:{6}".With(
                NormalizedAnimationTime, IsGrounded, IsGrabbingLedge,
                IsHit, ShieldHP, Direction, Input
            );
        }

        public CharacterStateContext Clone() {
            return MemberwiseClone() as CharacterStateContext;
        }

    }

}

