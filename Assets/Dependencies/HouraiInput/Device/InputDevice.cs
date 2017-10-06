using System;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class InputDevice {

        public static readonly InputDevice Null = new InputDevice("NullInputDevice");

        static readonly int ControlCount = Enum.GetValues(typeof(InputTarget)).Length;

        internal int SortOrder = int.MaxValue;

        public InputDevice(string name) {
            Name = name;
            Meta = "";

            LastChangeTick = 0;

            int numInputControlTypes = ControlCount;
            Controls = new InputControl[numInputControlTypes];

            LeftStick = new TwoAxisInputControl();
            RightStick = new TwoAxisInputControl();
            DPad = new TwoAxisInputControl();
        }

        public string Name { get; protected set; }
        public string Meta { get; protected set; }

        public ulong LastChangeTick { get; protected set; }

        public InputControl[] Controls { get; protected set; }

        public TwoAxisInputControl LeftStick { get; protected set; }
        public TwoAxisInputControl RightStick { get; protected set; }
        public TwoAxisInputControl DPad { get; protected set; }

        public InputControl this[InputTarget target] {
            get { return GetControl(target); }
         }

        Vector2 DPadVector {
            get {
                float x = DPad.Left.State ? -DPad.Left : DPad.Right;
                float t = DPad.Up.State ? DPad.Up : -DPad.Down;
                float y = HInput.InvertYAxis ? -t : t;
                return new Vector2(x, y).normalized;
            }
        }

        public virtual bool IsSupportedOnThisPlatform {
            get { return true; }
        }

        public virtual bool IsKnown {
            get { return true; }
        }

        public bool MenuWasPressed {
            get {
                return GetControl(InputTarget.Back).WasPressed || GetControl(InputTarget.Start).WasPressed
                    || GetControl(InputTarget.Select).WasPressed || GetControl(InputTarget.System).WasPressed
                    || GetControl(InputTarget.Pause).WasPressed || GetControl(InputTarget.Menu).WasPressed;
            }
        }

        public InputControl AnyButton {
            get {
                foreach (InputControl control in Controls.IgnoreNulls()) {
                    if (control.IsButton && control.IsPressed)
                        return control;
                }
                return InputControl.Null;
            }
        }

        public InputControl GetControl(InputTarget inputTarget) {
            return Controls[(int) inputTarget] ?? InputControl.Null;
        }

        public InputControl AddControl(InputTarget inputTarget, string handle) {
            var inputControl = new InputControl(handle, inputTarget);
            Controls[(int) inputTarget] = inputControl;
            return inputControl;
        }

        protected void UpdateWithState(InputTarget inputTarget, bool state, ulong updateTick) {
            GetControl(inputTarget).UpdateWithState(state, updateTick);
        }

        protected void UpdateWithValue(InputTarget inputTarget, float value, ulong updateTick) {
            GetControl(inputTarget).UpdateWithValue(value, updateTick);
        }

        internal virtual void Update(ulong updateTick, float deltaTime) {
            // Implemented by subclasses.
        }

        internal void PostUpdate(ulong updateTick, float deltaTime) {
            // Apply post-processing to controls.
            foreach (InputControl control in Controls.IgnoreNulls()) {
                if (control.RawValue != null)
                    control.UpdateWithValue(control.RawValue.Value, updateTick);
                else if (control.PreValue != null)
                    control.UpdateWithValue(ProcessAnalogControlValue(control, deltaTime), updateTick);

                control.PostUpdate(updateTick);

                if (control.HasChanged)
                    LastChangeTick = updateTick;
            }

            // Update two-axis controls.
            LeftStick.Update(LeftStick.X, LeftStick.Y, updateTick);
            RightStick.Update(RightStick.X, RightStick.Y, updateTick);

            Vector2 dpv = DPadVector;
            DPad.Update(dpv.x, dpv.y, updateTick);
        }

        float ProcessAnalogControlValue(InputControl control, float deltaTime) {
            float analogValue = control.PreValue.Value;

            InputTarget? obverseTarget = control.Obverse;
            if (obverseTarget.HasValue) {
                InputControl obverseControl = GetControl(obverseTarget.Value);
                if (obverseControl.PreValue.HasValue) {
                    analogValue = ApplyCircularDeadZone(analogValue,
                        obverseControl.PreValue.Value,
                        control.LowerDeadZone,
                        control.UpperDeadZone);
                } else {
                    analogValue = control.ApplyDeadZone(analogValue);
                }
            } else {
                analogValue = control.ApplyDeadZone(analogValue);
            }

            return ApplySmoothing(analogValue, control.LastState, deltaTime, control.Sensitivity);
        }

        static float ApplyCircularDeadZone(float axisValue1, float axisValue2, float lowerDeadZone, float upperDeadZone) {
            var axisVector = new Vector2(axisValue1, axisValue2);
            float magnitude = Mathf.InverseLerp(lowerDeadZone, upperDeadZone, axisVector.magnitude);
            return (axisVector.normalized * magnitude).x;
        }

        static float ApplySmoothing(float thisValue, float lastValue, float deltaTime, float sensitivity) {
            // 1.0f and above is instant (no smoothing).
            if (Mathf.Approximately(sensitivity, 1.0f))
                return thisValue;

            // Apply sensitivity (how quickly the value adapts to changes).
            float maxDelta = deltaTime * sensitivity * 100.0f;

            // Snap to zero when changing direction quickly.
            if (Mathf.Sign(lastValue) != Mathf.Sign(thisValue))
                lastValue = 0.0f;

            return Mathf.MoveTowards(lastValue, thisValue, maxDelta);
        }

        public bool LastChangedAfter(InputDevice otherDevice) { 
            return LastChangeTick > otherDevice.LastChangeTick; 
        }

        public virtual void Vibrate(float leftMotor, float rightMotor) {}

        public void Vibrate(float intensity) {
            Vibrate(intensity, intensity); 
        }

        public override string ToString() {
            return string.Format("[InputDevice ({0}, {1})]", Name, Meta);
        }

    }

}
