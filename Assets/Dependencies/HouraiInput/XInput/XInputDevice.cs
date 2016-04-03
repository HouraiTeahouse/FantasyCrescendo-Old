#if UNITY_STANDALONE_WIN || UNITY_EDITOR
using XInputDotNetPure;

namespace HouraiTeahouse.HouraiInput {
    public class XInputDevice : InputDevice {
        public int DeviceIndex { get; private set; }
        private GamePadState _state;

        public XInputDevice(int deviceIndex)
            : base("XInput Controller") {
            DeviceIndex = deviceIndex;
            SortOrder = deviceIndex;

            Meta = "XInput Controller #" + deviceIndex;

            AddControl(InputTarget.LeftStickX, "LeftStickX");
            AddControl(InputTarget.LeftStickY, "LeftStickY");
            AddControl(InputTarget.RightStickX, "RightStickX");
            AddControl(InputTarget.RightStickY, "RightStickY");

            AddControl(InputTarget.LeftTrigger, "LeftTrigger");
            AddControl(InputTarget.RightTrigger, "RightTrigger");

            AddControl(InputTarget.DPadUp, "DPadUp");
            AddControl(InputTarget.DPadDown, "DPadDown");
            AddControl(InputTarget.DPadLeft, "DPadLeft");
            AddControl(InputTarget.DPadRight, "DPadRight");

            AddControl(InputTarget.Action1, "Action1");
            AddControl(InputTarget.Action2, "Action2");
            AddControl(InputTarget.Action3, "Action3");
            AddControl(InputTarget.Action4, "Action4");

            AddControl(InputTarget.LeftBumper, "LeftBumper");
            AddControl(InputTarget.RightBumper, "RightBumper");

            AddControl(InputTarget.LeftStickButton, "LeftStickButton");
            AddControl(InputTarget.RightStickButton, "RightStickButton");

            AddControl(InputTarget.Start, "Start");
            AddControl(InputTarget.Back, "Back");

            QueryState();
        }

        public override void Update(ulong updateTick, float deltaTime) {
            QueryState();

            UpdateWithValue(InputTarget.LeftStickX, _state.ThumbSticks.Left.X, updateTick);
            UpdateWithValue(InputTarget.LeftStickY, _state.ThumbSticks.Left.Y, updateTick);
            UpdateWithValue(InputTarget.RightStickX, _state.ThumbSticks.Right.X, updateTick);
            UpdateWithValue(InputTarget.RightStickY, _state.ThumbSticks.Right.Y, updateTick);

            UpdateWithValue(InputTarget.LeftTrigger, _state.Triggers.Left, updateTick);
            UpdateWithValue(InputTarget.RightTrigger, _state.Triggers.Right, updateTick);

            UpdateWithState(InputTarget.DPadUp, _state.DPad.Up == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.DPadDown, _state.DPad.Down == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.DPadLeft, _state.DPad.Left == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.DPadRight, _state.DPad.Right == ButtonState.Pressed, updateTick);

            UpdateWithState(InputTarget.Action1, _state.Buttons.A == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.Action2, _state.Buttons.B == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.Action3, _state.Buttons.X == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.Action4, _state.Buttons.Y == ButtonState.Pressed, updateTick);

            UpdateWithState(InputTarget.LeftBumper, _state.Buttons.LeftShoulder == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.RightBumper, _state.Buttons.RightShoulder == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputTarget.LeftStickButton, _state.Buttons.LeftStick == ButtonState.Pressed,
                updateTick);
            UpdateWithState(InputTarget.RightStickButton, _state.Buttons.RightStick == ButtonState.Pressed,
                updateTick);

            UpdateWithState(InputTarget.Start, _state.Buttons.Start == ButtonState.Pressed, updateTick);
            UpdateWithState(InputTarget.Back, _state.Buttons.Back == ButtonState.Pressed, updateTick);
        }

        public override void Vibrate(float leftMotor, float rightMotor) {
            GamePad.SetVibration((PlayerIndex) DeviceIndex, leftMotor, rightMotor);
        }

        private void QueryState() {
            _state = GamePad.GetState((PlayerIndex) DeviceIndex, GamePadDeadZone.Circular);
        }

        public bool IsConnected {
            get { return _state.IsConnected; }
        }
    }
}

#endif
