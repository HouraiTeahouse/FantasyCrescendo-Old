using System;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class InputControl {
        public static readonly InputControl Null = new InputControl("NullInputControl");

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

        InputSource _thisSource;
        InputSource _lastSource;
        InputSource _tempSource;

        ulong zeroTick;


        private InputControl(string handle) {
            Handle = handle;
            Sensitivity = 1.0f;
            LowerDeadZone = 0.0f;
            UpperDeadZone = 1.0f;
        }


        public InputControl(string handle, InputTarget target) {
            Handle = handle;
            Target = target;

            IsButton = (target >= InputTarget.Action1 && target <= InputTarget.Action4) ||
                       (target >= InputTarget.Button0 && target <= InputTarget.Button19);
        }


        public void UpdateWithState(bool state, ulong updateTick) {
            if (IsNull) {
                throw new InvalidOperationException("A null control cannot be updated.");
            }

            if (UpdateTick > updateTick) {
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");
            }

            _tempSource.Set(state || _tempSource.State);
        }


        public void UpdateWithValue(float value, ulong updateTick) {
            if (IsNull) {
                throw new InvalidOperationException("A null control cannot be updated.");
            }

            if (UpdateTick > updateTick) {
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");
            }

            if (Mathf.Abs(value) > Mathf.Abs(_tempSource.Value)) {
                _tempSource.Set(value);
            }
        }


        internal void PreUpdate(ulong updateTick) {
            RawValue = null;
            PreValue = null;

            _lastSource = _thisSource;
            _tempSource.Reset();
        }


        internal void PostUpdate(ulong updateTick) {
            _thisSource = _tempSource;
            if (_thisSource != _lastSource) {
                UpdateTick = updateTick;
            }
        }


        internal void SetZeroTick() {
            zeroTick = UpdateTick;
        }


        internal bool IsOnZeroTick {
            get { return UpdateTick == zeroTick; }
        }


        public bool State {
            get { return _thisSource.State; }
        }


        public bool LastState {
            get { return _lastSource.State; }
        }


        public float Value {
            get { return _thisSource.Value; }
        }


        public float LastValue {
            get { return _lastSource.Value; }
        }


        public bool HasChanged {
            get { return _thisSource != _lastSource; }
        }


        public bool IsPressed {
            get { return _thisSource.State; }
        }


        public bool WasPressed {
            get { return _thisSource && !_lastSource; }
        }


        public bool WasReleased {
            get { return !_thisSource && _lastSource; }
        }


        public bool IsNull {
            get { return this == Null; }
        }


        public bool IsNotNull {
            get { return this != Null; }
        }


        public override string ToString() {
            return string.Format("[InputControl: Handle={0}, Value={1}]", Handle, Value);
        }


        public static implicit operator bool(InputControl control) {
            return control.State;
        }


        public static implicit operator float(InputControl control) {
            return control.Value;
        }


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

        // This is for internal use only and is not always set.
        internal float? RawValue;
        internal float? PreValue;
    }
}
