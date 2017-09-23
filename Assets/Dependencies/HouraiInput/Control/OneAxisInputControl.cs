using System;

namespace HouraiTeahouse.HouraiInput {

    public class OneAxisInputControl {

        InputState lastState;
        InputState thisState;

        public ulong UpdateTick { get; private set; }

        public bool State => thisState.State;
        public bool LastState => lastState.State;

        public float Value => thisState.Value;
        public float LastValue => lastState.Value;

        public bool HasChanged => thisState != lastState;
        public bool IsPressed => thisState.State;
        public bool WasPressed => thisState && !lastState;
        public bool WasReleased => !thisState && lastState;

        public static implicit operator bool(OneAxisInputControl control) => control.State;
        public static implicit operator float(OneAxisInputControl control) => control.Value;

        public void UpdateWithValue(float value, ulong updateTick, float stateThreshold) {
            if (UpdateTick > updateTick)
                throw new InvalidOperationException("A control cannot be updated with an earlier tick.");
            lastState = thisState;
            thisState.Set(value, stateThreshold);
            if (thisState != lastState)
                UpdateTick = updateTick;
        }

    }

}