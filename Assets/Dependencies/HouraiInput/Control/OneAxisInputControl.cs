using System;
using UnityEngine;


namespace HouraiTeahouse.HouraiInput {
    public class OneAxisInputControl {
        public ulong UpdateTick { get; private set; }

        InputSource _thisSource;
        InputSource _lastSource;


        public void UpdateWithValue(float value, ulong updateTick, float stateThreshold) {
            if (UpdateTick > updateTick) {
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");
            }

            _lastSource = _thisSource;

            _thisSource.Set(value, stateThreshold);

            if (_thisSource != _lastSource) {
                UpdateTick = updateTick;
            }
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


        public static implicit operator bool(OneAxisInputControl control) {
            return control.State;
        }


        public static implicit operator float(OneAxisInputControl control) {
            return control.Value;
        }
    }
}
