using System;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class InputControl {

        public static readonly InputControl Null = new InputControl("NullInputControl");

        internal float? PreValue;

        // This is for internal use only and is not always set.
        internal float? RawValue;
        InputState tempState;

        ulong zeroTick;

        InputControl(string handle) {
            Handle = handle;
            Sensitivity = 1.0f;
            LowerDeadZone = 0.0f;
            UpperDeadZone = 1.0f;
        }

        public InputControl(string handle, InputTarget target) {
            Handle = handle;
            Target = target;

            IsButton = (target >= InputTarget.Action1 && target <= InputTarget.Action4)
                || (target >= InputTarget.Button0 && target <= InputTarget.Button19);
        }

        public string Handle { get; protected set; }
        public InputTarget Target { get; protected set; }

        public ulong UpdateTick { get; protected set; }

        public float Sensitivity { get; set; }
        public float LowerDeadZone { get; set; }
        public float UpperDeadZone { get; set; }

        /// <summary> 
        /// Is this control a button? 
        /// </summary>
        public bool IsButton { get; protected set; }

        internal bool IsOnZeroTick => UpdateTick == zeroTick;

        public InputState State { get; private set; }
        public InputState LastState { get; private set; }

        public bool HasChanged => State != LastState;
        public bool IsPressed => State.State;
        public bool WasPressed => State && !LastState;
        public bool WasReleased => !State && LastState;
        public bool IsNull => this == Null;
        public bool IsNotNull => this != Null;

        public InputTarget? Obverse {
            get {
                switch (Target) {
                    case InputTarget.LeftStickX:
                        return InputTarget.LeftStickY;
                    case InputTarget.LeftStickY:
                        return InputTarget.LeftStickX;
                    case InputTarget.RightStickX:
                        return InputTarget.RightStickY;
                    case InputTarget.RightStickY:
                        return InputTarget.RightStickX;
                    default:
                        return null;
                }
            }
        }

        public void UpdateWithState(bool state, ulong updateTick) {
            if (IsNull)
                throw new InvalidOperationException("A null control cannot be updated.");

            if (UpdateTick > updateTick)
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");

            tempState.Set(state || tempState.State);
        }

        public void UpdateWithValue(float value, ulong updateTick) {
            if (IsNull)
                throw new InvalidOperationException("A null control cannot be updated.");

            if (UpdateTick > updateTick)
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");

            if (Mathf.Abs(value) > Mathf.Abs(tempState.Value))
                tempState.Set(value);
        }

        internal void PreUpdate(ulong updateTick) {
            RawValue = null;
            PreValue = null;

            LastState = State;
            tempState.Reset();
        }

        internal void PostUpdate(ulong updateTick) {
            State = tempState;
            if (State != LastState)
                UpdateTick = updateTick;
        }

        internal void SetZeroTick() { zeroTick = UpdateTick; }

        public override string ToString() { return "[InputControl: Handle={0}, Value={1}]".With(Handle, State.Value); }

        public static implicit operator bool(InputControl control) => control.State;
        public static implicit operator float(InputControl control) => control.State;

    }

}