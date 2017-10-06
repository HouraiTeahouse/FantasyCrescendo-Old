using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.HouraiInput {

    public class UnityAnalogSource : InputSource {

        static string[,] _analogQueries;
        static UnityAnalogSource() {
            _analogQueries = new string[UnityInputDevice.MaxDevices, UnityInputDevice.MaxAnalogs];

            for (var joystickId = 1; joystickId <= UnityInputDevice.MaxDevices; joystickId++)
                for (var analogId = 0; analogId < UnityInputDevice.MaxAnalogs; analogId++)
                    _analogQueries[joystickId - 1, analogId] = string.Format("joystick {0} analog {1}", joystickId, analogId);
        }

        readonly int _analogId;

        public UnityAnalogSource(int analogId) {
            _analogId = analogId;
        }

        public override float GetValue(InputDevice inputDevice) {
            var unityInputDevice = inputDevice as UnityInputDevice;
            Assert.IsNotNull(unityInputDevice);
            int joystickId = unityInputDevice.JoystickId;
            return Input.GetAxisRaw(_analogQueries[joystickId - 1, _analogId]);
        }

    }

}