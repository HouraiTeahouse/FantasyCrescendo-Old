using UnityEngine;

namespace HouraiTeahouse.HouraiInput {
    public class UnityKeyCodeAxisSource : InputControlSource {
        private readonly KeyCode negativeKeyCode;
        private readonly KeyCode positiveKeyCode;


        public UnityKeyCodeAxisSource(KeyCode negativeKeyCode, KeyCode positiveKeyCode) {
            this.negativeKeyCode = negativeKeyCode;
            this.positiveKeyCode = positiveKeyCode;
        }


        public float GetValue(InputDevice inputDevice) {
            var axisValue = 0;

            if (Input.GetKey(negativeKeyCode)) {
                axisValue--;
            }

            if (Input.GetKey(positiveKeyCode)) {
                axisValue++;
            }

            return axisValue;
        }


        public bool GetState(InputDevice inputDevice) {
            return !Mathf.Approximately(GetValue(inputDevice), 0.0f);
        }
    }
}