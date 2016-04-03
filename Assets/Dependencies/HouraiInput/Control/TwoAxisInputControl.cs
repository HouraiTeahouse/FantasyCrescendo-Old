using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class TwoAxisInputControl {
        public static float StateThreshold = 0.0f;
        private bool lastState;


        internal TwoAxisInputControl() {
            Left = new OneAxisInputControl();
            Right = new OneAxisInputControl();
            Up = new OneAxisInputControl();
            Down = new OneAxisInputControl();
        }

        public float X { get; protected set; }
        public float Y { get; protected set; }

        public OneAxisInputControl Left { get; protected set; }
        public OneAxisInputControl Right { get; protected set; }
        public OneAxisInputControl Up { get; protected set; }
        public OneAxisInputControl Down { get; protected set; }

        public ulong UpdateTick { get; protected set; }


        public bool State { get; private set; }


        public bool HasChanged {
            get { return State != lastState; }
        }


        public Vector2 Vector {
            get { return new Vector2(X, Y); }
        }


        internal void Update(float x, float y, ulong updateTick) {
            lastState = State;

            X = x;
            Y = y;

            Left.UpdateWithValue(Mathf.Clamp01(-X), updateTick, StateThreshold);
            Right.UpdateWithValue(Mathf.Clamp01(X), updateTick, StateThreshold);

            if (HInput.InvertYAxis) {
                Up.UpdateWithValue(Mathf.Clamp01(-Y), updateTick, StateThreshold);
                Down.UpdateWithValue(Mathf.Clamp01(Y), updateTick, StateThreshold);
            }
            else {
                Up.UpdateWithValue(Mathf.Clamp01(Y), updateTick, StateThreshold);
                Down.UpdateWithValue(Mathf.Clamp01(-Y), updateTick, StateThreshold);
            }

            State = Up.State || Down.State || Left.State || Right.State;

            if (State != lastState) {
                UpdateTick = updateTick;
            }
        }


        public static implicit operator bool(TwoAxisInputControl control) {
            return control.State;
        }


        public static implicit operator Vector2(TwoAxisInputControl control) {
            return control.Vector;
        }


        public static implicit operator Vector3(TwoAxisInputControl control) {
            return new Vector3(control.X, control.Y);
        }
    }
}
