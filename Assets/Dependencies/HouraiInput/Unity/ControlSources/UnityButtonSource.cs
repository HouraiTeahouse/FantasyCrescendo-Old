using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public class UnityButtonSource : InputSource {

        static string[,] _buttonQueries;
        static UnityButtonSource() {
            _buttonQueries = new string[UnityInputDevice.MaxDevices, UnityInputDevice.MaxButtons];

            for (var joystickId = 1; joystickId <= UnityInputDevice.MaxDevices; joystickId++)
                for (var buttonId = 0; buttonId < UnityInputDevice.MaxButtons; buttonId++)
                    _buttonQueries[joystickId - 1, buttonId] = string.Format("joystick {0} button {1}", joystickId, buttonId);
        }

        readonly int _buttonId;

        public UnityButtonSource(int buttonId) {
            _buttonId = buttonId;
        }

        public override bool GetState(InputDevice inputDevice) {
            var unityInputDevice = inputDevice as UnityInputDevice;
            if (unityInputDevice == null)
                return false;
            int joystickId = unityInputDevice.JoystickId;
            return Input.GetKey(_buttonQueries[joystickId - 1, _buttonId]);
        }

    }

}