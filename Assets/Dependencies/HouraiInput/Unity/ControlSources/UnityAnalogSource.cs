using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityAnalogSource : InputControlSource {
        private static string[,] analogQueries;
        private readonly int analogId;


        public UnityAnalogSource(int analogId) {
            this.analogId = analogId;
            SetupAnalogQueries();
        }


        public float GetValue(InputDevice inputDevice) {
            var joystickId = (inputDevice as UnityInputDevice).JoystickId;
            var analogKey = GetAnalogKey(joystickId, analogId);
            return Input.GetAxisRaw(analogKey);
        }


        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }


        private static void SetupAnalogQueries() {
            if (analogQueries == null) {
                analogQueries = new string[UnityInputDevice.MaxDevices, UnityInputDevice.MaxAnalogs];

                for (var joystickId = 1; joystickId <= UnityInputDevice.MaxDevices; joystickId++) {
                    for (var analogId = 0; analogId < UnityInputDevice.MaxAnalogs; analogId++) {
                        analogQueries[joystickId - 1, analogId] = "joystick " + joystickId + " analog " + analogId;
                    }
                }
            }
        }


        private static string GetAnalogKey(int joystickId, int analogId) {
            return analogQueries[joystickId - 1, analogId];
        }
    }
}