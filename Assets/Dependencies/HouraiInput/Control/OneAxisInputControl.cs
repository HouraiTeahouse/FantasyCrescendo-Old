using System;

namespace HouraiTeahouse.HouraiInput {
    public class OneAxisInputControl {
        public ulong UpdateTick { get; private set; }

        private InputState _currentState;
        private InputState _lastState;

        public void UpdateWithValue(float value, ulong updateTick, float stateThreshold) {
            if (UpdateTick > updateTick)
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");

            _lastState = _currentState;

            _currentState.Set(value, stateThreshold);

            if (_currentState != _lastState) {
                UpdateTick = updateTick;
            }
        }

        public bool State {
            get { return _currentState.State; }
        }

        public bool LastState {
            get { return _lastState.State; }
        }

        public float Value {
            get { return _currentState.Value; }
        }

        public float LastValue {
            get { return _lastState.Value; }
        }

        public bool HasChanged {
            get { return _currentState != _lastState; }
        }

        public bool IsPressed {
            get { return _currentState.State; }
        }

        public bool WasPressed {
            get { return _currentState && !_lastState; }
        }

        public bool WasReleased {
            get { return !_currentState && _lastState; }
        }

        public static implicit operator bool(OneAxisInputControl control) {
            return control.State;
        }

        public static implicit operator float(OneAxisInputControl control) {
            return control.Value;
        }
    }
}
